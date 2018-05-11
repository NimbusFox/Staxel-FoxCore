using System;
using System.Threading;
using System.Windows.Forms;

namespace NimbusFox.FoxCore.Forms {
    internal partial class ExceptionViewer : Form {

        internal ExceptionViewer(Exception exception, string mod) {
            InitializeComponent();
            Text = @"FoxCore Script Loader - " + mod;
            txtException.Text = $@"{exception.Message}{Environment.NewLine}{Environment.NewLine}{exception.StackTrace}";
        }

        private void btnOk_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
