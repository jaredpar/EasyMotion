using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace EasyMotion.Implementation.Margin
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [MarginContainer(PredefinedMarginNames.Bottom)]
    [ContentType("any")]
    [Name(Name)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class EasyMotionMarginProvider : IWpfTextViewMarginProvider
    {
        internal const string Name = "Easy Motion Margin";

        private readonly IEasyMotionUtilProvider _easyMotionUtilProvider;

        [ImportingConstructor]
        internal EasyMotionMarginProvider(IEasyMotionUtilProvider easyMotionUtilProvider)
        {
            _easyMotionUtilProvider = easyMotionUtilProvider;
        }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer)
        {
            var easyMotionUtil = _easyMotionUtilProvider.GetEasyMotionUtil(wpfTextViewHost.TextView);
            return new EasyMotionMarginController(easyMotionUtil);
        }
    }
}
