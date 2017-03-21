using System;
using System.Diagnostics;
using System.Reflection;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public class FileVersionInfoProvider : IFileVersionInfoProvider
    {
        public string GetCopyright(Type type)
        {
            var assembly = Assembly.GetAssembly(type).Location;
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly);

            return versionInfo.LegalCopyright;
        }
    }
}
