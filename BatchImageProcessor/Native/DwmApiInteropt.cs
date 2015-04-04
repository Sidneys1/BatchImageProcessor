using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BatchImageProcessor.Native
{
    #region Structures

    /// <summary>
    /// Defines a structure containing margin dimensions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int cxLeftWidth,
            cxRightWidth,
            cyTopHeight,
            cyBottomHeight;
    }

    #endregion

    public static class DwmApiInterop
    {
        #region Constants

        public const int HTCLIENT = 0x1;
        public const int HTCAPTION = 0x2;

        public const int WM_NCHITTEST = 0x84;
        public const int WM_DWMCOMPOSITIONCHANGED = 0x31E;

        #endregion

        #region Helper methods

        /// <summary>
        /// Determines whether the cursor is on the client area.
        /// </summary>
        /// <param name="hWnd">The window handle that is receiving and processing the window message.</param>
        /// <param name="uMsg">The window message.</param>
        /// <param name="wParam">Additional message information.</param>
        /// <param name="lParam">Additional message information.</param>
        /// <returns>true if the cursor is on the client area; otherwise, false.</returns>
        public static bool IsOnClientArea(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam)
        {
	        if (uMsg != WM_NCHITTEST) return false;
	        return DefWindowProc(hWnd, uMsg, wParam, lParam).ToInt32() == HTCLIENT;
        }

        #endregion

        #region Managed wrapper methods

        /// <summary>
        /// Determines whether desktop composition is enabled on the client system.
        /// </summary>
        /// <returns>true if desktop composition is enabled; otherwise, false.</returns>
        public static bool IsCompositionEnabled()
        {
            var isEnabled = false;
            NativeMethods.DwmIsCompositionEnabled(ref isEnabled);
            return isEnabled;
        }

        /// <summary>
        /// Extends the window frame into the client area.
        /// </summary>
        /// <param name="hWnd">The window handle whose client area to extend the window frame into.</param>
        /// <param name="margins">The amount of window frame to extend around the client area.</param>
        /// <returns>S_OK on success; otherwise, an HRESULT error code.</returns>
        public static int ExtendFrameIntoClientArea(IntPtr hWnd, ref Margins margins)
        {
            return NativeMethods.DwmExtendFrameIntoClientArea(hWnd, ref margins);
        }

        /// <summary>
        /// Invokes the default window message procedure for the given window handle.
        /// </summary>
        /// <param name="hWnd">The window handle that is receiving and processing the window message.</param>
        /// <param name="uMsg">The window message.</param>
        /// <param name="wParam">Additional message information.</param>
        /// <param name="lParam">Additional message information.</param>
        /// <returns>The result of the message processing, which depends on the message.</returns>
        public static IntPtr DefWindowProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam)
        {
            return NativeMethods.DefWindowProc(hWnd, uMsg, wParam, lParam);
        }

        #endregion
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        internal static extern void DwmIsCompositionEnabled(ref bool isEnabled);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins margins);

        [DllImport("user32.dll")]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);
    }

}
