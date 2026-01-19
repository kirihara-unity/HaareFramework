using UnityEngine;
using System.Text;

namespace Haare.Util.Logger
{
    public static class LogHelper
    {
        
        // ==========================================
        // 1. 기본 로그 (Log, LogTask)
        // ==========================================
        public static void Log(params string[] contents)
        {
            if (contents == null || contents.Length == 0) return;

            // 메시지와 헤더 분리 및 조합
            (string headers, string message) = ParseContents(contents);
            
            Debug.Log($"{headers}{message}");
        }
        
        public static void LogTask(params string[] contents)
        {
            if (contents == null || contents.Length == 0) return;

            (string headers, string message) = ParseContents(contents);

            // TASK 태그를 맨 앞에 붙여서 출력
            Debug.Log($"{TASK} {headers}{message}");
        }

        // ==========================================
        // 2. 경고 및 에러 (Warning, Error)
        // ==========================================
        public static void Warning(params string[] contents)
        {
            if (contents == null || contents.Length == 0) return;

            string message = contents[contents.Length - 1];
            StringBuilder headerBuilder = new StringBuilder();

            // 마지막 요소(메시지) 전까지 루프
            for (int i = 0; i < contents.Length - 1; i++)
            {
                // 각 헤더마다 노란색 대괄호 서식 적용
                headerBuilder.Append($"<b><color=yellow>[{contents[i]}]</color></b> ");
            }

            Debug.LogWarning($"{headerBuilder}{message}");
        }
        
        public static void Error(params string[] contents)
        {
            if (contents == null || contents.Length == 0) return;

            string message = contents[contents.Length - 1];
            StringBuilder headerBuilder = new StringBuilder();

            for (int i = 0; i < contents.Length - 1; i++)
            {
                // 각 헤더마다 빨간색 대괄호 서식 적용
                headerBuilder.Append($"<b><color=red>[{contents[i]}]</color></b> ");
            }

            Debug.LogError($"{headerBuilder}{message}");
        }

        // ==========================================
        // 3. 내부 유틸리티
        // ==========================================
        private static (string headers, string message) ParseContents(string[] contents)
        {
            if (contents.Length == 1) return ("", contents[0]);

            string message = contents[contents.Length - 1]; // 마지막이 메시지
            
            StringBuilder sb = new StringBuilder();
            // 마지막 전까지 모두 헤더로 취급하여 합침
            for (int i = 0; i < contents.Length - 1; i++)
            {
                sb.Append(contents[i]).Append(" "); // 헤더 사이에 공백 추가
            }

            return (sb.ToString(), message);
        }
    
        
        public static string DEMO    = $"<b><color=#00FF00>[DEMO]</color></b>";   
        
        public static string CANCELLED = $"<b><color=yellow>[CTS]</color></b>";       
        public static string TASK      = $"<b><color=teal>[TASK]</color></b>";        
        
        public static string SERVER    = $"<b><color=#00FF00>[SERVER]</color></b>";    
        public static string CLIENT    = $"<b><color=cyan>[TWINKLE]</color></b>";     
        public static string FRAMEWORK = $"<b><color=lightgreen>[HAARE]</color></b>";     
        public static string SERVICE   = $"<b><color=#6495ED>[SERVICE]</color></b>";  
        
        public static string ASSETLOADER = $"<b><color=orange>[ASSETLOADER]</color></b>"; 
        public static string DATAMANAGER = $"<b><color=silver>[DATAMANAGER]</color></b>"; 
    }
}