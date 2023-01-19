using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Wimi.BtlCore.Runtime.Exception;

namespace Wimi.BtlCore.Extensions
{
    /// <summary>
    /// 日志记录扩展方法
    /// </summary>
    public static class LoggerExtensions
    {
        public static void DebugToInfluxdb(this ILogger logger, Type type, string message, string methodName = "")
        {
            LogToInfluxdb(logger, LoggerLevel.Debug, type, message, methodName);
        }

        public static void ErrorToInfluxdb(this ILogger logger, Type type, string message, string methodName = "")
        {
            LogToInfluxdb(logger, LoggerLevel.Error, type, message, methodName);
        }

        public static void FatalToInfluxdb(this ILogger logger, Type type, string message, string methodName = "")
        {
            LogToInfluxdb(logger, LoggerLevel.Fatal, type, message, methodName);
        }

        public static void InfoToInfluxdb(this ILogger logger, Type type, string message, string methodName = "")
        {
            LogToInfluxdb(logger, LoggerLevel.Info, type, message, methodName);
        }

        public static void WarnToInfluxdb(this ILogger logger, Type type, string message, string methodName = "")
        {
            LogToInfluxdb(logger, LoggerLevel.Warn, type, message, methodName);
        }

        /// <summary>
        /// WebService接口调用日志
        /// </summary>
        public static void LogToInfluxdb(this ILogger logger, string measurement, Dictionary<string, object> fields, Dictionary<string, string> tags = null)
        {
            InfluxdbManager.Save(measurement, fields, tags);
        }

        private static void LogToInfluxdb(this ILogger logger, LoggerLevel loggerLevel, Type type, string message, string methodName = "")
        {
            InfluxdbManager.Save("MDCLogs",
                new Dictionary<string, object>()
                {
                    { "EventTime",DateTime.Now},
                    { "Message",message },
                    { "Type",type?.Name??"NULL"},
                    { "MethodName",string.IsNullOrEmpty(methodName)?"":methodName},
                },
                new Dictionary<string, string>()
                {
                    {"LoggerLevel",LoggerLevel.Warn.ToString() }
                }
                );
        }

    }
}
