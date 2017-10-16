﻿using Microsoft.VisualStudio.Text.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
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
        private readonly IEditorFormatMapService _editorFormatMapService;
        private readonly IClassificationFormatMapService _classificationFormatMapService;
        private readonly ITextSearchService _textSerachService;

#pragma warning disable 169
        [Export(typeof(AdornmentLayerDefinition))]
        [Name(AdornmentLayerName)]
        [Order(After = PredefinedAdornmentLayers.Text)]
        private AdornmentLayerDefinition _easyMotionAdornmentLayerDefinition;
#pragma warning restore 169

        [ImportingConstructor]
        internal EasyMotionAdornmentFactory(IEasyMotionUtilProvider easyMotionUtilProvider, IEditorFormatMapService editorFormatMapService
            , IClassificationFormatMapService classificationFormatMapService, ITextSearchService textSearchService)
        {
            _easyMotionUtilProvider = easyMotionUtilProvider;
            _editorFormatMapService = editorFormatMapService;
            _classificationFormatMapService = classificationFormatMapService;
            _textSerachService = textSearchService;
        }

        private EasyMotionAdornmentController GetOrCreate(IWpfTextView wpfTextView)
        {
            return wpfTextView.Properties.GetOrCreateSingletonProperty(
                Key,
                () =>
                {
                    var easyMotionUtil = _easyMotionUtilProvider.GetEasyMotionUtil(wpfTextView);
                    var editorFormatMap = _editorFormatMapService.GetEditorFormatMap(wpfTextView);
                    var classificationFormatMap = _classificationFormatMapService.GetClassificationFormatMap(wpfTextView);
                    return new EasyMotionAdornmentController(easyMotionUtil, wpfTextView, editorFormatMap, classificationFormatMap, _textSerachService);
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
