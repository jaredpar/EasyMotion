using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion.Implementation.KeyProcessing
{
    internal sealed class EasyMotionKeyProcessor : KeyProcessor
    {
        private readonly IEasyMotionUtil _easyMotionUtil;
        private readonly IEasyMotionNavigator _easyMotionNavigator;
        private System.Text.StringBuilder _easyMotionLettersAccumulator = new System.Text.StringBuilder();

        internal EasyMotionKeyProcessor(IEasyMotionUtil easyMotionUtil, IEasyMotionNavigator easyMotionNavigator)
        {
            _easyMotionUtil = easyMotionUtil;
            _easyMotionNavigator = easyMotionNavigator;
        }

        public override void TextInput(TextCompositionEventArgs args)
        {
            base.TextInput(args);

            switch (_easyMotionUtil.State)
            {
                case EasyMotionState.Disabled:
                    // Nothing to do here 
                    break;
                case EasyMotionState.LookingForChar:
                case EasyMotionState.LookingCharNotFound:
                    TextInputLookingForChar(args);
                    break;
                case EasyMotionState.LookingForDecision:
                    TextInputLookingForDecision(args);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public override void KeyUp(KeyEventArgs args)
        {
            base.KeyUp(args);

            if (args.Key == Key.Escape && _easyMotionUtil.State != EasyMotionState.Disabled)
            {
                //_userInput = string.Empty;
                _easyMotionUtil.ChangeToDisabled();
            }
        }

        private void TextInputLookingForChar(TextCompositionEventArgs args)
        {
            if (args.Text.Length == 1)
            {
                _easyMotionUtil.ChangeToLookingForDecision(args.Text[0]);
                _easyMotionLettersAccumulator.Clear();
                args.Handled = true;
            }
        }

        private void TextInputLookingForDecision(TextCompositionEventArgs args)
        {
            if (args.Text.Length > 0)
            {
                _easyMotionLettersAccumulator.Append(args.Text);
                if (_easyMotionNavigator.NavigateTo(_easyMotionLettersAccumulator.ToString()))
                {
                    _easyMotionUtil.ChangeToDisabled();
                }
                else
                {
                    SystemSounds.Beep.Play();
                }

                args.Handled = true;
            }
        }
    }
}
