using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion.Implementation.EasyMotionUtil
{
    [Export(typeof(IEasyMotionUtilProvider))]
    internal sealed class EasyMotionUtilProvider : IEasyMotionUtilProvider
    {
        private static readonly object Key = new object();

        public IEasyMotionUtil GetEasyMotionUtil(ITextView textView)
        {
            return textView.Properties.GetOrCreateSingletonProperty(Key, () => new EasyMotionUtil(textView));
        }
    }
}
