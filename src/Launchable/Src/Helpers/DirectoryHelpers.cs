using System.IO;
using System.Reflection;

namespace Launchable.Helpers
{
    public static class DirectoryHelpers
    {
        public static void SetCurrentDirectory()
        {
            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (dir != null)
            {
                Directory.SetCurrentDirectory(dir);
            }
        }

    }
}