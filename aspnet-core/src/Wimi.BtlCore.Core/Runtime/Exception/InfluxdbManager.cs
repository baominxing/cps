using Abp.Events.Bus.Exceptions;
using Abp.Threading;
using Flurl.Http;
using InfluxDB.Collector;
using System;
using System.Collections.Generic;
using System.Linq;
using Wimi.BtlCore.Configuration;

namespace Wimi.BtlCore.Runtime.Exception
{
    public class InfluxdbManager : BtlCoreDomainServiceBase
    {
        private string InfluxdbApiAddress => $"{AppSettings.Influxdb.Address}/query";

        /// <summary>
        /// 初始化Influxdb，修改默认保留策略 = 30天
        /// </summary>
        public void Initialize()
        {
            try
            {
                //创建数据库
                AsyncHelper.RunSync(async () => await InfluxdbApiAddress.PostUrlEncodedAsync(new { q = $@"CREATE DATABASE { AppSettings.ProjectTag} WITH DURATION 30d NAME Keep_30_Day "}));
            }
            catch (System.Exception e)
            {
                this.Logger.Fatal($"设置Influxdb默认保留策略失败，原因:{e}");
            }
        }

        public void Save(AbpHandledExceptionData data)
        {
            try
            {
                var query = StackTraceParser.Parse(
                    data.Exception.StackTrace,
                    (f, t, m, pl, ps, fn, ln) => new
                    {
                        Frame = f,
                        Type = t,
                        Method = m,
                        ParameterList = pl,
                        Parameters = ps,
                        File = fn,
                        Line = ln,
                    }).Where(t => t.File.Contains("Wimi")).ToList();


                Metrics.Collector = GetCollectorConfiguration();
                foreach (var item in query)
                {
                    Metrics.Write("MDCException",
                        new Dictionary<string, object>
                        {
                            {"EventTime", data.EventTime},
                            {"Line", item.Line},
                            {"Frame", item.Frame},
                            {"ParameterList", item.ParameterList},
                            {"Message", data.Exception.Message},
                            {"StackTrace", data.Exception.StackTrace},

                        }, new Dictionary<string, string>
                        {
                            {"Type", item.Type},
                            {"File", item.File.Split(new[] {@"\"}, StringSplitOptions.RemoveEmptyEntries).Last()}
                        });
                }
            }
            catch (System.Exception e)
            {
                this.Logger.Fatal($"保存异常到Influxdb 失败，原因: {e}");
            }
            finally
            {
                Metrics.Collector.Dispose();
                Metrics.Close();
            }
        }

        public static void Save(string measurement, Dictionary<string, object> fields, Dictionary<string, string> tags = null)
        {
            try
            {
                Metrics.Collector = GetCollectorConfiguration();

                Metrics.Write(measurement, fields, tags);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                Metrics.Collector.Dispose();
                Metrics.Close();
            }
        }

        private static MetricsCollector GetCollectorConfiguration()
        {
            return new CollectorConfiguration()
                .WriteTo.InfluxDB(AppSettings.Influxdb.Address, AppSettings.ProjectTag, "btlsystem", "123qwe")
                .CreateCollector();
        }

    }
}
