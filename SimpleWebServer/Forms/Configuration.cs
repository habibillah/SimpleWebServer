using SimpleWebServer.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleWebServer.Forms
{
    public partial class Configuration : Form
    {
        private SimpleHttpServer server;

        public Configuration()
        {
            InitializeComponent();

            Utility.LoadSetting();
            this.txtFolderLocation.Text = CurrentSetting.Instance.directoryPath;
            this.txtPortNumber.Text = CurrentSetting.Instance.port.ToString();
            this.txtIPAddress.Text = Utility.GetLocalIP().ToString();

            this.BuildControlState();
        }

        private void BuildControlState()
        {
            var isRun = this.server != null && this.server.IsAlive;

            foreach (Control control in this.Controls)
            {
                control.Enabled = false;
            }

            if (isRun)
            {
                btnStop.Enabled = true;
            }
            else
            {
                btnStartServer.Enabled = true;
                btnBrowse.Enabled = true;
                txtPortNumber.Enabled = true;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = CurrentSetting.Instance.directoryPath;
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
            try
            {
                var setting = new Setting();
                setting.directoryPath = txtFolderLocation.Text.Trim();

                int port;
                if (int.TryParse(txtPortNumber.Text, out port))
                {
                    setting.port = port;
                }
                else
                {
                    setting.port = 8080;
                }

                Utility.SaveSetting(setting);
                btnSave.Enabled = false;

                MessageBox.Show("Saving configuration success.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                this.server = new Server(CurrentSetting.Instance.directoryPath, CurrentSetting.Instance.port);
                this.server.Start();
                this.BuildControlState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                this.server.Stop();
                this.BuildControlState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
