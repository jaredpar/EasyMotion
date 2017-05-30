﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows;

namespace EasyMotion.Implementation.Adornment {
    internal sealed class EasyMotionAdornmentController : IEasyMotionNavigator {
        private static readonly string[] NavigationKeys =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
            .Select (x => x.ToString ())
            .ToArray ();

        private readonly IEasyMotionUtil _easyMotionUtil;
        private readonly IWpfTextView _wpfTextView;
        private readonly IEditorFormatMap _editorFormatMap;
        private readonly IClassificationFormatMap _classificationFormatMap;
        private readonly Dictionary<string, SnapshotSpan> _navigateMap = new Dictionary<string, SnapshotSpan>();
        private readonly object _tag = new object ();
        private IAdornmentLayer _adornmentLayer;

        internal EasyMotionAdornmentController(IEasyMotionUtil easyMotionUtil, IWpfTextView wpfTextview, IEditorFormatMap editorFormatMap, IClassificationFormatMap classificationFormatMap)
        {
            _easyMotionUtil = easyMotionUtil;
            _wpfTextView = wpfTextview;
            _editorFormatMap = editorFormatMap;
            _classificationFormatMap = classificationFormatMap;
        }

        internal void SetAdornmentLayer(IAdornmentLayer adornmentLayer)
        {
            Debug.Assert (_adornmentLayer == null);
            _adornmentLayer = adornmentLayer;
            Subscribe ();
        }

        private void Subscribe()
        {
            _easyMotionUtil.StateChanged += OnStateChanged;
            _wpfTextView.LayoutChanged += OnLayoutChanged;
        }

        private void Unsubscribe()
        {
            _easyMotionUtil.StateChanged -= OnStateChanged;
            _wpfTextView.LayoutChanged -= OnLayoutChanged;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (_easyMotionUtil.State == EasyMotionState.LookingForDecision)
            {
                AddAdornments ();
            }
            else
            {
                _adornmentLayer.RemoveAdornmentsByTag (_tag);
            }
        }

        private void OnLayoutChanged(object sender, EventArgs e)
        {
            switch (_easyMotionUtil.State)
            {
                case EasyMotionState.LookingCharNotFound:
                    _easyMotionUtil.ChangeToLookingForDecision(_easyMotionUtil.Target);
                    break;

                case EasyMotionState.LookingForDecision:
                    ResetAdornments ();
                    break;
            }
        }

        private void ResetAdornments () {
            _adornmentLayer.RemoveAdornmentsByTag (_tag);
            AddAdornments ();
        }

        private void AddAdornments () {
            Debug.Assert (_easyMotionUtil.State == EasyMotionState.LookingForDecision);

            if (_wpfTextView.InLayout) {
                return;
            }

            _navigateMap.Clear ();
            var textViewLines = _wpfTextView.TextViewLines;
            var startPoint = textViewLines.FirstVisibleLine.Start;
            var endPoint = textViewLines.LastVisibleLine.End;
            var snapshot = startPoint.Snapshot;
            int navigateIndex = 0;
            var ignoreCase = _easyMotionUtil.Target.ToLowerInvariant () == _easyMotionUtil.Target; //smartcase
            var target = ignoreCase ? _easyMotionUtil.Target.ToLowerInvariant () : _easyMotionUtil.Target;
            for (int i = startPoint.Position; i < endPoint.Position; i++) {
                var span = new SnapshotSpan (snapshot, i, _easyMotionUtil.Target.Length);

                if ((ignoreCase ? span.GetText ().ToLowerInvariant () : span.GetText ()) == target && navigateIndex < NavigationKeys.Length) {
                    string key = NavigationKeys[navigateIndex];
                    navigateIndex++;
                    AddNavigateToPoint (textViewLines, span, key);
                }
                if (navigateIndex < NavigationKeys.Length == false) break;//don't search further, as I won't use them anyway
            }

            if (navigateIndex == 0) {
                _easyMotionUtil.ChangeToLookingCharNotFound ();
            }
        }

        private void AddNavigateToPoint (IWpfTextViewLineCollection textViewLines, SnapshotSpan span, string key) {
            _navigateMap[key] = span;

            var resourceDictionary = _editorFormatMap.GetProperties (EasyMotionNavigateFormatDefinition.Name);

            var bounds = textViewLines.GetCharacterBounds (span.Start);

            var textBox = new TextBox ();
            textBox.Text = key;
            textBox.FontFamily = _classificationFormatMap.DefaultTextProperties.Typeface.FontFamily;
            textBox.Foreground = resourceDictionary.GetForegroundBrush (EasyMotionNavigateFormatDefinition.DefaultForegroundBrush);
            textBox.Background = resourceDictionary.GetBackgroundBrush (EasyMotionNavigateFormatDefinition.DefaultBackgroundBrush);
            textBox.Measure (new Size (double.PositiveInfinity, double.PositiveInfinity));

            Canvas.SetTop (textBox, bounds.TextTop);
            Canvas.SetLeft (textBox, bounds.Left);
            Canvas.SetZIndex (textBox, 10);

            _adornmentLayer.AddAdornment (span, _tag, textBox);
        }

        public bool NavigateTo (string key) {
            if (!_navigateMap.TryGetValue (key, out SnapshotSpan span)) {
                return false;
            }

            if (span.Snapshot != _wpfTextView.TextSnapshot) {
                return false;
            }

            _wpfTextView.Caret.MoveTo (span.Start);
            System.Windows.Forms.SendKeys.Send ("{ESC}");// send ESC for VsVim; untested in vanilla VS, but I hope it will be NOP
            return true;
        }
    }
}
