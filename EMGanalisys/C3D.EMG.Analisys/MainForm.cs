﻿using System;
using System.IO;
using System.Windows.Forms;

using C3D.EMG.Analisys.Controls;
using C3D.EMG.Analisys.Helper;

namespace C3D.EMG.Analisys
{
    public partial class MainForm : Form
    {
        private const String PROGRAM_TITLE = "EMG DataViewer";

        private String _currentFileName;
        private C3DFile _currentFile;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(String filePath)
        {
            InitializeComponent();

            if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                this.OpenFile(filePath);
            }
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = (e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None);
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            String[] files = e.Data.GetData(DataFormats.FileDrop, false) as String[];
            if (files != null && files.Length > 0)
            {
                this.OpenFile(files[0]);
            }
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            if (this.dlgOpen.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(this.dlgOpen.FileName))
            {
                this.OpenFile(this.dlgOpen.FileName);
            }
        }

        private void mnuReload_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this._currentFileName))
            {
                this.OpenFile(this._currentFileName);
            }
        }

        private void mnuClose_Click(object sender, EventArgs e)
        {
            this.CloseFile();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void tvItems_AfterSelect(object sender, TreeViewEventArgs e)
        {
            while (this.scMain.Panel2.Controls.Count > 0)
            {
                Control control = this.scMain.Panel2.Controls[0];

                this.scMain.Panel2.Controls.RemoveAt(0);

                control.Dispose();
                control = null;
            }

            if (this._currentFile == null)
            {
                return;
            }

            String tag = e.Node.Tag as String;

            /*if (tag.Equals("OVERVIEW"))
            {
                this.scMain.Panel2.Controls.Add(new OverviewControl(this._currentFile) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }
            else if (tag.Equals("HEADER"))
            {
                this.scMain.Panel2.Controls.Add(new HeaderControl(this._currentFile) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }
            else if (tag.Equals("EVENTS"))
            {
                this.scMain.Panel2.Controls.Add(new EventsControl(this._currentFile) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }
            else if (tag.Equals("PARAMETERS"))
            {
                this.scMain.Panel2.Controls.Add(new ParameterGroupControl(this._currentFile) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }
            else if (tag.Contains("PARAMETERS_GROUP|"))
            {
                String name = tag.Replace("PARAMETERS_GROUP|", "");
                this.scMain.Panel2.Controls.Add(new ParameterControl(this._currentFile, 0, name) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }
            else if (tag.Contains("PARAMETERS_ITEM|"))
            {
                String name = tag.Replace("PARAMETERS_ITEM|", "");
                this.scMain.Panel2.Controls.Add(new ParameterControl(this._currentFile, 1, name) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }*/
            if (tag.Equals("3D"))
            {
                this.scMain.Panel2.Controls.Add(new PointLabelsControl(this._currentFile) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }
            else if (tag.Contains("3D|"))
            {
                Int32 id = Int32.Parse(tag.Replace("3D|", ""));
                this.scMain.Panel2.Controls.Add(new Point3DControl(this._currentFile, id) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }
            else if (tag.Equals("ANALOG"))
            {
                this.scMain.Panel2.Controls.Add(new AnalogLabelsControl(this._currentFile) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }
            else if (tag.Contains("ANALOG|"))
            {
                Int32 id = Int32.Parse(tag.Replace("ANALOG|", ""));
                this.scMain.Panel2.Controls.Add(new AnalogSamplesControl(this._currentFile, id) { Width = this.scMain.Panel2.Width, Height = this.scMain.Panel2.Height, Dock = DockStyle.Fill });
            }
        }
        
        private void OpenFile(String filePath)
        {
            try
            {
                this._currentFileName = filePath;
                this._currentFile = C3DFile.LoadFromFile(this._currentFileName);
                this.Text = String.Format("{0} - {1}", this._currentFileName, MainForm.PROGRAM_TITLE);

                this.scMain.Panel2.Controls.Clear();
                this.ShowTreeList();
            }
            catch (Exception ex)
            {
                this.CloseFile();
                MessageBox.Show(String.Format("Error: {0}", ex.Message), MainForm.PROGRAM_TITLE, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void CloseFile()
        {
            this._currentFileName = null;
            this._currentFile = null;
            this.Text = MainForm.PROGRAM_TITLE;

            this.scMain.Panel2.Controls.Clear();
            this.tvItems.Nodes.Clear();
        }

        private void ShowTreeList()
        {
            this.tvItems.Nodes.Clear();

            TreeNode overview = TreeListHelper.GetOverviewNode(this._currentFile);
            //this.tvItems.Nodes.Add(overview);
            //this.tvItems.Nodes.Add(TreeListHelper.GetHeaderNode(this._currentFile));
            //this.tvItems.Nodes.Add(TreeListHelper.GetParametersNode(this._currentFile));
            this.tvItems.Nodes.Add(TreeListHelper.Get3DDataNode(this._currentFile));
            this.tvItems.Nodes.Add(TreeListHelper.GetAnalogDataNode(this._currentFile));

            this.tvItems.CollapseAll();
            this.tvItems.SelectedNode = overview;
        }
    }
}