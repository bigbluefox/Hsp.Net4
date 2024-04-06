using CSRedis;
using Hsp.Net4.Common.Data;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Razor.Tokenizer;
using System.Web.UI;

namespace Hsp.Net4.Web.Utility
{
    public class PageBase : Page
    {
        public static readonly SqlSugarClient db;

        public static readonly SqlSugarScope scope;

        static PageBase()
        {
            // 初始化RedisHelper
            RedisHelper.Initialization(CSRedisInstance.GetRedis());

            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConfigId = "0",
                DbType = DbType.SqlServer,
                ConnectionString = DBHelper.Get_ConnectionString(),
                IsAutoCloseConnection = true
            });

            scope = new SqlSugarScope(new ConnectionConfig()
            {
                ConfigId = "1",
                DbType = DbType.SqlServer,
                ConnectionString = DBHelper.Get_ConnectionString(),
                IsAutoCloseConnection = true
            });
        }


    }

}