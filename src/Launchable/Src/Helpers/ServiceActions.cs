using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Launchable.Helpers
{
    public static class ServiceActions
    {
        private static readonly Assembly EntryAssembly = Assembly.GetEntryAssembly();
        private const string ServiceNamePath = "service.name.txt";

        public static void Install(string customServiceName = null)
        {
            var serviceName = (customServiceName ?? EntryAssembly.GetName().Name).Replace(' ', '-');
            Console.WriteLine("Creating service {0} ...", serviceName);

            if (Sc($"create {serviceName} binpath= \"{EntryAssembly.Location} --service\" start= auto") != 0)
            {
                return;
            }

            SaveServiceName(serviceName);

            Console.WriteLine("Modifying properties of service {0} ...", serviceName);
            Sc($"failure {serviceName} reset= 60 actions= restart/30000");

            var displayName = EntryAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title.Trim();
            if (!String.IsNullOrWhiteSpace(displayName))
            {
                Sc($"config {serviceName} displayname=\"{displayName}\"");
            }

            var description = EntryAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description.Trim();
            if (!String.IsNullOrWhiteSpace(description))
            {
                Sc($"description {serviceName} \"{description}\"");
            }
        }

        public static void Uninstall()
        {
            var serviceName = GetServiceName();
            Stop(serviceName);
            Console.WriteLine("Uninstalling service {0} ...", serviceName);
            Sc($"delete {serviceName}");
        }

        public static void Start()
        {
            var serviceName = GetServiceName();
            Console.WriteLine("Starting service {0} ...", serviceName);
            Sc($"start {serviceName}");
        }

        public static void Stop()
        {
            Stop(GetServiceName());
        }

        private static void Stop(string serviceName)
        {
            if (String.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentOutOfRangeException(nameof(serviceName));
            }
            Console.WriteLine("Stopping service {0} ...", serviceName);
            Sc($"stop {serviceName}");
        }

        private static int Sc(string arguments)
        {
            var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "sc",
                    Arguments = arguments,
                    UseShellExecute = false
                });
            process?.WaitForExit();
            return process?.ExitCode ?? -1;
        }

        private static void SaveServiceName(string name)
        {
            File.WriteAllText(ServiceNamePath, name);
        }

        private static string GetServiceName()
        {
            if (File.Exists(ServiceNamePath))
            {
                var customServiceName = File.ReadAllText(ServiceNamePath).Trim();
                if (!String.IsNullOrEmpty(customServiceName))
                {
                    return customServiceName;
                }
            }

            return EntryAssembly.GetName().Name;
        }
    }
}
