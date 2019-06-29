using System;
using System.IO;

namespace EnhancedDisastersMod
{
    public static class DebugLogger
    {
        private static string fileName = "EnhancedDisastersMod.log";
        public static bool IsDebug = false;

        public static void Log(string msg)
        {
            if (IsDebug)
            {
                File.AppendAllText(geFilePath(), msg + Environment.NewLine);
            }
        }

        private static string geFilePath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Combine(path, "Colossal Order");
            path = Path.Combine(path, "Cities_Skylines");
            path = Path.Combine(path, fileName);
            return path;
        }
    }
}
