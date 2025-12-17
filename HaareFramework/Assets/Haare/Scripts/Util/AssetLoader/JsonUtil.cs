using System;
using UnityEngine;

namespace Haare.Util.Loader
{
    public class JsonUtil
    {
        public static class JsonUtilityGeneric
        {
            public static object FromJson(string json, Type targetType)
            {
                var method = typeof(JsonUtility).GetMethod(nameof(JsonUtility.FromJson), 
                    new Type[] { typeof(string) });
                var genericMethod = method.MakeGenericMethod(targetType);
                return genericMethod.Invoke(null, new object[] { json });
            }
        }
    }
}