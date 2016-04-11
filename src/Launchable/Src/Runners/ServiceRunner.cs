using System;
using System.IO;
using System.ServiceProcess;
using Launchable.Helpers;

namespace Launchable.Runners
{
    class ServiceRunner : ServiceBase
    {
        static ServiceRunner()
        {
            DirectoryHelpers.SetCurrentDirectory();
        }

        private readonly ILaunchable _instance;

        public ServiceRunner(Func<ILaunchable> factory)
        {
            _instance = factory();

            CanHandlePowerEvent = false;
            CanHandleSessionChangeEvent = false;
            CanPauseAndContinue = false;
            CanShutdown = false;
            CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _instance.Start();
                base.OnStart(args);
            }
            catch (Exception ex)
            {
                Log.StartupError(ex.ToString());
                TryDispose();
                Environment.Exit(-1);
            }
        }

        private void TryDispose()
        {
            try
            {
                _instance?.Dispose();
            }
            catch
            {
                // Not much we can do here
            }
        }

        protected override void OnStop()
        {
            try
            {
                _instance.Dispose();
            }
            catch (Exception ex)
            {
                File.WriteAllText("stop.err.log", ex.ToString());
            }
        }

        public static void Run(Func<ILaunchable> factory)
        {
            Run(new ServiceRunner(factory));
        }
    }
}
