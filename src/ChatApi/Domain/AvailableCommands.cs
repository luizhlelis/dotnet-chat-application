using System;
using System.Collections.Generic;

namespace ChatApi.Domain
{
    public static class AvailableCommands
    {
        public static HashSet<string> Get() => new HashSet<string>() { "stock" };
    }
}
