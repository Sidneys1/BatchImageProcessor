using System.Runtime.InteropServices;

namespace BatchImageProcessor.Native
{
	class Natives
	{
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool FreeConsole();
	}
}
