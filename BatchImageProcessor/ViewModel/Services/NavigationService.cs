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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BatchImageProcessor.ViewModel.Services
{
	public static class NavigationService
	{
		// Attribution: http://geekswithblogs.net/casualjim/archive/2005/12/01/61722.aspx
		private static readonly Regex ReUrl = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");

		public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
			"Text",
			typeof(string),
			typeof(NavigationService),
			new PropertyMetadata(null, OnTextChanged)
		);

		public static string GetText(DependencyObject d)
		{ return d.GetValue(TextProperty) as string; }

		public static void SetText(DependencyObject d, string value)
		{ d.SetValue(TextProperty, value); }

		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var textBlock = d as TextBlock;
			if (textBlock == null)
				return;

			textBlock.Inlines.Clear();

			var newText = (string)e.NewValue;
			if (string.IsNullOrEmpty(newText))
				return;

			// Find all URLs using a regular expression
			var lastPos = 0;
			foreach (Match match in ReUrl.Matches(newText))
			{
				// Copy raw string from the last position up to the match
				if (match.Index != lastPos)
				{
					var rawText = newText.Substring(lastPos, match.Index - lastPos);
					textBlock.Inlines.Add(new Run(rawText));
				}

				// Create a hyperlink for the match
				var link = new Hyperlink(new Run(match.Value)) {
					NavigateUri = new Uri(match.Value)
				};

				link.Click += OnUrlClick;

				textBlock.Inlines.Add(link);

				// Update the last matched position
				lastPos = match.Index + match.Length;
			}

			// Finally, copy the remainder of the string
			if (lastPos < newText.Length)
				textBlock.Inlines.Add(new Run(newText.Substring(lastPos)));
		}

		private static void OnUrlClick(object sender, RoutedEventArgs e)
		{
			var link = (Hyperlink)sender;
			// Do something with link.NavigateUri like:
			Process.Start(link.NavigateUri.ToString());
		}
	}

}
