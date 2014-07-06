using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion
{
    interface IEasyMotionUtil
    {
        ITextView TextView {get; }

        bool Enabled { get; set; }

        event EventHandler EnabledChanged;
    }

    interface IEasyMotionUtilProvider
    {
        IEasyMotionUtil GetEasyMotionUtil(ITextView textView);
    }
}
