using CSRedis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Hsp.Net4.Web.Utility
{

    /// <summary>
    /// CSRedisClient 单例
    /// </summary>
    public class CSRedisInstance
    {
        //3.在实际应用中，有可能会出现程序与Redis服务连接不稳定的情况，如果Redis服务没有发现问题的话，
        //可以尝试使用下面三种方式解决(参考 https://github.com/2881099/csredis/issues)
        //connectTimeout= 30000 设置连接超时时间
        //tryit = 3 设置重试次数
        //preheat = 100 预热连接数

        private static readonly object _lock = new object();
        private static CSRedisClient _csRedis = null;

        //private const string ip = "127.0.0.1";
        //private const string port = "6379";
        //private const string preheat = "100"; // 设置预热连接数
        //private const string connectTimeout = "100"; // 设置连接超时时间
        //private const string tryit = "1"; // 设置重试次数
        //private const string prefix = "CSRedisTest."; // 设置前缀
        //private static readonly string _connectString = $"{ip}:{port}," +
        //  $"preheat={preheat},connectTimeout={connectTimeout},tryit={tryit},prefix={prefix}";

        private static readonly string _connectString = ConfigurationManager.AppSettings["RedisConnection"].ToString();

        internal static CSRedisClient GetRedis()
        {
            if (_csRedis == null)
            {
                lock (_lock)
                {
                    if (_csRedis == null)
                    {
                        _csRedis = GetCSRedisClient();
                    }
                }
            }

            return _csRedis;
        }

        private static CSRedisClient GetCSRedisClient()
        {
            return new CSRedisClient(_connectString);
        }

    }
}