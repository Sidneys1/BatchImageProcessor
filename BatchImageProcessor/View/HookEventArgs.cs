using System;

namespace BatchImageProcessor.View
{
    public class HookEventArgs : EventArgs
    {
        public int HookCode; // Hook code
        public IntPtr LParam; // LPARAM argument
        public IntPtr WParam; // WPARAM argument
    }
}