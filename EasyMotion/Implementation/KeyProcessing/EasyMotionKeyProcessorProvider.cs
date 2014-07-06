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

        [ImportingConstructor]
        internal EasyMotionKeyProcessorProvider(IEasyMotionUtilProvider easyMotionUtilProvider)
        {
            _easyMotionUtilProvider = easyMotionUtilProvider;
        }

        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            var easyMotionUtil = _easyMotionUtilProvider.GetEasyMotionUtil(wpfTextView);
            return new EasyMotionKeyProcessor(easyMotionUtil);
        }
    }
}
