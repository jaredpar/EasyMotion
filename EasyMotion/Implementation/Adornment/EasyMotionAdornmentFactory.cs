using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace EasyMotion.Implementation.Adornment
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [Export(typeof(IEasyMotionNavigatorProvider))]
    [ContentType("any")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [TagType(typeof(IntraTextAdornmentTag))]
    internal sealed class EasyMotionAdornmentFactory : IWpfTextViewCreationListener, IEasyMotionNavigatorProvider
    {
        private static readonly object Key = new object();
        private const string AdornmentLayerName = "Easy Motion Adornment Layer";

        private readonly IEasyMotionUtilProvider _easyMotionUtilProvider;

#pragma warning disable 169
        [Export(typeof(AdornmentLayerDefinition))]
        [Name(AdornmentLayerName)]
        [Order(After = PredefinedAdornmentLayers.Selection)]
        private AdornmentLayerDefinition _easyMotionAdornmentLayerDefinition;
#pragma warning restore 169

        [ImportingConstructor]
        internal EasyMotionAdornmentFactory(IEasyMotionUtilProvider easyMotionUtilProvider)
        {
            _easyMotionUtilProvider = easyMotionUtilProvider;
        }

        private EasyMotionAdornmentController GetOrCreate(IWpfTextView wpfTextView)
        {
            return wpfTextView.Properties.GetOrCreateSingletonProperty(
                Key,
                () =>
                {
                    var easyMotionUtil = _easyMotionUtilProvider.GetEasyMotionUtil(wpfTextView);
                    return new EasyMotionAdornmentController(easyMotionUtil, wpfTextView);
                });
        }

        public void TextViewCreated(IWpfTextView wpfTextView)
        {
            var easyMotionAdornmentController = GetOrCreate(wpfTextView);
            var adornmentLayer = wpfTextView.GetAdornmentLayer(AdornmentLayerName);
            easyMotionAdornmentController.SetAdornmentLayer(adornmentLayer);
        }

        public IEasyMotionNavigator GetEasyMotionNavigator(IWpfTextView wpfTextView)
        {
            return GetOrCreate(wpfTextView);
        }
    }
}
