namespace Wimi.BtlCore.Authorization.Users.Exporting
{
    using Abp.Collections.Extensions;
    using Abp.Runtime.Session;
    using Abp.Timing.Timezone;
    using System.Collections.Generic;
    using System.Linq;
    using Wimi.BtlCore.Authorization.Users.Dto;
    using Wimi.BtlCore.DataExporting.Excel.EpPlus;
    using Wimi.BtlCore.Dto;

    public class UserListExcelExporter : EpPlusExcelExporterBase, IUserListExcelExporter
    {
        private readonly IAbpSession abpSession;

        private readonly ITimeZoneConverter timeZoneConverter;

        public UserListExcelExporter(ITimeZoneConverter timeZoneConverter, IAbpSession abpSession)
        {
            this.timeZoneConverter = timeZoneConverter;
            this.abpSession = abpSession;
        }

        public FileDto ExportToFile(List<UserListDto> userListDtos)
        {
            return this.CreateExcelPackage(
                "UserList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(this.L("Users"));
                    sheet.OutLineApplyStyle = true;

                    this.AddHeader(
                        sheet,
                        this.L("Name"),
                        this.L("Surname"),
                        this.L("UserName"),
                        this.L("EmailAddress"),
                        this.L("EmailConfirm"),
                        this.L("Roles"),
                        this.L("LastLoginTime"),
                        this.L("Active"),
                        this.L("CreationTime"));

                    this.AddObjects(
                        sheet,
                        2,
                        userListDtos,
                        _ => _.Name,
                        _ => _.Surname,
                        _ => _.UserName,
                        _ => _.EmailAddress,
                        _ => _.IsEmailConfirmed,
                        _ => _.Roles.Select(r => r.RoleName).JoinAsString(", "),
                        _ =>
                        this.timeZoneConverter.Convert(
                            _.LastLoginTime,
                            this.abpSession.TenantId,
                            this.abpSession.GetUserId()),
                        _ => _.IsActive,
                        _ =>
                        this.timeZoneConverter.Convert(
                            _.CreationTime,
                            this.abpSession.TenantId,
                            this.abpSession.GetUserId()));

                    // Formatting cells
                    var lastLoginTimeColumn = sheet.Column(7);
                    lastLoginTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    var creationTimeColumn = sheet.Column(9);
                    creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    for (var i = 1; i <= 7; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }
    }
}
