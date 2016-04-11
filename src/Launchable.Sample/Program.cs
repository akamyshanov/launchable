using System;
using System.IO;

namespace Launchable.Sample
{
    class Program : ILaunchable
    {
        public void Dispose()
        {
            Log("Stopping");
        }

        public void Start()
        {
            Log("Starting");
        }

        private static void Log(string msg)
        {
            msg = DateTime.Now.ToString("R") + " / " + msg;

            Console.WriteLine(msg);
            File.AppendAllText("log.txt", msg + Environment.NewLine);
        }

        private static void Main(string[] args)
        {
            LaunchableRunner.Run<Program>(args);
        }
    }
}
