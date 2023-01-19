using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Barcoder;
using Barcoder.Qr;
using Barcoder.Renderer.Image;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Wimi.BtlCore.BasicData.Calendars;
using Wimi.BtlCore.BasicData.DeviceGroups;
using Wimi.BtlCore.BasicData.Shifts;
using Wimi.BtlCore.Trace;

namespace Wimi.BtlCore.Cartons
{
    public class CartonManager : BtlCoreDomainServiceBase
    {
        private readonly IRepository<CartonSetting> cartonSettingRepository;

        private readonly IRepository<TraceCatalog, long> traceCatalogRepository;

        private readonly IRepository<TraceFlowRecord, long> traceFlowRecordRepository;

        private readonly IRepository<TraceFlowSetting> traceFlowSettingRepository;

        private readonly IRepository<CartonRecord> cartonRecordRepository;

        private readonly IRepository<CartonRuleDetail> cartonRuleDetailRepository;

        private readonly IRepository<Carton> cartonRepository;

        private readonly IRepository<MachinesShiftDetail> machinesShiftDetailsRepository;

        private readonly ICalendarRepository calendarRepository;

        private readonly IRepository<CalibratorCode> calibratorCodeRepository;

        private readonly DeviceGroupManager deviceGroupManager;

        private readonly IRepository<CartonSerialNumber, long> cartonSerialNumberRepository;

        private IRepository<DeviceGroup> DeviceGroupRepository;

        private readonly IRepository<ShiftSolutionItem> shiftSolutionItemRepository;

        private readonly ICartonRepository CartonRepository;

        private readonly IPrinterManager printerManager;

        private readonly IWebHostEnvironment hostingEnvironment;

        private readonly object thisLock = new object();

        public CartonManager(
            IRepository<CartonSetting> cartonSettingRepository,
            IRepository<TraceCatalog, long> traceCatalogRepository,
            IRepository<TraceFlowRecord, long> traceFlowRecordRepository,
            IRepository<TraceFlowSetting> traceFlowSettingRepository,
            IRepository<CartonRecord> cartonRecordRepository,
            IRepository<CartonRuleDetail> cartonRuleDetailRepository,
            IRepository<Carton> cartonRepository,
            IRepository<MachinesShiftDetail> machinesShiftDetailsRepository,
            ICalendarRepository calendarRepository,
            IRepository<CalibratorCode> calibratorCodeRepository,
            DeviceGroupManager deviceGroupManager,
            IRepository<CartonSerialNumber, long> cartonSerialNumberRepository,
            IRepository<DeviceGroup> DeviceGroupRepository,
            IRepository<ShiftSolutionItem> shiftSolutionItemRepository,
            ICartonRepository CartonRepository,
            IPrinterManager printerManager,
            IWebHostEnvironment hostingEnvironment)
        {
            this.cartonSettingRepository = cartonSettingRepository;
            this.traceCatalogRepository = traceCatalogRepository;
            this.traceFlowRecordRepository = traceFlowRecordRepository;
            this.traceFlowSettingRepository = traceFlowSettingRepository;
            this.cartonRecordRepository = cartonRecordRepository;
            this.cartonRuleDetailRepository = cartonRuleDetailRepository;
            this.cartonRepository = cartonRepository;
            this.machinesShiftDetailsRepository = machinesShiftDetailsRepository;
            this.calendarRepository = calendarRepository;
            this.calibratorCodeRepository = calibratorCodeRepository;
            this.deviceGroupManager = deviceGroupManager;
            this.cartonSerialNumberRepository = cartonSerialNumberRepository;
            this.DeviceGroupRepository = DeviceGroupRepository;
            this.shiftSolutionItemRepository = shiftSolutionItemRepository;
            this.CartonRepository = CartonRepository;
            this.printerManager = printerManager;
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<CartonSetting> GetCartonSettingByDeviceGroupId(int deviceGroupId)
        {
            var firstClassDeviceGroup = await this.deviceGroupManager.GetFirstClassDeviceGroupById(deviceGroupId);
            if (firstClassDeviceGroup == null)
            {
                throw new UserFriendlyException(L("CantFindFirstDeviceGroupClass"));
            }
            var cartonSetting = await this.cartonSettingRepository.FirstOrDefaultAsync(s => s.DeviceGroupId == firstClassDeviceGroup.Id);
            if (cartonSetting == null)
            {
                throw new UserFriendlyException(L("CantFindPartCartonSetting", firstClassDeviceGroup.DisplayName));
            }
            return cartonSetting;
        }

        public async Task CheckCartonSetting(TraceCatalog traceCatalog, CartonSetting cartonSetting)
        {
            //校验合格工件
            if (cartonSetting.IsGoodOnly)
            {
                this.CheckGood(traceCatalog.PartNo, traceCatalog.Qualified);
            }

            //跳序检查
            if (cartonSetting.ForbidHopSequence)
            {
                await this.CheckHopSequence(traceCatalog.PartNo, cartonSetting.DeviceGroupId);
            }

            //重复装箱检查
            if (cartonSetting.ForbidRepeatPacking)
            {
                await this.CheckRepeatPacking(traceCatalog.PartNo);
            }

            //必须经过流程检查
            if (cartonSetting.HasToFlow)
            {
                await this.CheckHasToFlow(traceCatalog.PartNo, cartonSetting.FlowIds);
            }
        }

        private void CheckGood(string partNo, bool? qualified)
        {
            if (qualified != true)
            {
                throw new UserFriendlyException(L("UnqualifiedCantCarton"));
            }
        }

        private async Task CheckHopSequence(string partNo, int deviceGroupId)
        {
            var realSeqList = await this.traceFlowRecordRepository.GetAll()
                                .Where(t => t.PartNo == partNo)
                                .Join(this.traceFlowSettingRepository.GetAll(), tfr => tfr.TraceFlowSettingId, tfs => tfs.Id, (tfr, tfs) => new { tfr, tfs })
                                .Select(x => x.tfs.FlowSeq)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToListAsync();

            var settingSeqList = await this.traceFlowSettingRepository.GetAll()
                .Where(t => t.DeviceGroupId == deviceGroupId && t.FlowType != FlowType.OptionFlow)
                .Select(t => t.FlowSeq)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            if (string.Join(",", realSeqList) != string.Join(",", settingSeqList))
            {
                throw new UserFriendlyException(L("HopSequenceCantCarton"));
            }

        }

        private async Task CheckRepeatPacking(string partNo)
        {
            var existParts = await this.cartonRecordRepository.FirstOrDefaultAsync(x => x.PartNo == partNo);

            if (existParts != null)
            {
                throw new UserFriendlyException(L("CantRepeatPacking"));
            }
        }

        private async Task CheckHasToFlow(string partNo, string flowId)
        {
            var flowIdList = flowId.Split(',').ToList()
                .Select(x => Convert.ToInt32(x));

            var realIdList = await this.traceFlowRecordRepository.GetAll()
                               .Where(t => t.PartNo == partNo)
                               .Select(t => t.TraceFlowSettingId)
                               .ToListAsync();

            if (!flowIdList.All(f => realIdList.Any(r => r == f)))
            {
                throw new UserFriendlyException(L("HasToFlowCantCarton"));
            }
        }

        public async Task<string> GenerateCartonNoById(int cartonRuleId, TraceCatalog traceCatalog = null)
        {
            var result = string.Empty;
            var isPreview = false;
            var rules = await this.cartonRuleDetailRepository.GetAll()
                .Where(r => r.CartonRuleId == cartonRuleId)
                .OrderBy(r => r.SequenceNo)
                .ToListAsync();

            if (traceCatalog == null)
            {
                isPreview = true;
            }
            var deviceGroupId = 0;
            var shiftItemId = 0;

            if (traceCatalog != null)
            {
                deviceGroupId = (await this.deviceGroupManager.GetFirstClassDeviceGroupById(traceCatalog.DeviceGroupId)).Id;
                shiftItemId = (await this.machinesShiftDetailsRepository.GetAsync(traceCatalog.MachineShiftDetailId)).ShiftSolutionItemId;
            }
            else
            {
                deviceGroupId = this.DeviceGroupRepository.GetAll().FirstOrDefault().Id;
                var shiftItem = this.shiftSolutionItemRepository.GetAll().FirstOrDefault();
                if (shiftItem == null) throw new UserFriendlyException(this.L("NoShiftItems"));
                shiftItemId = shiftItem.Id;
            }
            var isShiftMatch = false;
            var shiftAddTag = false;
            foreach (var item in rules)
            {
                var temp = await this.GeneratePartCartonNoByRuleDetail(item, deviceGroupId, shiftItemId, isPreview);
                if (item.Type == EnumRuleType.Shift && !string.IsNullOrEmpty(temp))
                {
                    isShiftMatch = true;
                }

                if (item.Type == EnumRuleType.Shift && string.IsNullOrEmpty(temp))
                {
                    if (!shiftAddTag && isPreview)
                    {
                        temp = item.Value;
                        shiftAddTag = true;
                    }
                }

                result += temp;
            }

            if (!rules.Any(r => r.Type == EnumRuleType.Shift))
            {
                isShiftMatch = true;
            }

            if (!isShiftMatch && !isPreview)
            {
                throw new UserFriendlyException(this.L("ShiftNotMatch"));
            }

            var carRule = rules.FirstOrDefault(r => r.Type == EnumRuleType.CalibratorCode);
            if (carRule != null)
            {

                var resultR = result.Replace("/CalibratorCode/", "");

                if (string.IsNullOrEmpty(resultR))
                {
                    return "";
                }

                if (carRule.EndIndex > resultR.Length || carRule.StartIndex > resultR.Length)
                {
                    throw new UserFriendlyException("校验长度大于箱码长度，请重新设置校验码");
                    //carRule.EndIndex = resultR.Length;
                    //carRule.StartIndex = carRule.StartIndex > resultR.Length ? resultR.Length : carRule.StartIndex;
                    //await cartonRuleDetailRepository.UpdateAsync(carRule);
                    //return "";
                }

                var carKey = SumCarCode(resultR.Substring(carRule.StartIndex - 1, carRule.EndIndex - carRule.StartIndex + 1)) % Convert.ToInt32(carRule.Value);
                var carValue = (await this.calibratorCodeRepository.FirstOrDefaultAsync(c => c.Key == carKey)).Value;
                result = result.Replace("/CalibratorCode/", carValue);

            }

            return result;
        }

        public async Task<string> GeneratePartCartonNoByRuleDetail(CartonRuleDetail input, int deviceGroupId, int shiftItemId, bool isPreview)
        {
            var result = string.Empty;

            switch (input.Type)
            {
                case EnumRuleType.Ascii:
                    char uinicode = (char)(Convert.ToInt32(input.Value));
                    result = $"{uinicode}";
                    break;
                case EnumRuleType.FixedString:
                    result = input.Value;
                    break;
                case EnumRuleType.Year:
                    result = DateTime.Now.ToString($"{input.Value}");
                    break;
                case EnumRuleType.Month:
                    result = DateTime.Now.ToString("MM");
                    break;
                case EnumRuleType.Day:
                    if (input.Value == "2")
                    {
                        result = DateTime.Now.DayOfYear.ToString();
                    }
                    else
                    {
                        result = DateTime.Now.ToString("dd");
                    }
                    break;
                case EnumRuleType.Quarter:
                    result = (DateTime.Now.Month / 3 + 1).ToString();
                    break;
                case EnumRuleType.Week:
                    var datekey = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                    var date = await this.calendarRepository.GetCalendarsByKey(new EntityDto { Id = datekey });
                    result = date.WeekOfYear.ToString();
                    break;
                case EnumRuleType.Shift:
                    if (input.ExpansionKey == shiftItemId)
                    {
                        result = input.Value;
                    }
                    break;
                case EnumRuleType.Line:
                    if (input.ExpansionKey == deviceGroupId)
                    {
                        result = input.Value;
                    }
                    break;
                case EnumRuleType.SerialNumber:
                    result = await this.GetSerialNo(input, isPreview);
                    break;
                case EnumRuleType.CalibratorCode:
                    result = "/CalibratorCode/";
                    break;
                case EnumRuleType.SpecialCode:
                    for (var i = 0; i < input.Length; i++)
                    {
                        result += "*";
                    }
                    break;
                case EnumRuleType.Time:
                    result = DateTime.Now.ToString($"{input.Value}");
                    break;
                default:
                    break;
            }
            return result;
        }

        private int SumCarCode(string input)
        {
            var sum = 0;
            foreach (var item in input)
            {
                if (Char.IsNumber(item))
                {
                    sum += item;
                }
            }
            return sum;
        }

        private async Task<string> GetSerialNo(CartonRuleDetail input, bool isPreview)
        {
            var format = "";
            for (int i = 0; i < input.Length; i++)
            {
                format += "0";
            }

            if (isPreview)
            {
                return 0.ToString(format);
            }

            var dateKey = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            var canuse = await this.cartonSerialNumberRepository.GetAll()
                .WhereIf(input.ExpansionKey == 1, x => x.DateKey == dateKey)
                .FirstOrDefaultAsync(x => x.Status == SerialNumberStatus.Appointment && input.Length == x.SerialNumber.Length);
            if (canuse != null)
            {
                //更新为已使用成功
                if (this.CartonRepository.AppointNumber(canuse.SerialNumber))
                {
                    return canuse.SerialNumber;
                }
                else
                {
                    return await this.GetSerialNo(input, false);
                }

            }
            else
            {
                var count = await this.cartonSerialNumberRepository
                    .GetAll()
                    .WhereIf(input.ExpansionKey == 1, x => x.DateKey == dateKey)
                    .CountAsync(x => input.Length == x.SerialNumber.Length);
                count++;
                var serNo = count.ToString(format);
                if (serNo.Length != input.Length)
                {
                    throw new UserFriendlyException(L("SeriesNumUseEnd", input.Length));
                }
                this.CartonRepository.InsertCartonSerialNumber(new CartonSerialNumber { DateKey = dateKey, SerialNumber = serNo, Status = SerialNumberStatus.Appointment });
                //await this.cartonSerialNumberRepository.InsertAsync(new CartonSerialNumber { DateKey = dateKey, SerialNumber = serNo, Status = SerialNumberStatus.Appointment });
                //await this.CurrentUnitOfWork.SaveChangesAsync();
                //更新为已使用成功
                if (this.CartonRepository.AppointNumber(serNo))
                {
                    return serNo;
                }
                else
                {
                    return await this.GetSerialNo(input, false);
                }
            }
        }

        public async Task AddPackingRecord(TraceCatalog traceCatalog, CartonSetting cartonSetting, string cartonNo, long? operatorId)
        {
            var pack = await this.cartonRepository.FirstOrDefaultAsync(c => c.CartonNo == cartonNo);

            var shift = await this.machinesShiftDetailsRepository.GetAsync(traceCatalog.MachineShiftDetailId);

            var cartonRecord = new CartonRecord()
            {
                CartonNo = cartonNo,
                PartNo = traceCatalog.PartNo,
                ShiftSolutionItemId = shift.ShiftSolutionItemId,
                ShiftDay = shift.ShiftDay,
                CreatorUserId = operatorId
            };

            if (pack == null)
            {
                var carton = new Carton()
                {
                    CartonNo = cartonNo,
                    DeviceGroupId = traceCatalog.DeviceGroupId,
                    MaxPackingCount = cartonSetting.MaxPackingCount,
                    PrintLabelCount = 0,
                    RealPackingCount = 1,
                    CreatorUserId = operatorId
                };

                //新增 箱
                var cartonId = await this.cartonRepository.InsertAndGetIdAsync(carton);

                cartonRecord.CartonId = cartonId;

                //插入包装箱记录
                await this.cartonRecordRepository.InsertAsync(cartonRecord);
            }
            else
            {
                //插入包装箱记录
                cartonRecord.CartonId = pack.Id;
                if (pack.RealPackingCount >= pack.MaxPackingCount)
                {
                    throw new UserFriendlyException(L("FullCartonCantPutIn", pack.CartonNo));
                }
                await this.cartonRecordRepository.InsertAsync(cartonRecord);
                pack.RealPackingCount++;
                await this.cartonRepository.UpdateAsync(pack);
            }

        }

        //TODO:替换现有生成二维码方案为Core版本的库
        public async Task Print(string cartonNo, string printName)
        {
            string error = string.Empty;

            //打印机的名称
            var printerName = printName;

            var allPrinter = System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToList();
            Logger.Info($"本机已安装的打印机列表：{string.Join(",", allPrinter)}，是否包含配置的打印机：{allPrinter.Contains(printerName, StringComparer.OrdinalIgnoreCase)}");

            if (!string.IsNullOrEmpty(printerName))
            {
                if (allPrinter.Contains(printerName, StringComparer.OrdinalIgnoreCase))
                {
                    ////箱码信息的base64位字符串
                    string base64Carton = string.Empty;

                    var barcode =  QrEncoder.Encode(cartonNo, ErrorCorrectionLevel.M,Encoding.Auto);
                    var renderer = new ImageRenderer();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        renderer.Render(barcode, stream);
                        byte[] arr1 = new byte[stream.Length];
                        stream.Position = 0;
                        stream.Read(arr1, 0, (int)stream.Length);
                        stream.Close();
                        base64Carton = Convert.ToBase64String(arr1);
                    }

                    lock (thisLock)
                    {
                        var path = Path.Combine(hostingEnvironment.WebRootPath, "FRX\\Label.frx");
                        printerManager.PrintLabel(path, printerName, base64Carton, cartonNo, true);
                    }
                }
                else
                {
                    throw new UserFriendlyException(L("CantFindSelectPrinter", printerName));
                }
            }
            else
            {
                throw new UserFriendlyException(L("CantFindAnyPrinter"));
            }

            //打印成功后次数+1
            var carton = await this.cartonRepository.FirstOrDefaultAsync(x => x.CartonNo == cartonNo);
            if (carton != null)
            {
                carton.PrintLabelCount++;
                await this.cartonRepository.UpdateAsync(carton);
            }
        }

        public async Task UpdateCartonMaxCount(int maxCount, string cartonNo, int cartonSettingId)
        {
            var carton = await this.cartonRepository.FirstOrDefaultAsync(c => c.CartonNo == cartonNo);
            if (carton == null)
            {
                throw new UserFriendlyException(L("CantFindCarton"));
            }

            carton.MaxPackingCount = maxCount;
            await this.cartonRepository.UpdateAsync(carton);

            var setting = await this.cartonSettingRepository.GetAsync(cartonSettingId);
            setting.MaxPackingCount = maxCount;
        }
    }
}
