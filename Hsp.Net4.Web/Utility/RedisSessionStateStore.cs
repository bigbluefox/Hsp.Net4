using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Hsp.Net4.Web.Utility
{
    /// <summary>
    /// 使用Cookie实现SessionStateStoreProviderBase
    /// 注意：它只适合保存简单的基元类型数据。
    /// </summary>
    public class RedisSessionStateStore : SessionStateStoreProviderBase
    {
        private const string Stuff = "Session";

        private static string CreateCustomKey(string key)
        {
            return Stuff + ":" + key;
        }

        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return CreateLegitStoreData(context, null, null, timeout);
        }

        internal static SessionStateStoreData CreateLegitStoreData(HttpContext context, ISessionStateItemCollection sessionItems, HttpStaticObjectsCollection staticObjects, int timeout)
        {
            if (sessionItems == null)
                sessionItems = new SessionStateItemCollection();
            if (staticObjects == null && context != null)
                staticObjects = SessionStateUtility.GetSessionStaticObjects(context);
            return new SessionStateStoreData(sessionItems, staticObjects, timeout);
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            RedisSessionState state = new RedisSessionState(null, null, timeout);
            RedisBase.Item_Set<string>(CreateCustomKey(id), state.ToJson(), timeout);
        }

        private SessionStateStoreData DoGet(HttpContext context, string id, bool exclusive, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
        {
            locked = false;
            lockId = null;
            lockAge = TimeSpan.Zero;
            actionFlags = SessionStateActions.None;
            //var json = RedisBase.Item_Get<string>(CreateCustomKey(id));
            var json = RedisBase.Item_Get(CreateCustomKey(id));
            RedisSessionState state = RedisSessionState.FromJson(json);
            if (state == null)
            {
                return null;
            }
            RedisBase.Item_SetExpire(CreateCustomKey(id), state.Timeout);
            return CreateLegitStoreData(context, state.SessionItems, state.StaticObjects, state.Timeout);
        }

        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
        {
            return this.DoGet(context, id, false, out locked, out lockAge, out lockId, out actionFlags);
        }

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actionFlags)
        {
            return this.DoGet(context, id, true, out locked, out lockAge, out lockId, out actionFlags);
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            ISessionStateItemCollection sessionItems = null;
            HttpStaticObjectsCollection staticObjects = null;

            if (item.Items.Count > 0)
                sessionItems = item.Items;
            if (!item.StaticObjects.NeverAccessed)
                staticObjects = item.StaticObjects;

            RedisSessionState state2 = new RedisSessionState(sessionItems, staticObjects, item.Timeout);

            //JsonSerializerSettings settings = new JsonSerializerSettings
            //{
            //    NullValueHandling = NullValueHandling.Include
            //};
            //var json = JsonConvert.SerializeObject(state2, settings);
            //var json1 = JsonConvert.SerializeObject(state2);
            //var json = state2.ToJson();

            RedisBase.Item_Set<string>(CreateCustomKey(id), state2.ToJson(), item.Timeout);
        }

        #region "未实现方法"

        public override void Dispose()
        {

        }

        public override void EndRequest(HttpContext context)
        {
            //
        }

        public override void InitializeRequest(HttpContext context)
        {

        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            RedisBase.Item_Remove(CreateCustomKey(id));
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {

        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return true;
        }

        #endregion

    }

    internal sealed class SessionStateItem
    {
        public Dictionary<string, SaveValue> Dict;
        public int Timeout;
    }

    internal sealed class SaveValue
    {
        public object Value { get; set; }

        public Type Type { get; set; }
    }

    internal sealed class RedisSessionState
    {
        internal ISessionStateItemCollection SessionItems;
        internal HttpStaticObjectsCollection StaticObjects;
        internal int Timeout;

        internal RedisSessionState(ISessionStateItemCollection sessionItems, HttpStaticObjectsCollection staticObjects, int timeout)
        {
            this.Copy(sessionItems, staticObjects, timeout);
        }

        internal void Copy(ISessionStateItemCollection sessionItems, HttpStaticObjectsCollection staticObjects, int timeout)
        {
            this.SessionItems = sessionItems;
            this.StaticObjects = staticObjects;
            this.Timeout = timeout;
        }

        public string ToJson()
        {
            // 这里忽略_staticObjects这个成员。

            if (SessionItems == null || SessionItems.Count == 0)
            {
                return null;
            }

            Dictionary<string, SaveValue> dict = new Dictionary<string, SaveValue>(SessionItems.Count);

            NameObjectCollectionBase.KeysCollection keys = SessionItems.Keys;
            string key;
            object objectValue = string.Empty;
            Type type = null;
            for (int i = 0; i < keys.Count; i++)
            {
                key = keys[i];
                objectValue = SessionItems[key];
                if (objectValue != null)
                {
                    type = objectValue.GetType();
                }
                else
                {
                    type = typeof(object);
                }
                dict.Add(key, new SaveValue { Value = objectValue, Type = type });
            }

            SessionStateItem item = new SessionStateItem { Dict = dict, Timeout = this.Timeout };

            return JsonConvert.SerializeObject(item);
        }

        public static RedisSessionState FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            try
            {
                SessionStateItem item = JsonConvert.DeserializeObject<SessionStateItem>(json);

                SessionStateItemCollection collections = new SessionStateItemCollection();

                SaveValue objectValue = null;

                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = null;
                JsonTextReader tReader = null;

                foreach (KeyValuePair<string, SaveValue> kvp in item.Dict)
                {
                    objectValue = kvp.Value as SaveValue;
                    if (objectValue.Value == null)
                    {
                        collections[kvp.Key] = null;
                    }
                    else
                    {
                        if (!IsValueType(objectValue.Type))
                        {
                            sr = new StringReader(objectValue.Value.ToString());
                            tReader = new JsonTextReader(sr);
                            collections[kvp.Key] = serializer.Deserialize(tReader, objectValue.Type);
                        }
                        else
                        {
                            collections[kvp.Key] = objectValue.Value;
                        }
                    }
                }

                return new RedisSessionState(collections, null, item.Timeout);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 判断是否为值类型
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public static bool IsValueType(Type type)
        {
            if (type.IsValueType)
            {
                return true;
            }
            //基础数据类型
            if (type == typeof(string) || type == typeof(char)
                || type == typeof(ushort) || type == typeof(short) || type == typeof(uint) || type == typeof(int)
                || type == typeof(ulong) || type == typeof(long) || type == typeof(double) || type == typeof(decimal)
                || type == typeof(bool)
                || type == typeof(byte))
            {
                return true;
            }
            //可为null的基础数据类型
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                Type genericType = type.GetGenericTypeDefinition();

                if (Object.ReferenceEquals(genericType, typeof(Nullable<>)))
                {
                    var actualType = type.GetGenericArguments()[0];
                    return IsValueType(actualType);

                }
            }
            return false;
        }
    }
}