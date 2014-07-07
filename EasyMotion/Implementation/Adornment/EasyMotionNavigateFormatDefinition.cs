using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EasyMotion.Implementation.Adornment
{
    [Export(typeof(EditorFormatDefinition))]
    [Name(Name)]
    [UserVisible(true)]
    internal sealed class EasyMotionNavigateFormatDefinition : EditorFormatDefinition
    {
        internal const string Name = "easymotion_navigate";

        internal static readonly Color DefaultForegroundColor = Colors.Black;
        internal static readonly Brush DefaultForegroundBrush = new SolidColorBrush(DefaultForegroundColor);
        internal static readonly Color DefaultBackgroundColor = Colors.LightYellow;
        internal static readonly Brush DefaultBackgroundBrush = new SolidColorBrush(DefaultBackgroundColor);

        internal EasyMotionNavigateFormatDefinition()
        {
            DisplayName = "EasyMotion Navigate Characters";
            ForegroundColor = DefaultForegroundColor;
            BackgroundColor = DefaultBackgroundColor;
        }
    }
}
