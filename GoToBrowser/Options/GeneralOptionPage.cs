using System.Collections.Generic;
using System.Windows.Forms;

namespace GoToBrowser.Options
{
    public partial class GeneralOptionPage : UserControl
    {
        public string SolutionName
        {
            get { return solutionName.Text; }
            set { solutionName.Text = value; }
        }

        public string UrlFormat
        {
            get { return urlFormat.Text; }
            set { urlFormat.Text = value; }
        }

        public GeneralOptionPage()
        {
            InitializeComponent();
        }
    }
}