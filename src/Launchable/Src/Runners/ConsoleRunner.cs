using System;
using System.Reflection;
using System.Threading;
using Launchable.Helpers;

namespace Launchable.Runners
{
    public class ConsoleRunner
    {
        static ConsoleRunner()
        {
            DirectoryHelpers.SetCurrentDirectory();
        }

        private readonly ILaunchable _instance;
        private readonly ManualResetEventSlim _cancelEvent = new ManualResetEventSlim(false);

        public ConsoleRunner(Func<ILaunchable> factory)
        {
            _instance = factory();
        }

        public void Run()
        {
            Console.WriteLine($"Starting {Assembly.GetEntryAssembly().FullName} in console, press CTRL+C to exit . . .");
            Console.WriteLine();
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            _instance.Start();
            _cancelEvent.Wait();
            Console.WriteLine("CTRL+C pressed, stopping...");
            _instance.Dispose();
        }

        private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            _cancelEvent.Set();
        }

        public static void Run(Func<ILaunchable> factory)
        {
            new ConsoleRunner(factory).Run();
        }
    }
}