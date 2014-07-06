using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace EasyMotion.Implementation.KeyProcessing
{
    [Export(typeof(IKeyProcessorProvider))]
    [Order(Before = "VisualStudioKeyProcessor")]
    [Name("Easy Motion Key Processor")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [ContentType("any")]
    internal sealed class EasyMotionKeyProcessorProvider : IKeyProcessorProvider
    {
        private readonly IEasyMotionUtilProvider _easyMotionUtilProvider;
        private readonly IEasyMotionNavigatorProvider _easyMotionNavigatorProvider;

        [ImportingConstructor]
        internal EasyMotionKeyProcessorProvider(IEasyMotionUtilProvider easyMotionUtilProvider, IEasyMotionNavigatorProvider easyMotionNavigatorProvider)
        {
            _easyMotionUtilProvider = easyMotionUtilProvider;
            _easyMotionNavigatorProvider = easyMotionNavigatorProvider;
        }

        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            var easyMotionUtil = _easyMotionUtilProvider.GetEasyMotionUtil(wpfTextView);
            var easyMotionNavigator = _easyMotionNavigatorProvider.GetEasyMotionNavigator(wpfTextView);
            return new EasyMotionKeyProcessor(easyMotionUtil, easyMotionNavigator);
        }
    }
}
