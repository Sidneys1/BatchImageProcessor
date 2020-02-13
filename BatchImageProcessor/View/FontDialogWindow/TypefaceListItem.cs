// Copyright (c) 2017 Sidneys1
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
using System;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace FontDialogWindow
{
    internal class TypefaceListItem : TextBlock, IComparable
    {
        private string _displayName;
        private bool _simulated;

        public TypefaceListItem(Typeface typeface)
        {
            _displayName = GetDisplayName(typeface);
            _simulated = typeface.IsBoldSimulated || typeface.IsObliqueSimulated;

            this.FontFamily = typeface.FontFamily;
            this.FontWeight = typeface.Weight;
            this.FontStyle = typeface.Style;
            this.FontStretch = typeface.Stretch;

            string itemLabel = _displayName;

            if (_simulated)
            {
                /*
                string formatString = FontDialogWindow.Properties.Resources.ResourceManager.GetString(
                    "simulated", 
                    CultureInfo.CurrentUICulture
                    );
                    */
                //TODO: resource 
                string formatString = "simulated";
                itemLabel = string.Format(formatString, itemLabel);
            }

            this.Text = itemLabel;
            this.ToolTip = itemLabel;

            // In the case of symbol font, apply the default message font to the text so it can be read.
            if (FontFamilyListItem.IsSymbolFont(typeface.FontFamily))
            {
                TextRange range = new TextRange(this.ContentStart, this.ContentEnd);
                range.ApplyPropertyValue(TextBlock.FontFamilyProperty, SystemFonts.MessageFontFamily);
            }
        }

        public override string ToString()
        {
            return _displayName;
        }

        public Typeface Typeface
        {
            get { return new Typeface(FontFamily, FontStyle, FontWeight, FontStretch); }
        }

        int IComparable.CompareTo(object obj)
        {
            TypefaceListItem item = obj as TypefaceListItem;
            if (item == null)
            {
                return -1;
            }

            // Sort all simulated faces after all non-simulated faces.
            if (_simulated != item._simulated)
            {
                return _simulated ? 1 : -1;
            }

            // If weight differs then sort based on weight (lightest first).
            int difference = FontWeight.ToOpenTypeWeight() - item.FontWeight.ToOpenTypeWeight();
            if (difference != 0)
            {
                return difference > 0 ? 1 : -1;
            }

            // If style differs then sort based on style (Normal, Italic, then Oblique).
            FontStyle thisStyle = FontStyle;
            FontStyle otherStyle = item.FontStyle;

            if (thisStyle != otherStyle)
            {
                if (thisStyle == FontStyles.Normal)
                {
                    // This item is normal style and should come first.
                    return -1;
                }
                else if (otherStyle == FontStyles.Normal)
                {
                    // The other item is normal style and should come first.
                    return 1;
                }
                else
                {
                    // Neither is normal so sort italic before oblique.
                    return (thisStyle == FontStyles.Italic) ? -1 : 1;
                }
            }

            // If stretch differs then sort based on stretch (Normal first, then numerically).
            FontStretch thisStretch = FontStretch;
            FontStretch otherStretch = item.FontStretch;

            if (thisStretch != otherStretch)
            {
                if (thisStretch == FontStretches.Normal)
                {
                    // This item is normal stretch and should come first.
                    return -1;
                }
                else if (otherStretch == FontStretches.Normal)
                {
                    // The other item is normal stretch and should come first.
                    return 1;
                }
                else
                {
                    // Neither is normal so sort numerically.
                    return thisStretch.ToOpenTypeStretch() < otherStretch.ToOpenTypeStretch() ? -1 : 0;
                }
            }

            // They're the same.
            return 0;
        }

        internal static string GetDisplayName(Typeface typeface)
        {
            return NameDictionaryHelper.GetDisplayName(typeface.FaceNames);
        }
    }
}
