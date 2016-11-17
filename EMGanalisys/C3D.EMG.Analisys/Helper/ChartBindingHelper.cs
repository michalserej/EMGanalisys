using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace C3D.EMG.Analisys.Helper
{
    internal static class ChartBindingHelper
    {
        internal static void BindDataToChart<T1, T2>(Chart chartCtl, Dictionary<T1, T2> data, Single minX, Single maxX, Single minY, Single maxY)
        {
            chartCtl.Series[0].Points.DataBind(data, "Key", "Value", "");
            chartCtl.ChartAreas[0].AxisX.Minimum = minX;
            chartCtl.ChartAreas[0].AxisX.Maximum = maxX;
            chartCtl.ChartAreas[0].AxisY.Minimum = minY;
            chartCtl.ChartAreas[0].AxisY.Maximum = maxY;
        }

        internal static void SetStripLineToChart(Chart chartCtl, Double offset, String name)
        {
            StripLine line = new StripLine();
            line.Interval = 0;
            line.IntervalOffset = offset;
            line.Text = name;
            line.BorderColor = Color.Black;
            line.BorderDashStyle = ChartDashStyle.Dash;
            line.BorderWidth = 1;

            chartCtl.ChartAreas[0].AxisX.StripLines.Add(line);
        }
    }
}