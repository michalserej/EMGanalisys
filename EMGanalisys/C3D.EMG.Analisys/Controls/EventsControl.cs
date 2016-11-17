﻿using System;
using System.Windows.Forms;

namespace C3D.EMG.Analisys.Controls
{
    public partial class EventsControl : UserControl
    {
        public EventsControl(C3DFile file)
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

            C3DHeaderEvent[] events = file.Header.GetAllHeaderEvents();
            if (events != null && events.Length > 0)
            {
                for (Int32 i = 0; i < events.Length; i++)
                {
                    this.lvItems.Items.Add(new ListViewItem(new String[] { events[i].EventName, events[i].EventTime.ToString(), events[i].IsDisplay.ToString() }));
                }
            }
        }
    }
}