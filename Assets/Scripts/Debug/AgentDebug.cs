using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Debugging
{
    public static class AgentDebug
    {
        private const string SessionId = "6dca8d";
        private static string LogFilePath => Path.Combine(Application.dataPath, "debug-6dca8d.log");

        public static void Log(string hypothesisId, string location, string message, string dataJson, string runId = "pre-fix")
        {
            try
            {
                var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var id = $"log_{ts}_{Guid.NewGuid():N}";
                var line =
                    "{\"sessionId\":\"" + Escape(SessionId) + "\"" +
                    ",\"id\":\"" + Escape(id) + "\"" +
                    ",\"timestamp\":" + ts +
                    ",\"location\":\"" + Escape(location) + "\"" +
                    ",\"message\":\"" + Escape(message) + "\"" +
                    ",\"data\":" + (string.IsNullOrWhiteSpace(dataJson) ? "{}" : dataJson) +
                    ",\"runId\":\"" + Escape(runId) + "\"" +
                    ",\"hypothesisId\":\"" + Escape(hypothesisId) + "\"}";

                File.AppendAllText(LogFilePath, line + "\n");
            }
            catch
            {
                // ignore
            }
        }

        private static string Escape(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}

