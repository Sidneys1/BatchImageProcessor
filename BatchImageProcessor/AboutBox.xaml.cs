using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace BatchImageProcessor
{
	/// <summary>
	/// Interaction logic for AboutBox.xaml
	/// </summary>
	public partial class AboutBox
	{
		#region Assembly Attribute Accessors

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}

		#endregion

		public AboutBox()
		{
			InitializeComponent();

			VersionTxt.Text = AssemblyVersion;
			CopyrightTxt.Text = AssemblyCopyright;
			CompanyTxt.Text = AssemblyCompany;
			DescriptionTxt.Text = AssemblyDescription;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var args = Environment.GetCommandLineArgs();

			if (args.Length > 1 && args.Contains("-noshaders"))
			{
				Resources["dse"] = null;
			}
		}

		private void okBtn_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
