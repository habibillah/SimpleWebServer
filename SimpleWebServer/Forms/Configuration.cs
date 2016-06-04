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

            this.Hide();
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

                this.Icon = Icon.FromHandle(((Bitmap)imageList.Images["on.ico"]).GetHicon());
                notifyIcon.Icon = Icon.FromHandle(((Bitmap)imageList.Images["on.ico"]).GetHicon());

                startServerToolStripMenuItem.Enabled = false;
                stopServerToolStripMenuItem.Enabled = true;
            }
            else
            {
                btnStartServer.Enabled = true;
                btnBrowse.Enabled = true;
                txtPortNumber.Enabled = true;

                this.Icon = Icon.FromHandle(((Bitmap)imageList.Images["off.ico"]).GetHicon());
                notifyIcon.Icon = Icon.FromHandle(((Bitmap)imageList.Images["off.ico"]).GetHicon());

                startServerToolStripMenuItem.Enabled = true;
                stopServerToolStripMenuItem.Enabled = false;
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

        private void StartServer()
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

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            this.StartServer();    
        }

        private void StopServer()
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

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.StopServer();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((this.server != null) && (this.server.IsAlive))
                this.server.Stop();

            Application.ExitThread();
        }

        private void startServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.StartServer();
        }

        private void stopServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.StopServer();
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowConfigurationForm();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.ShowConfigurationForm();
        }

        private void Configuration_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void ShowConfigurationForm()
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;

            this.Show();
        }
    }
}
