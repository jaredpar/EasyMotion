using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;

namespace EasyMotion
{
    internal interface IEasyMotionNavigator
    {
        bool NavigateTo(string key);
    }

    internal interface IEasyMotionNavigatorProvider
    {
        IEasyMotionNavigator GetEasyMotionNavigator(IWpfTextView wpfTextView);
    }
}
