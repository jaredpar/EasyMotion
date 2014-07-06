using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion.Implementation.Adornment
{
    internal sealed class EasyMotionAdornmentController
    {
        private readonly IEasyMotionUtil _easyMotionUtil;
        private readonly IWpfTextView _wpfTextView;
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly object _tag = new object();

        internal EasyMotionAdornmentController(IEasyMotionUtil easyMotionUtil, IWpfTextView wpfTextview, IAdornmentLayer adornmentLayer)
        {
            _easyMotionUtil = easyMotionUtil;
            _wpfTextView = wpfTextview;
            _adornmentLayer = adornmentLayer;

            _easyMotionUtil.EnabledChanged += OnEnabledChanged;
            _wpfTextView.LayoutChanged += OnLayoutChanged;
        }

        private void Unsubscribe()
        {
            _easyMotionUtil.EnabledChanged -= OnEnabledChanged;
            _wpfTextView.LayoutChanged -= OnLayoutChanged;
        }

        private void OnEnabledChanged(object sender, EventArgs e)
        {
            if (_easyMotionUtil.Enabled)
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
            if (_easyMotionUtil.Enabled)
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
            if (_wpfTextView.InLayout)
            {
                return;
            }

            var textViewLines = _wpfTextView.TextViewLines;
            var startPoint = textViewLines.FirstVisibleLine.Start;
            var endPoint = textViewLines.LastVisibleLine.End;
            var snapshot = startPoint.Snapshot;
            for (int i = startPoint.Position; i < endPoint.Position; i++)
            {
                var point = new SnapshotPoint(snapshot, i);

                // HACK: for now just hard code 'a'.  Add a real configuration later
                if (point.GetChar() == 'a')
                {
                    var textBlock = new TextBlock();
                    textBlock.Text = point.GetChar().ToString();
                    textBlock.Background = Brushes.LightYellow;

                    var span = new SnapshotSpan(point, 1);
                    var bounds = textViewLines.GetMarkerGeometry(span).Bounds;
                    Canvas.SetTop(textBlock, bounds.Top);
                    Canvas.SetLeft(textBlock, bounds.Left);

                    _adornmentLayer.AddAdornment(span, _tag, textBlock);
                }
            }
        }
    }
}
