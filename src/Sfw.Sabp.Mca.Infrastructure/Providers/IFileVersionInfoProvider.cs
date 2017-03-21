using System;

namespace Sfw.Sabp.Mca.Infrastructure.Providers
{
    public interface IFileVersionInfoProvider
    {
        string GetCopyright(Type type);
    }
}