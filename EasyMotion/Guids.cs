// Guids.cs
// MUST match guids.h
using System;

namespace EasyMotion
{
    static class GuidList
    {
        public const string guidEasyMotionPkgString = "5bb019ad-a18e-4c86-af3c-27217853df95";
        public const string guidEasyMotionCmdSetString = "907a344a-75bb-4df4-a3b3-9bc3299593b7";

        public static readonly Guid guidEasyMotionCmdSet = new Guid(guidEasyMotionCmdSetString);
    };
}