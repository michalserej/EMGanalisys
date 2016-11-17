﻿using System;
using System.Windows.Forms;

namespace C3D.EMG.Analisys.Controls
{
    public partial class OverviewControl : UserControl
    {
        public OverviewControl(C3DFile file)
        {
            InitializeComponent();
            this.LoadData(file);
        }

        private void LoadData(C3DFile file)
        {
            if (file == null)
            {
                return;
            }

            C3DParameterCache parameterCache = C3DParameterCache.CreateCache(file);

            this.lvItems.Items.Add(new ListViewItem(new String[] { "File Format", file.CreateProcessorType.ToString() }));
            this.lvItems.Items.Add(new ListViewItem(new String[] { "Number Format", parameterCache.ScaleFactor > 0 ? "Integer" : "Float" }));
            this.lvItems.Items.Add(new ListViewItem(new String[] { "Frame Count", parameterCache.FrameCount.ToString() }));
            this.lvItems.Items.Add(new ListViewItem(new String[] { "Frame Rate", parameterCache.FrameRate.ToString() + " FPS" }));
            this.lvItems.Items.Add(new ListViewItem(new String[] { "Frame Duration", ((Double)parameterCache.FrameCount / (Double)parameterCache.FrameRate).ToString("F3") + " s" }));
            this.lvItems.Items.Add(new ListViewItem(new String[] { "3D Point Count", parameterCache.PointCount.ToString() }));
            this.lvItems.Items.Add(new ListViewItem(new String[] { "Analog Channel Count", parameterCache.AnalogChannelCount.ToString() }));
            this.lvItems.Items.Add(new ListViewItem(new String[] { "Analog Samples Per Frame", parameterCache.AnalogSamplesPerFrame.ToString() }));
        }
    }
}