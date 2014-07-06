using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion.Implementation.EasyMotionUtil
{
    internal sealed class EasyMotionUtil : IEasyMotionUtil
    {
        private readonly ITextView _textView;
        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; } 
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    RaiseEnabledChanged();
                }
            }
        }

        public ITextView TextView
        {
            get { return _textView; }
        }

        public event EventHandler EnabledChanged;

        internal EasyMotionUtil(ITextView textView)
        {
            _textView = textView;

            // HACK: should be disabled by default.  Enabled by default only for testing
            _enabled = true;
        }

        private void RaiseEnabledChanged()
        {
            var list = EnabledChanged;
            if (list != null)
            {
                list(this, EventArgs.Empty);
            }
        }
    }
}
