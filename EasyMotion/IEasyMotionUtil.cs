using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion
{
    internal enum EasyMotionSearchMode
    {
        /// <summary>
        /// Developer is looking for characters anywhere
        /// </summary>
        Char,

        /// <summary>
        /// Developer is looking for words starting with the typed character
        /// </summary>
        Word,

        /// <summary>
        /// Same as Char + extending the selection
        /// </summary>
       CharExtend,

       /// <summary>
       /// Same as Word + extending the selection
       /// </summary>
       WordExtend
    }



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

        EasyMotionSearchMode SearchMode { get; }

        /// <summary>
        /// During the LookingForDecision state this will be the character which 
        /// the user has decided to make an easy motion for 
        /// </summary>
        char TargetChar { get; }

        event EventHandler StateChanged;

        void ChangeToDisabled();

        void ChangeToLookingForChar(EasyMotionSearchMode searchMode);

        void ChangeToLookingForDecision(char target);

        void ChangeToLookingCharNotFound();

        bool IsInWordMode { get; }
    }

    internal interface IEasyMotionUtilProvider
    {
        IEasyMotionUtil GetEasyMotionUtil(ITextView textView);
    }

}
