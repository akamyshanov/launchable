using System;
using System.IO;

namespace Launchable.Helpers
{
    static class Log
    {
        public static void StartupError(string msg)
        {
            Console.WriteLine(msg);
            msg = DateTime.Now.ToString("R") + Environment.NewLine + msg;
            File.WriteAllText("startup.error.log", msg);
        }
    }
}