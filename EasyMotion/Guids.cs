// Guids.cs
// MUST match guids.h
using System;

namespace JaredPar.EasyMotion
{
    static class GuidList
    {
        public const string guidEasyMotionPkgString = "964abc60-3497-47df-9eee-029d3085c9fc";
        public const string guidEasyMotionCmdSetString = "1ddfffc7-09e1-4c00-ba6e-a2d02e217fd1";

        public static readonly Guid guidEasyMotionCmdSet = new Guid(guidEasyMotionCmdSetString);
    };
}