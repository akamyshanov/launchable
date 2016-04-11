[![Build status](https://ci.appveyor.com/api/projects/status/lgmj67l8r3pu3h2u?svg=true)](https://ci.appveyor.com/project/akamyshanov/launchable)

# Launchable
Easily install your .NET console apps as windows services. Internally uses `sc` to install, uninstall, modify, start and stop services.

Uses entry assembly's:
- name for the service name (spaces get replaced by `-`)
- `AssemblyTitleAttribute` for the service display name
- `AssemblyDescriptionAttribute` for the service description

## Sample
```C#
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

        static void Main(string[] args)
        {
            LaunchableRunner.Run<Program>(args);
        }
    }
}
```
