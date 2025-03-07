﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidRedirectNotification
{
    internal partial class SettingsForm : Form
    {
        public Settings? Value { get; set; }
        public SettingsForm(Settings settings)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.portNum.Value = settings.Port;
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Port = (ushort)portNum.Value;
            try
            {
                Settings.SaveSettings(settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save Settings Failed.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Value = settings;
            this.Close();
        }

        private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
