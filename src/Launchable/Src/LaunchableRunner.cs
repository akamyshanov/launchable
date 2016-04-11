using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Launchable.Helpers;
using Launchable.Runners;

namespace Launchable
{
    public class LaunchableRunner
    {
        static LaunchableRunner()
        {
            DirectoryHelpers.SetCurrentDirectory();
        }

        public static void Run(Func<ILaunchable> factory, string[] args, bool throwOnError = false)
        {
            try
            {
                RunImpl(factory, args);
            }
            catch (Exception ex)
            {
                Log.StartupError(ex.ToString());
                if (throwOnError)
                {
                    throw;
                }
            }
        }

        public static void Run<T>(string[] args, bool throwOnError = false) where T : ILaunchable, new()
        {
            Run(() => new T(), args, throwOnError);
        }

        private static void RunImpl(Func<ILaunchable> factory, string[] args)
        {
            args = args ?? new string[0];

            if (!args.Any())
            {
                ConsoleRunner.Run(factory);
                return;
            }

            switch (args[0].ToLower())
            {
                case "--service":
                    ServiceRunner.Run(factory);
                    break;
                case "--console":
                    ConsoleRunner.Run(factory);
                    break;

                case "--install":
                case "-i":
                    ServiceActions.Install(args.Length > 1 ? args[1] : null);
                    break;
                case "--uninstall":
                case "-u":
                    ServiceActions.Uninstall();
                    break;
                case "--start":
                    ServiceActions.Start();
                    break;
                case "--stop":
                    ServiceActions.Stop();
                    break;

                default:
                    ShowUsage();
                    break;
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine($"{Path.GetFileName(Assembly.GetEntryAssembly().Location)} [<args>]");
            Console.WriteLine("To launch in console, omit any arguments");
            Console.WriteLine("Arguments:");
            Console.WriteLine("  -c, --console".PadRight(36) + "Run in console (with passed args)");
            Console.WriteLine("  -i, --install [serviceName]".PadRight(36) + "Installs the service [with a custom service name]");
            Console.WriteLine("  -u, --uninstall [serviceName]".PadRight(36) + "Uninstalls the service [with a custom service name]");
            Console.WriteLine("  --start".PadRight(36) + "Starts the service");
            Console.WriteLine("  --stop".PadRight(36) + "Stops the service");
        }
    }
}