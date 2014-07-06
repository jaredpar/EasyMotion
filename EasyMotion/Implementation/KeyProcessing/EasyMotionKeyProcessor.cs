using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion.Implementation.KeyProcessing
{
    internal sealed class EasyMotionKeyProcessor : KeyProcessor
    {
        private readonly IEasyMotionUtil _easyMotionUtil;

        internal EasyMotionKeyProcessor(IEasyMotionUtil easyMotionUtil)
        {
            _easyMotionUtil = easyMotionUtil;
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
                    if (args.Text.Length == 1)
                    {
                        _easyMotionUtil.ChangeToLookingForDecision(args.Text[0]);
                        args.Handled = true;
                    }

                    break;
                case EasyMotionState.LookingForDecision:
                    // TODO: Actually do the navigation
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }
    }
}
