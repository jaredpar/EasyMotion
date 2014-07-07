using System;
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

namespace EasyMotion.Implementation.Adornment
{
    internal sealed class EasyMotionAdornmentController : IEasyMotionNavigator
    {
        private const string CharLettersUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private readonly IEasyMotionUtil _easyMotionUtil;
        private readonly IWpfTextView _wpfTextView;
        private readonly IClassificationFormatMap _classificationFormatMap;
        private readonly Dictionary<string, SnapshotPoint> _navigateMap = new Dictionary<string, SnapshotPoint>();
        private readonly object _tag = new object();
        private IAdornmentLayer _adornmentLayer;

        internal EasyMotionAdornmentController(IEasyMotionUtil easyMotionUtil, IWpfTextView wpfTextview, IClassificationFormatMap classificationFormatMap)
        {
            _easyMotionUtil = easyMotionUtil;
            _wpfTextView = wpfTextview;
            _classificationFormatMap = classificationFormatMap;
        }

        internal void SetAdornmentLayer(IAdornmentLayer adornmentLayer)
        {
            Debug.Assert(_adornmentLayer == null);
            _adornmentLayer = adornmentLayer;
            Subscribe();
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
                AddAdornments();
            }
            else
            {
                _adornmentLayer.RemoveAdornmentsByTag(_tag);
            }
        }

        private void OnLayoutChanged(object sender, EventArgs e)
        {
            if (_easyMotionUtil.State == EasyMotionState.LookingForDecision)
            {
                ResetAdornments();
            }
        }

        private void ResetAdornments()
        {
            _adornmentLayer.RemoveAdornmentsByTag(_tag);
            AddAdornments();
        }

        private void AddAdornments()
        {
            Debug.Assert(_easyMotionUtil.State == EasyMotionState.LookingForDecision);

            if (_wpfTextView.InLayout)
            {
                return;
            }

            _navigateMap.Clear();
            var textViewLines = _wpfTextView.TextViewLines;
            var startPoint = textViewLines.FirstVisibleLine.Start;
            var endPoint = textViewLines.LastVisibleLine.End;
            var snapshot = startPoint.Snapshot;
            int navigateIndex = 0;
            for (int i = startPoint.Position; i < endPoint.Position; i++)
            {
                var point = new SnapshotPoint(snapshot, i);

                if (point.GetChar() == _easyMotionUtil.TargetChar && navigateIndex < CharLettersUpper.Length)
                {
                    string key = CharLettersUpper[navigateIndex].ToString();
                    navigateIndex++;
                    AddNavigateToPoint(textViewLines, point, key);
                }
            }
        }

        private void AddNavigateToPoint(IWpfTextViewLineCollection textViewLines, SnapshotPoint point, string key)
        {
            _navigateMap[key] = point;

            var textBlock = new TextBlock();
            textBlock.Text = key;
            textBlock.FontFamily = _classificationFormatMap.DefaultTextProperties.Typeface.FontFamily;
            textBlock.Background = Brushes.LightYellow;

            var span = new SnapshotSpan(point, 1);
            var bounds = textViewLines.GetMarkerGeometry(span).Bounds;
            Canvas.SetTop(textBlock, bounds.Top);
            Canvas.SetLeft(textBlock, bounds.Left);

            _adornmentLayer.AddAdornment(span, _tag, textBlock);
        }

        public bool NavigateTo(string key)
        {
            SnapshotPoint point;
            if (!_navigateMap.TryGetValue(key, out point))
            {
                return false;
            }

            if (point.Snapshot != _wpfTextView.TextSnapshot)
            {
                return false;
            }

            _wpfTextView.Caret.MoveTo(point);
            return true;
        }
    }
}
