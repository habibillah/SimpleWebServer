using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleWebServer.Forms
{
    public partial class Configuration : Form
    {
        public Configuration()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == DialogResult.OK || dialogResult == DialogResult.Yes)
            {
                txtFolderLocation.Text = folderBrowserDialog.SelectedPath;
            }
        }
        
        private void activateSaveConfigurationButton()
        {
            btnSave.Enabled = true;
        }

        private void txtPortNumber_TextChanged(object sender, EventArgs e)
        {
            this.activateSaveConfigurationButton();
        }
        
        private void txtFolderLocation_TextChanged(object sender, EventArgs e)
        {
            this.activateSaveConfigurationButton();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
