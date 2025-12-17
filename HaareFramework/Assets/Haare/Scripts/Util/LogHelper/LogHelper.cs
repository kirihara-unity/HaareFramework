using UnityEngine;

namespace Haare.Util.LogHelper
{
    public static class LogHelper
    {
        // Harre 기본
        public static void Log(string header, string message)
        {
            Debug.Log($"{header} {message}");
        }
        public static void LogTask(string header, string message)
        {
            Debug.Log($"{TASK} / {header} {message}");
        }
        // Harre 경고
        public static void Warning(string header, string message)
        {
            Debug.LogWarning($"<b><color=yellow>[{header}]</color></b> {message}");
        }

        // Harre 에러
        public static void Error(string header, string message)
        {
            Debug.LogError($"<b><color=red>[{header}]</color></b> {message}");
        }

        public static string TASK = $"<b><color=green>[TASK]</color></b>";
        public static string CANCELLED = $"<b><color=yellow>[CTS]</color></b>";
        public static string FRAMEWORK = $"<b><color=cyan>[HAARE]</color></b>";
        public static string DEMO = "<b><color=gray>[DEMO]</color></b>";
        public static string ASSETLOADER = "<b><color=gray>[ASSETLOADER]</color></b>";
        public static string DATAMANAGER = "<b><color=gray>[DATAMANAGER]</color></b>";
    }
}