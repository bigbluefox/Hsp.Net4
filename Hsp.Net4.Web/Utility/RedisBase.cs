using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hsp.Net4.Web.Utility
{
    /// <summary>
    /// RedisBase
    /// </summary>
    public class RedisBase
    {
        public RedisBase() { }


        #region -- Item --

        /// <summary> 
        /// 设置单体 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="t"></param> 
        /// <returns></returns> 
        public static bool Item_Set<T>(string key, T t)
        {
            try
            {
                //using (IRedisClient redis = prcm.GetClient())
                //{
                //    return redis.Set<T>(key, t, new TimeSpan(1, 0, 0));
                //}

                return RedisHelper.Set(key, t, new TimeSpan(1, 0, 0));
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary> 
        /// 设置单体 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <param name="t"></param>
        /// <param name="timeout">分钟</param>
        /// <returns></returns> 
        public static bool Item_Set<T>(string key, T t, int timeout)
        {
            try
            {
                //var abc = key;

                //using (IRedisClient redis = prcm.GetClient())
                //{
                //    return redis.Set<T>(key, t, new TimeSpan(0, timeout, 0));
                //}

                return RedisHelper.Set(key, t, new TimeSpan(0, timeout, 0));
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary> 
        /// 获取单体 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public static T Item_Get<T>(string key) where T : class
        {
            //using (IRedisClient redis = prcm.GetReadOnlyClient())
            //{
            //    return redis.Get<T>(key);
            //}
            var str = RedisHelper.Get(key);
            var obj = JsonConvert.DeserializeObject<T>(str);
            return obj;
        }

        /// <summary> 
        /// 获取单体 
        /// </summary> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        public static string Item_Get(string key)
        {
            return RedisHelper.Get(key);
        }

        /// <summary> 
        /// 设置缓存过期 
        /// </summary> 
        /// <param name="key"></param> 
        public static bool Item_Remove(string key)
        {
            //using (IRedisClient redis = prcm.GetClient())
            //{
            //    return redis.Remove(key);
            //}

            return RedisHelper.Del(key) > 0;
        }

        /// <summary> 
        /// 设置缓存过期 
        /// </summary> 
        /// <param name="key"></param>
        /// <param name="timeout">分钟</param> 
        public static bool Item_SetExpire(string key, int timeout)
        {
            //using (IRedisClient redis = prcm.GetClient())
            //{
            //    return redis.ExpireEntryIn(key, new TimeSpan(0, timeout, 0));
            //}

            return RedisHelper.Expire(key, new TimeSpan(0, timeout, 0));

        }

        #endregion
    }
}