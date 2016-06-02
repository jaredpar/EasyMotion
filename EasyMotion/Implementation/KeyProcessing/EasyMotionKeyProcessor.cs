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
        private string _userInput = string.Empty;

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
                _userInput = string.Empty;
                _easyMotionUtil.ChangeToDisabled();
            }
        }

        private void TextInputLookingForChar(TextCompositionEventArgs args)
        {
            if (args.Text.Length == 1)
            {
                _easyMotionUtil.ChangeToLookingForDecision(args.Text[0]);
                args.Handled = true;
            }
        }

        private void TextInputLookingForDecision(TextCompositionEventArgs args)
        {
            if (args.Text.Length > 0)
            {
                _userInput = _userInput + args.Text;

                if (_userInput.Length == 2)
                {
                    if (_easyMotionNavigator.NavigateTo(_userInput))
                    {
                        _easyMotionUtil.ChangeToDisabled();
                    }
                    else
                    {
                        SystemSounds.Beep.Play();
                    }

                    _userInput = string.Empty;
                }
                else if(_userInput.Length > 2)
                {
                    _userInput = string.Empty;
                }

                args.Handled = true;
            }
        }
    }
}
