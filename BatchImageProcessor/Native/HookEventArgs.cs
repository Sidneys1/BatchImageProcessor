using System;

namespace BatchImageProcessor.Native
{
    public class HookEventArgs : EventArgs
    {
        public int HookCode; // Hook code
        public IntPtr LParam; // LPARAM argument
        public IntPtr WParam; // WPARAM argument
    }
}