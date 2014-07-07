using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion.Implementation.Margin
{
    internal sealed class EasyMotionMarginController : IWpfTextViewMargin
    {
        private readonly IEasyMotionUtil _easyMotionUtil;
        private readonly EasyMotionMargin _control;

        internal EasyMotionMarginController(IEasyMotionUtil easyMotionUtil)
        {
            _easyMotionUtil = easyMotionUtil;
            _easyMotionUtil.StateChanged += OnStateChanged;
            _control = new EasyMotionMargin();
        }

        private void Unsubscribe()
        {
            _easyMotionUtil.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            switch (_easyMotionUtil.State)
            {
                case EasyMotionState.Disabled:
                    _control.Visibility = Visibility.Collapsed;
                    break;
                case EasyMotionState.LookingForChar:
                    _control.Visibility = Visibility.Visible;
                    _control.StatusLine = "Type the character you want to search for";
                    break;
                case EasyMotionState.LookingForDecision:
                    _control.Visibility = Visibility.Visible;
                    _control.StatusLine = "Type the character at the location you want to jump to";
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public bool Enabled  
        {
            get { return _easyMotionUtil.State != EasyMotionState.Disabled; }
        }

        public double MarginSize
        {
            get { return 25; }
        }

        public FrameworkElement VisualElement
        {
            get { return _control; }
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return EasyMotionMarginProvider.Name == marginName ? this : null;
        }
    }
}
