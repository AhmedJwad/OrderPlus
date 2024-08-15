using System.Runtime.InteropServices;

namespace OrderPlus.Backend.Helpers
{
    public interface IRuntimeInformationWrapper
    {
        bool IsOSPlatform(OSPlatform osPlatform);
    }
}

