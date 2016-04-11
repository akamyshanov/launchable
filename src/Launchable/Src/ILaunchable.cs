using System;

namespace Launchable
{
    public interface ILaunchable : IDisposable
    {
        void Start();
    }
}
