using CSRedis;
using Hsp.Net4.Common.Data;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hsp.Net4.Web.Utility
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            // 初始化RedisHelper
            //RedisHelper.Initialization(CSRedisInstance.GetRedis());
        }

        public static string Db_ConnectionString { get; set; } = DBHelper.Get_ConnectionString();

        public static SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = Db_ConnectionString,
            DbType = SqlSugar.DbType.SqlServer,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.SystemTable
        });

        public static SqlSugarScope scope = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = Db_ConnectionString,
            DbType = SqlSugar.DbType.SqlServer,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.SystemTable
        });
    }

}