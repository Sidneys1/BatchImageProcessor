using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace BatchImageProcessor.View
{
    /// <summary>
    ///     Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox
    {
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

        #region Assembly Attribute Accessors

        public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string AssemblyDescription
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute) attributes[0]).Description;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                var attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof (AssemblyCompanyAttribute), false);
                return attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute) attributes[0]).Company;
            }
        }

        #endregion
    }
}