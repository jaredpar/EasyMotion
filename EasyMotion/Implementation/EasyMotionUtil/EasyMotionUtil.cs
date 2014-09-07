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
        private EasyMotionState _state;
        private char _targetChar;

        public EasyMotionState State
        {
            get { return _state; }
        }

        public char TargetChar
        {
            get { return _targetChar; }
        }

        public ITextView TextView
        {
            get { return _textView; }
        }

        public event EventHandler StateChanged;

        internal EasyMotionUtil(ITextView textView)
        {
            _textView = textView;
            ChangeToDisabled();
        }

        public void ChangeToDisabled()
        {
            _state = EasyMotionState.Disabled;
            _targetChar = (char)0;
            RaiseStateChanged();
        }

        public void ChangeToLookingForChar()
        {
            _state = EasyMotionState.LookingForChar;
            _targetChar = (char)0;
            RaiseStateChanged();
        }

        public void ChangeToLookingForDecision(char target)
        {
            _state = EasyMotionState.LookingForDecision;
            _targetChar = target;
            RaiseStateChanged();
        }

        public void ChangeToLookingCharNotFound()
        {
            _state = EasyMotionState.LookingCharNotFound;
            RaiseStateChanged();
        }

        private void RaiseStateChanged()
        {
            var list = StateChanged;
            if (list != null)
            {
                list(this, EventArgs.Empty);
            }
        }
    }
}
