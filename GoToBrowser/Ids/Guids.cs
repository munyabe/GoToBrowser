// Guids.cs
// MUST match guids.h
using System;

namespace GoToBrowser
{
    static class GuidList
    {
        public const string guidGoToBrowserPkgString = "cd8f7ba3-3b25-4272-9b03-be6e009378da";
        public const string guidGoToBrowserCmdSetString = "55ba95ee-03a2-4f1f-8107-8148a9dfea43";

        public static readonly Guid guidGoToBrowserCmdSet = new Guid(guidGoToBrowserCmdSetString);
    };
}