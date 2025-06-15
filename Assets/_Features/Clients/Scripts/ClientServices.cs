using System;

namespace Quackery
{
    public static class ClientServices
    {
        internal static Action ForgetAilment = delegate { };

        internal static Func<bool> HasNextClient = () => true;

    }
}
