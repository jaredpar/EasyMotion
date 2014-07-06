using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion
{
    internal enum EasyMotionState
    {
        /// <summary>
        /// Easy motion is currently not contributing to the UI 
        /// </summary>
        Disabled,

        /// <summary>
        /// Easy motion is waiting for the character to be typed by the developer 
        /// </summary>
        LookingForChar,

        /// <summary>
        /// Easy motion is waiting for the navigation decision
        /// </summary>
        LookingForDecision
    }

    internal interface IEasyMotionUtil
    {
        ITextView TextView {get; }

        EasyMotionState State { get; }

        /// <summary>
        /// During the LookingForDecision state this will be the character which 
        /// the user has decided to make an easy motion for 
        /// </summary>
        char TargetChar { get; }

        event EventHandler StateChanged;

        void ChangeToDisabled();

        void ChangeToLookingForChar();

        void ChangeToLookingForDecision(char target);
    }

    internal interface IEasyMotionUtilProvider
    {
        IEasyMotionUtil GetEasyMotionUtil(ITextView textView);
    }

}
