using AgileConfig.Client;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Wimi.BtlCore.Web.Host.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    //new一个client实例
                    //使用无参构造函数会自动读取本地appsettings.json文件的AgileConfig节点的配置
                    var configClient = new ConfigClient();
                    //使用AddAgileConfig配置一个新的IConfigurationSource
                    config.AddAgileConfig(configClient);
                    //注册配置项修改事件
                    configClient.ConfigChanged += (arg) =>
                    {
                        Console.WriteLine($"action:{arg.Action} key:{arg.Key}");
                    };
                })
                .UseStartup<Startup>()
                .UseUrls("http://*:5000")
                .Build();
        }
    }
}
