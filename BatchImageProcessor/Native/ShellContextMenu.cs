using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BatchImageProcessor.Native
{
    /// <summary>
    ///     "Stand-alone" shell context menu
    ///     It isn't really debugged but is mostly working.
    ///     Create an instance and call ShowContextMenu with a list of FileInfo for the files.
    ///     Limitation is that it only handles files in the same directory but it can be fixed
    ///     by changing the way files are translated into PIDLs.
    ///     Based on FileBrowser in C# from CodeProject
    ///     http://www.codeproject.com/useritems/FileBrowser.asp
    ///     Hooking class taken from MSDN Magazine Cutting Edge column
    ///     http://msdn.microsoft.com/msdnmag/issues/02/10/CuttingEdge/
    ///     Andreas Johansson
    ///     afjohansson@hotmail.com
    ///     http://afjohansson.spaces.live.com
    /// </summary>
    /// <example>
    ///     ShellContextMenu scm = new ShellContextMenu();
    ///     FileInfo[] files = new FileInfo[1];
    ///     files[0] = new FileInfo(@"c:\windows\notepad.exe");
    ///     scm.ShowContextMenu(this.Handle, files, Cursor.Position);
    /// </example>
    public class ShellContextMenu
    {
        #region Destructor

        /// <summary>Ensure all resources get released</summary>
        ~ShellContextMenu()
        {
            ReleaseAll();
        }

        #endregion

        #region GetContextMenuInterfaces()

        /// <summary>Gets the interfaces to the context menu</summary>
        /// <param name="oParentFolder">Parent folder</param>
        /// <param name="arrPidLs">PIDLs</param>
        /// <returns>true if it got the interfaces, otherwise false</returns>
        private bool GetContextMenuInterfaces(IShellFolder oParentFolder, IntPtr[] arrPidLs)
        {
            IntPtr pUnknownContextMenu;

            var nResult = oParentFolder.GetUIObjectOf(
                IntPtr.Zero,
                (uint) arrPidLs.Length,
                arrPidLs,
                ref _iidIContextMenu,
                IntPtr.Zero,
                out pUnknownContextMenu);

	        if (S_OK != nResult) return false;
	        _oContextMenu =
		        (IContextMenu) Marshal.GetTypedObjectForIUnknown(pUnknownContextMenu, typeof (IContextMenu));

	        IntPtr pUnknownContextMenu2;
	        if (S_OK == Marshal.QueryInterface(pUnknownContextMenu, ref _iidIContextMenu2, out pUnknownContextMenu2))
	        {
		        _oContextMenu2 =
			        (IContextMenu2) Marshal.GetTypedObjectForIUnknown(pUnknownContextMenu2, typeof (IContextMenu2));
	        }
	        IntPtr pUnknownContextMenu3;
	        if (S_OK == Marshal.QueryInterface(pUnknownContextMenu, ref _iidIContextMenu3, out pUnknownContextMenu3))
	        {
		        _oContextMenu3 =
			        (IContextMenu3) Marshal.GetTypedObjectForIUnknown(pUnknownContextMenu3, typeof (IContextMenu3));
	        }

	        return true;
        }

        #endregion

        #region InvokeCommand

        private static void InvokeCommand(IContextMenu oContextMenu, uint nCmd, string strFolder,
            System.Drawing.Point pointInvoke)
        {
            var invoke = new CmInvokeCommandInfoEx
            {
                cbSize = CbInvokeCommand,
                lpVerb = (IntPtr) (nCmd - CMD_FIRST),
                lpDirectory = strFolder,
                lpVerbW = (IntPtr) (nCmd - CMD_FIRST),
                lpDirectoryW = strFolder,
                fMask = Cmic.Unicode | Cmic.Ptinvoke |
                        ((Control.ModifierKeys & Keys.Control) != 0 ? Cmic.ControlDown : 0) |
                        ((Control.ModifierKeys & Keys.Shift) != 0 ? Cmic.ShiftDown : 0),
                ptInvoke = new Point(pointInvoke.X, pointInvoke.Y),
                nShow = Sw.ShowNormal
            };

            oContextMenu.InvokeCommand(ref invoke);
        }

        #endregion

        #region ReleaseAll()

        /// <summary>
        ///     Release all allocated interfaces, PIDLs
        /// </summary>
        private void ReleaseAll()
        {
            if (null != _oContextMenu)
            {
                Marshal.ReleaseComObject(_oContextMenu);
                _oContextMenu = null;
            }
            if (null != _oContextMenu2)
            {
                Marshal.ReleaseComObject(_oContextMenu2);
                _oContextMenu2 = null;
            }
            if (null != _oContextMenu3)
            {
                Marshal.ReleaseComObject(_oContextMenu3);
                _oContextMenu3 = null;
            }
            if (null != _oDesktopFolder)
            {
                Marshal.ReleaseComObject(_oDesktopFolder);
                _oDesktopFolder = null;
            }
            if (null != _oParentFolder)
            {
                Marshal.ReleaseComObject(_oParentFolder);
                _oParentFolder = null;
            }
	        if (null == _arrPidLs) return;
	        FreePidLs(_arrPidLs);
	        _arrPidLs = null;
        }

        #endregion

        #region GetDesktopFolder()

        /// <summary>
        ///     Gets the desktop folder
        /// </summary>
        /// <returns>IShellFolder for desktop folder</returns>
        private IShellFolder GetDesktopFolder()
        {
	        if (null != _oDesktopFolder) return _oDesktopFolder;
	        // Get desktop IShellFolder
	        IntPtr pUnkownDesktopFolder;
	        var nResult = SHGetDesktopFolder(out pUnkownDesktopFolder);
	        if (S_OK != nResult)
	        {
		        throw new ShellContextMenuException("Failed to get the desktop shell folder");
	        }
	        _oDesktopFolder =
		        (IShellFolder) Marshal.GetTypedObjectForIUnknown(pUnkownDesktopFolder, typeof (IShellFolder));

	        return _oDesktopFolder;
        }

        #endregion

        #region GetParentFolder()

        /// <summary>
        ///     Gets the parent folder
        /// </summary>
        /// <param name="folderName">Folder path</param>
        /// <returns>IShellFolder for the folder (relative from the desktop)</returns>
        private IShellFolder GetParentFolder(string folderName)
        {
	        if (null != _oParentFolder) return _oParentFolder;
	        var oDesktopFolder = GetDesktopFolder();
	        if (null == oDesktopFolder)
	        {
		        return null;
	        }

	        // Get the PIDL for the folder file is in
	        IntPtr pPidl;
	        uint pchEaten = 0;
	        Sfgao pdwAttributes = 0;
	        var nResult = oDesktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, folderName, ref pchEaten,
		        out pPidl,
		        ref pdwAttributes);
	        if (S_OK != nResult)
	        {
		        return null;
	        }

	        var pStrRet = Marshal.AllocCoTaskMem(MAX_PATH*2 + 4);
	        Marshal.WriteInt32(pStrRet, 0, 0);
	        _oDesktopFolder.GetDisplayNameOf(pPidl, Shgno.ForParsing, pStrRet);
	        var strFolder = new StringBuilder(MAX_PATH);
	        StrRetToBuf(pStrRet, pPidl, strFolder, MAX_PATH);
	        Marshal.FreeCoTaskMem(pStrRet);
	        _strParentFolder = strFolder.ToString();

	        // Get the IShellFolder for folder
	        IntPtr pUnknownParentFolder;
	        nResult = oDesktopFolder.BindToObject(pPidl, IntPtr.Zero, ref _iidIShellFolder, out pUnknownParentFolder);
	        // Free the PIDL first
	        Marshal.FreeCoTaskMem(pPidl);
	        if (S_OK != nResult)
	        {
		        return null;
	        }
	        _oParentFolder =
		        (IShellFolder) Marshal.GetTypedObjectForIUnknown(pUnknownParentFolder, typeof (IShellFolder));

	        return _oParentFolder;
        }

        #endregion

        #region GetPIDLs()

        /// <summary>
        ///     Get the PIDLs
        /// </summary>
        /// <param name="arrFi">Array of FileInfo</param>
        /// <returns>Array of PIDLs</returns>
        protected IntPtr[] GetPidLs(FileInfo[] arrFi)
        {
            if (null == arrFi || 0 == arrFi.Length)
            {
                return null;
            }

            var oParentFolder = GetParentFolder(arrFi[0].DirectoryName);
            if (null == oParentFolder)
            {
                return null;
            }

            var arrPidLs = new IntPtr[arrFi.Length];
            var n = 0;
            foreach (var fi in arrFi)
            {
                // Get the file relative to folder
                uint pchEaten = 0;
                Sfgao pdwAttributes = 0;
                IntPtr pPidl;
                var nResult = oParentFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, fi.Name, ref pchEaten, out pPidl,
                    ref pdwAttributes);
                if (S_OK != nResult)
                {
                    FreePidLs(arrPidLs);
                    return null;
                }
                arrPidLs[n] = pPidl;
                n++;
            }

            return arrPidLs;
        }

        #endregion

        #region FreePIDLs()

        /// <summary>
        ///     Free the PIDLs
        /// </summary>
        /// <param name="arrPidLs">Array of PIDLs (IntPtr)</param>
        protected void FreePidLs(IntPtr[] arrPidLs)
        {
            if (null == arrPidLs) return;
            for (var n = 0; n < arrPidLs.Length; n++)
            {
                if (arrPidLs[n] == IntPtr.Zero) continue;
                Marshal.FreeCoTaskMem(arrPidLs[n]);
                arrPidLs[n] = IntPtr.Zero;
            }
        }

        #endregion

        #region Constructor

        #endregion

        #region InvokeContextMenuDefault

/*
        private void InvokeContextMenuDefault(FileInfo[] arrFI)
        {
            // Release all resources first.
            ReleaseAll();

            var pMenu = IntPtr.Zero;

            try
            {
                _arrPIDLs = GetPidLs(arrFI);
                if (null == _arrPIDLs)
                {
                    ReleaseAll();
                    return;
                }

                if (false == GetContextMenuInterfaces(_oParentFolder, _arrPIDLs))
                {
                    ReleaseAll();
                    return;
                }

                pMenu = CreatePopupMenu();

                var nResult = _oContextMenu.QueryContextMenu(
                    pMenu,
                    0,
                    CMD_FIRST,
                    CMD_LAST,
                    CMF.DEFAULTONLY |
                    ((Control.ModifierKeys & Keys.Shift) != 0 ? CMF.ExtendedVerbs : 0));

                var nDefaultCmd = (uint)GetMenuDefaultItem(pMenu, false, 0);
                if (nDefaultCmd >= CMD_FIRST)
                {
                    InvokeCommand(_oContextMenu, nDefaultCmd, arrFI[0].DirectoryName, Control.MousePosition);
                }

                DestroyMenu(pMenu);
                pMenu = IntPtr.Zero;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pMenu != IntPtr.Zero)
                {
                    DestroyMenu(pMenu);
                }
                ReleaseAll();
            }
        }
*/

        #endregion

        #region ShowContextMenu()

        /// <summary>
        ///     Shows the context menu
        /// </summary>
        /// <param name="handleOwner">Window that will get messages</param>
        /// <param name="arrFi">FileInfos (should all be in same directory)</param>
        /// <param name="pointScreen">Where to show the menu</param>
        public void ShowContextMenu(IntPtr handleOwner, FileInfo[] arrFi, System.Drawing.Point pointScreen)
        {
            // Release all resources first.
            ReleaseAll();

            var pMenu = IntPtr.Zero;
            // ReSharper disable once UseObjectOrCollectionInitializer
            var hook = new LocalWindowsHook(HookType.WhCallwndproc);
            hook.HookInvoked += WindowsHookInvoked;

            try
            {
                //Application.AddMessageFilter(this);

                _arrPidLs = GetPidLs(arrFi);
                if (null == _arrPidLs)
                {
                    ReleaseAll();
                    return;
                }

                if (false == GetContextMenuInterfaces(_oParentFolder, _arrPidLs))
                {
                    ReleaseAll();
                    return;
                }

                pMenu = CreatePopupMenu();

                _oContextMenu.QueryContextMenu(
                    pMenu,
                    0,
                    CMD_FIRST,
                    CMD_LAST,
                    Cmf.Explore |
                    Cmf.Normal |
                    ((Control.ModifierKeys & Keys.Shift) != 0 ? Cmf.ExtendedVerbs : 0));

                hook.Install();

                var nSelected = TrackPopupMenuEx(
                    pMenu,
                    Tpm.ReturnCmd,
                    pointScreen.X,
                    pointScreen.Y,
                    handleOwner,
                    IntPtr.Zero);

                DestroyMenu(pMenu);
                pMenu = IntPtr.Zero;

                if (nSelected != 0)
                {
                    InvokeCommand(_oContextMenu, nSelected, _strParentFolder, pointScreen);
                }
            }
            finally
            {
                hook.Uninstall();
                if (pMenu != IntPtr.Zero)
                {
                    DestroyMenu(pMenu);
                }
                ReleaseAll();
            }
        }

        #endregion

        #region WindowsHookInvoked()

        /// <summary>
        ///     Handle messages for context menu
        /// </summary>
        private void WindowsHookInvoked(object sender, HookEventArgs e)
        {
            var cwp = (Cwpstruct) Marshal.PtrToStructure(e.LParam, typeof (Cwpstruct));

            if (_oContextMenu2 != null &&
                (cwp.message == (int) Wm.InitMenuPopup ||
                 cwp.message == (int) Wm.MeasureItem ||
                 cwp.message == (int) Wm.DrawItem))
            {
                if (_oContextMenu2.HandleMenuMsg((uint) cwp.message, cwp.wparam, cwp.lparam) == S_OK)
                {
                    return;
                }
            }

            if (_oContextMenu3 == null || cwp.message != (int) Wm.MenuChar) return;
            if (_oContextMenu3.HandleMenuMsg2((uint) cwp.message, cwp.wparam, cwp.lparam, IntPtr.Zero) == S_OK)
            {
            }
        }

        #endregion

        #region Local variables

        private IntPtr[] _arrPidLs;
        private IContextMenu _oContextMenu;
        private IContextMenu2 _oContextMenu2;
        private IContextMenu3 _oContextMenu3;
        private IShellFolder _oDesktopFolder;
        private IShellFolder _oParentFolder;
        private string _strParentFolder;

        #endregion

        #region Variables and Constants

        private const int MAX_PATH = 260;
        private const uint CMD_FIRST = 1;
        private const uint CMD_LAST = 30000;

        private const int S_OK = 0;

/*
	    private static readonly int CbMenuItemInfo = Marshal.SizeOf(typeof(MenuItemInfo));
*/
        private static readonly int CbInvokeCommand = Marshal.SizeOf(typeof (CmInvokeCommandInfoEx));

        #endregion

        #region DLL Import

        // Retrieves the IShellFolder interface for the desktop folder, which is the root of the Shell's namespace.
        [DllImport("shell32.dll")]
        private static extern Int32 SHGetDesktopFolder(out IntPtr ppshf);

        // Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf, converts it to a string, and places the result in a buffer. 
        [DllImport("shlwapi.dll", EntryPoint = "StrRetToBuf", ExactSpelling = false, CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern Int32 StrRetToBuf(IntPtr pstr, IntPtr pidl, StringBuilder pszBuf, int cchBuf);

        // The TrackPopupMenuEx function displays a shortcut menu at the specified location and tracks the selection of items on the shortcut menu. The shortcut menu can appear anywhere on the screen.
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern uint TrackPopupMenuEx(IntPtr hmenu, Tpm flags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        // The CreatePopupMenu function creates a drop-down menu, submenu, or shortcut menu. The menu is initially empty. You can insert or append menu items by using the InsertMenuItem function. You can also use the InsertMenu function to insert menu items and the AppendMenu function to append menu items.
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreatePopupMenu();

        // The DestroyMenu function destroys the specified menu and frees any memory that the menu occupies.
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool DestroyMenu(IntPtr hMenu);

        // Determines the default menu item on the specified menu
/*
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetMenuDefaultItem(IntPtr hMenu, bool fByPos, uint gmdiFlags);
*/

        #endregion

        #region Shell GUIDs

        private static Guid _iidIShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
        private static Guid _iidIContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");
        private static Guid _iidIContextMenu2 = new Guid("{000214f4-0000-0000-c000-000000000046}");
        private static Guid _iidIContextMenu3 = new Guid("{bcfce0a0-ec17-11d0-8d10-00a0c90f2719}");

        #endregion

        #region Structs

        // Contains extended information about a shortcut menu command
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CmInvokeCommandInfoEx
        {
            public int cbSize;
            public Cmic fMask;
            private readonly IntPtr hwnd;
            public IntPtr lpVerb;
            [MarshalAs(UnmanagedType.LPStr)] private readonly string lpParameters;
            [MarshalAs(UnmanagedType.LPStr)] public string lpDirectory;
            public Sw nShow;
            private readonly int dwHotKey;
            private readonly IntPtr hIcon;
            [MarshalAs(UnmanagedType.LPStr)] private readonly string lpTitle;
            public IntPtr lpVerbW;
            [MarshalAs(UnmanagedType.LPWStr)] private readonly string lpParametersW;
            [MarshalAs(UnmanagedType.LPWStr)] public string lpDirectoryW;
            [MarshalAs(UnmanagedType.LPWStr)] private readonly string lpTitleW;
            public Point ptInvoke;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Cwpstruct
        {
            public readonly IntPtr lparam;
            public readonly IntPtr wparam;
            public readonly int message;
            private readonly IntPtr hwnd;
        }

        // Contains information about a menu item
//        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
//        private struct MenuItemInfo
//        {
        /// *
//            public MenuItemInfo(string text)
//            {
//                cbSize = CbMenuItemInfo;
//                dwTypeData = text;
//                cch = text.Length;
//                fMask = 0;
//                fType = 0;
//                fState = 0;
//                wID = 0;
//                hSubMenu = IntPtr.Zero;
//                hbmpChecked = IntPtr.Zero;
//                hbmpUnchecked = IntPtr.Zero;
//                dwItemData = IntPtr.Zero;
//                hbmpItem = IntPtr.Zero;
//            }
//*/
//
//	        private readonly int cbSize;
//	        private readonly Miim fMask;
//	        private readonly Mft fType;
//	        private readonly Mfs fState;
//	        private readonly uint wID;
//	        private readonly IntPtr hSubMenu;
//	        private readonly IntPtr hbmpChecked;
//	        private readonly IntPtr hbmpUnchecked;
//	        private readonly IntPtr dwItemData;
//            [MarshalAs(UnmanagedType.LPTStr)] private readonly string dwTypeData;
//	        private readonly int cch;
//	        private readonly IntPtr hbmpItem;
//        }

        // A generalized global memory handle used for data transfer operations by the 
        // IAdviseSink, IDataObject, and IOleCache interfaces
/*
        [StructLayout(LayoutKind.Sequential)]
        private struct StgMedium
        {
            public TYMED tymed;
            public IntPtr hBitmap;
            public IntPtr hMetaFilePict;
            public IntPtr hEnhMetaFile;
            public IntPtr hGlobal;
            public IntPtr lpszFileName;
            public IntPtr pstm;
            public IntPtr pstg;
            public IntPtr pUnkForRelease;
        }
*/

        // Defines the x- and y-coordinates of a point
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct Point
        {
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
            private readonly int x;
// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
            private readonly int y;
        }

        #endregion

        #region Enums

        // Defines the values used with the IShellFolder::GetDisplayNameOf and IShellFolder::SetNameOf 
        // methods to specify the type of file or folder names used by those methods

        // Specifies how the shortcut menu can be changed when calling IContextMenu::QueryContextMenu
        [Flags]
        private enum Cmf : uint
        {
            Normal = 0x00000000,
            //DEFAULTONLY = 0x00000001,
            //VERBSONLY = 0x00000002,
            Explore = 0x00000004,
            //NOVERBS = 0x00000008,
            //CANRENAME = 0x00000010,
            //NODEFAULT = 0x00000020,
            //INCLUDESTATIC = 0x00000040,
            ExtendedVerbs = 0x00000100
            //RESERVED = 0xffff0000
        }

        // Flags specifying the information to return when calling IContextMenu::GetCommandString

        // Flags used with the CmInvokeCommandInfoEx structure
        [Flags]
        private enum Cmic : uint
        {
            //HOTKEY = 0x00000020,
            //ICON = 0x00000010,
            //FLAG_NO_UI = 0x00000400,
            Unicode = 0x00004000,
            //NO_CONSOLE = 0x00008000,
            //ASYNCOK = 0x00100000,
            //NOZONECHECKS = 0x00800000,
            ShiftDown = 0x10000000,
            ControlDown = 0x40000000,
            //FLAG_LOG_USAGE = 0x04000000,
            Ptinvoke = 0x20000000
        }

        [Flags]
        private enum Gcs : uint
        {
/*
            VERBA = 0,
            HELPTEXTA = 1,
            VALIDATEA = 2,
            VERBW = 4,
            HELPTEXTW = 5,
            VALIDATEW = 6*/
        }

        [Flags]
        private enum Sfgao : uint
        {
/*
            BROWSABLE = 0x8000000,
            CANCOPY = 1,
            CANDELETE = 0x20,
            CANLINK = 4,
            CANMONIKER = 0x400000,
            CANMOVE = 2,
            CANRENAME = 0x10,
            CAPABILITYMASK = 0x177,
            COMPRESSED = 0x4000000,
            CONTENTSMASK = 0x80000000,
            DISPLAYATTRMASK = 0xfc000,
            DROPTARGET = 0x100,
            ENCRYPTED = 0x2000,
            FILESYSANCESTOR = 0x10000000,
            FILESYSTEM = 0x40000000,
            FOLDER = 0x20000000,
            GHOSTED = 0x8000,
            HASPROPSHEET = 0x40,
            HASSTORAGE = 0x400000,
            HASSUBFOLDER = 0x80000000,
            HIDDEN = 0x80000,
            ISSLOW = 0x4000,
            LINK = 0x10000,
            NEWCONTENT = 0x200000,
            NONENUMERATED = 0x100000,
            READONLY = 0x40000,
            REMOVABLE = 0x2000000,
            SHARE = 0x20000,
            STORAGE = 8,
            STORAGEANCESTOR = 0x800000,
            STORAGECAPMASK = 0x70c50008,
            STREAM = 0x400000,
            VALIDATE = 0x1000000
		   */
        }

        // Determines the type of items included in an enumeration. 
        // These values are used with the IShellFolder::EnumObjects method
        [Flags]
        private enum Shcontf
        {
/*
            FOLDERS = 0x0020,
            NONFOLDERS = 0x0040,
            INCLUDEHIDDEN = 0x0080,
            INIT_ON_FIRST_NEXT = 0x0100,
            NETPRINTERSRCH = 0x0200,
            SHAREABLE = 0x0400,
            STORAGE = 0x0800,*/
        }

        [Flags]
        private enum Shgno
        {
/*
            Normal = 0x0000,
            InFolder = 0x0001,
            ForEditing = 0x1000,
            ForAddressBar = 0x4000,
*/
            ForParsing = 0x8000
        }

        // Specifies how the window is to be shown
        [Flags]
        private enum Sw
        {
            //HIDE = 0,
            ShowNormal = 1 /*
            NORMAL = 1,
            SHOWMINIMIZED = 2,
            SHOWMAXIMIZED = 3,
            MAXIMIZE = 3,
            SHOWNOACTIVATE = 4,
            SHOW = 5,
            MINIMIZE = 6,
            SHOWMINNOACTIVE = 7,
            SHOWNA = 8,
            RESTORE = 9,
            SHOWDEFAULT = 10,*/
        }

        [Flags]
        private enum Tpm : uint
        {
/*
            LEFTBUTTON = 0x0000,
            RIGHTBUTTON = 0x0002,
            LEFTALIGN = 0x0000,
            CENTERALIGN = 0x0004,
            RIGHTALIGN = 0x0008,
            TOPALIGN = 0x0000,
            VCENTERALIGN = 0x0010,
            BOTTOMALIGN = 0x0020,
            HORIZONTAL = 0x0000,
            VERTICAL = 0x0040,
            NONOTIFY = 0x0080,*/
            ReturnCmd = 0x0100 /*
            RECURSE = 0x0001,
            HORPOSANIMATION = 0x0400,
            HORNEGANIMATION = 0x0800,
            VERPOSANIMATION = 0x1000,
            VERNEGANIMATION = 0x2000,
            NOANIMATION = 0x4000,
            LAYOUTRTL = 0x8000*/
        }

        // Window message flags
        [Flags]
        private enum Wm : uint
        {
            DrawItem = 0x2B,
            InitMenuPopup = 0x117,
            MeasureItem = 0x2C,
            MenuChar = 0x120
        }

        // Specifies the content of the new menu item
//        [Flags]
//        private enum Mft : uint
//        {/*
//            GRAYED = 0x00000003,
//            DISABLED = 0x00000003,
//            CHECKED = 0x00000008,
//            SEPARATOR = 0x00000800,
//            RADIOCHECK = 0x00000200,
//            BITMAP = 0x00000004,
//            OWNERDRAW = 0x00000100,
//            MENUBARBREAK = 0x00000020,
//            MENUBREAK = 0x00000040,
//            RIGHTORDER = 0x00002000,
//            BYCOMMAND = 0x00000000,
//            BYPOSITION = 0x00000400,
//            POPUP = 0x00000010*/
//        }

        // Specifies the state of the new menu item
//        [Flags]
//        private enum Mfs : uint
//        {/*
//            GRAYED = 0x00000003,
//            DISABLED = 0x00000003,
//            CHECKED = 0x00000008,
//            HILITE = 0x00000080,
//            ENABLED = 0x00000000,
//            UNCHECKED = 0x00000000,
//            UNHILITE = 0x00000000,
//            DEFAULT = 0x00001000*/
//        }

        // Specifies the content of the new menu item
//        [Flags]
//        private enum Miim : uint
//        {/*
//            BITMAP = 0x80,
//            CHECKMARKS = 0x08,
//            DATA = 0x20,
//            FTYPE = 0x100,
//            ID = 0x02,
//            STATE = 0x01,
//            STRING = 0x40,
//            SUBMENU = 0x04,
//            TYPE = 0x10*/
//        }

        // Indicates the type of storage medium being used in a data transfer

        #endregion

        #region IShellFolder

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214E6-0000-0000-C000-000000000046")]
        private interface IShellFolder
        {
            // Translates a file object's or folder's display name into an item identifier list.
            // Return value: error code, if any
            [PreserveSig]
            Int32 ParseDisplayName(
                IntPtr hwnd,
                IntPtr pbc,
                [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName,
                ref uint pchEaten,
                out IntPtr ppidl,
                ref Sfgao pdwAttributes);

            // Allows a client to determine the contents of a folder by creating an item
            // identifier enumeration object and returning its IEnumIDList interface.
            // Return value: error code, if any
            [PreserveSig]
            Int32 EnumObjects(
                IntPtr hwnd,
                Shcontf grfFlags,
                out IntPtr enumIdList);

            // Retrieves an IShellFolder object for a subfolder.
            // Return value: error code, if any
            [PreserveSig]
            Int32 BindToObject(
                IntPtr pidl,
                IntPtr pbc,
                ref Guid riid,
                out IntPtr ppv);

            // Requests a pointer to an object's storage interface. 
            // Return value: error code, if any
            [PreserveSig]
            Int32 BindToStorage(
                IntPtr pidl,
                IntPtr pbc,
                ref Guid riid,
                out IntPtr ppv);

            // Determines the relative order of two file objects or folders, given their
            // item identifier lists. Return value: If this method is successful, the
            // CODE field of the HRESULT contains one of the following values (the code
            // can be retrived using the helper function GetHResultCode): Negative A
            // negative return value indicates that the first item should precede
            // the second (pidl1 < pidl2). 

            // Positive A positive return value indicates that the first item should
            // follow the second (pidl1 > pidl2).  Zero A return value of zero
            // indicates that the two items are the same (pidl1 = pidl2). 
            [PreserveSig]
            Int32 CompareIDs(
                IntPtr lParam,
                IntPtr pidl1,
                IntPtr pidl2);

            // Requests an object that can be used to obtain information from or interact
            // with a folder object.
            // Return value: error code, if any
            [PreserveSig]
            Int32 CreateViewObject(
                IntPtr hwndOwner,
                Guid riid,
                out IntPtr ppv);

            // Retrieves the attributes of one or more file objects or subfolders. 
            // Return value: error code, if any
            [PreserveSig]
            Int32 GetAttributesOf(
                uint cidl,
                [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
                ref Sfgao rgfInOut);

            // Retrieves an OLE interface that can be used to carry out actions on the
            // specified file objects or folders.
            // Return value: error code, if any
            [PreserveSig]
            Int32 GetUIObjectOf(
                IntPtr hwndOwner,
                uint cidl,
                [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl,
                ref Guid riid,
                IntPtr rgfReserved,
                out IntPtr ppv);

            // Retrieves the display name for the specified file object or subfolder. 
            // Return value: error code, if any
            [PreserveSig]
            Int32 GetDisplayNameOf(
                IntPtr pidl,
                Shgno uFlags,
                IntPtr lpName);

            // Sets the display name of a file object or subfolder, changing the item
            // identifier in the process.
            // Return value: error code, if any
            [PreserveSig]
            Int32 SetNameOf(
                IntPtr hwnd,
                IntPtr pidl,
                [MarshalAs(UnmanagedType.LPWStr)] string pszName,
                Shgno uFlags,
                out IntPtr ppidlOut);
        }

        #endregion

        #region IContextMenu

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214e4-0000-0000-c000-000000000046")]
        private interface IContextMenu
        {
            // Adds commands to a shortcut menu
            [PreserveSig]
            Int32 QueryContextMenu(
                IntPtr hmenu,
                uint iMenu,
                uint idCmdFirst,
                uint idCmdLast,
                Cmf uFlags);

            // Carries out the command associated with a shortcut menu item
            [PreserveSig]
            Int32 InvokeCommand(
                ref CmInvokeCommandInfoEx info);

            // Retrieves information about a shortcut menu command, 
            // including the help string and the language-independent, 
            // or canonical, name for the command
            [PreserveSig]
            Int32 GetCommandString(
                uint idcmd,
                Gcs uflags,
                uint reserved,
                [MarshalAs(UnmanagedType.LPArray)] byte[] commandstring,
                int cch);
        }

        [ComImport, Guid("000214f4-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IContextMenu2
        {
            // Adds commands to a shortcut menu
            [PreserveSig]
            Int32 QueryContextMenu(
                IntPtr hmenu,
                uint iMenu,
                uint idCmdFirst,
                uint idCmdLast,
                Cmf uFlags);

            // Carries out the command associated with a shortcut menu item
            [PreserveSig]
            Int32 InvokeCommand(
                ref CmInvokeCommandInfoEx info);

            // Retrieves information about a shortcut menu command, 
            // including the help string and the language-independent, 
            // or canonical, name for the command
            [PreserveSig]
            Int32 GetCommandString(
                uint idcmd,
                Gcs uflags,
                uint reserved,
                [MarshalAs(UnmanagedType.LPWStr)] StringBuilder commandstring,
                int cch);

            // Allows client objects of the IContextMenu interface to 
            // handle messages associated with owner-drawn menu items
            [PreserveSig]
            Int32 HandleMenuMsg(
                uint uMsg,
                IntPtr wParam,
                IntPtr lParam);
        }

        [ComImport, Guid("bcfce0a0-ec17-11d0-8d10-00a0c90f2719")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IContextMenu3
        {
            // Adds commands to a shortcut menu
            [PreserveSig]
            Int32 QueryContextMenu(
                IntPtr hmenu,
                uint iMenu,
                uint idCmdFirst,
                uint idCmdLast,
                Cmf uFlags);

            // Carries out the command associated with a shortcut menu item
            [PreserveSig]
            Int32 InvokeCommand(
                ref CmInvokeCommandInfoEx info);

            // Retrieves information about a shortcut menu command, 
            // including the help string and the language-independent, 
            // or canonical, name for the command
            [PreserveSig]
            Int32 GetCommandString(
                uint idcmd,
                Gcs uflags,
                uint reserved,
                [MarshalAs(UnmanagedType.LPWStr)] StringBuilder commandstring,
                int cch);

            // Allows client objects of the IContextMenu interface to 
            // handle messages associated with owner-drawn menu items
            [PreserveSig]
            Int32 HandleMenuMsg(
                uint uMsg,
                IntPtr wParam,
                IntPtr lParam);

            // Allows client objects of the IContextMenu3 interface to 
            // handle messages associated with owner-drawn menu items
            [PreserveSig]
            Int32 HandleMenuMsg2(
                uint uMsg,
                IntPtr wParam,
                IntPtr lParam,
                IntPtr plResult);
        }

        #endregion
    }

    #region ShellContextMenuException

    #endregion

    #region Class HookEventArgs

    #endregion

    #region Enum HookType

    // Hook Types

    #endregion

    #region Class LocalWindowsHook

    #endregion
}