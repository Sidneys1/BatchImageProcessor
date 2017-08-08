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
namespace BatchImageProcessor.Native
{
    public enum HookType
    {
        WhJournalRecord = 0,
        WhJournalplayback = 1,
        WhKeyboard = 2,
        WhGetmessage = 3,
        WhCallwndproc = 4,
        WhCbt = 5,
        WhSysmsgfilter = 6,
        WhMouse = 7,
        WhHardware = 8,
        WhDebug = 9,
        WhShell = 10,
        WhForegroundidle = 11,
        WhCallwndprocret = 12,
        WhKeyboardLl = 13,
        WhMouseLl = 14
    }
}