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
        LookingForDecision,

        /// <summary>
        /// Typed character not found in current text layout. 
        /// Easy motion is waiting for the character to be typed by the developer
        /// </summary>
        LookingCharNotFound
    }

    internal interface IEasyMotionUtil
    {
        ITextView TextView {get; }

        EasyMotionState State { get; }

        /// <summary>
        /// During the LookingForDecision state this will be the character/string which 
        /// the user has decided to make an easy motion for 
        /// </summary>
        string Target { get; }

        event EventHandler StateChanged;

        void ChangeToDisabled();

        void ChangeToLookingForChar();

        void ChangeToLookingForDecision(string target);

        void ChangeToLookingCharNotFound();
    }

    internal interface IEasyMotionUtilProvider
    {
        IEasyMotionUtil GetEasyMotionUtil(ITextView textView);
    }

}
