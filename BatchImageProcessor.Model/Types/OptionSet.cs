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
using BatchImageProcessor.Model.Types.Enums;

namespace BatchImageProcessor.Model.Types
{
    [Serializable]
    public class OptionSet
    {
        #region Enabled

        public bool EnableRotation { get; set; } = false;
        public bool EnableResize { get; set; } = false;
        public bool EnableCrop { get; set; } = false;

        public bool EnableMessage{ get; set; } = false;

        public bool EnableWatermark { get; set; } = false;
        public bool EnableAdjustments { get; set; } = false;

        #endregion

        public Rotation Rotation { get; set; } = Rotation.None;

        
        public MessageOptions MessageOptions { get; } = new MessageOptions();

        public WatermarkOptions WatermarkOptions { get; } = new WatermarkOptions();

        public OutputOptions OutputOptions { get; } = new OutputOptions();

        public CropOptions CropOptions { get; } = new CropOptions();

        public AdjustmentOptions AdjustmentOptions { get; } = new AdjustmentOptions();

        public ResizeOptions ResizeOptions { get; } = new ResizeOptions();
    }
}