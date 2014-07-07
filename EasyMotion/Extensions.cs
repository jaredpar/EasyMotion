using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace EasyMotion
{
    internal static class Extensions
    {
        #region ResourceDictionary

        internal static Brush GetForegroundBrush(this ResourceDictionary dictionary, Brush defaultBrush)
        {
            return GetBrush(dictionary, EditorFormatDefinition.ForegroundBrushId, EditorFormatDefinition.ForegroundColorId, defaultBrush);
        }

        internal static Brush GetBackgroundBrush(this ResourceDictionary dictionary, Brush defaultBrush)
        {
            return GetBrush(dictionary, EditorFormatDefinition.BackgroundBrushId, EditorFormatDefinition.BackgroundColorId, defaultBrush);
        }

        internal static Brush GetBrush(this ResourceDictionary dictionary, string brushName, string colorName, Brush defaultBrush)
        {
            if (dictionary == null)
            {
                return defaultBrush;
            }

            var obj = dictionary[brushName];
            if (obj is Brush)
            {
                return (Brush)obj;
            }

            obj = dictionary[colorName];
            if (obj is Color?)
            {
                var color = (Color?)obj;
                if (color.HasValue)
                {
                    var brush = new SolidColorBrush(color.Value);
                    brush.Freeze();
                    return brush;
                }
            }

            return defaultBrush;
        }

        #endregion
    }
}
