using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using C3D.EMG.Analisys.Controls;
using C3D.EMG.Analisys.Status;

namespace C3D.EMG.Analisys.Helper
{
    internal static class ChartZoomHelper
    {
        internal static void ZoomChart(Chart chart, ChartScaleStatus status, Byte type, Int32 scale)
        {
            Axis axis = (type == 0 ? chart.ChartAreas[0].AxisX : chart.ChartAreas[0].AxisY);
            Int32 newScale = status.Scales[type] + scale;

            if (scale == 0 || newScale == 0)
            {
                axis.Maximum = status.Maxs[type];
                axis.Minimum = status.Mins[type];
                axis.ScaleView.ZoomReset(0);

                status.Scales[type] = 0;
            }
            else
            {
                if (newScale > 0)
                {
                    Double diff = status.Maxs[type] - status.Mins[type];

                    axis.Maximum = status.Maxs[type] + diff * 0.1 * Math.Pow(1.5, newScale);
                    axis.Minimum = status.Mins[type] - diff * 0.1 * Math.Pow(1.5, newScale);
                }
                else if (newScale < 0 && scale < 0)
                {
                    Double diff = axis.ScaleView.ViewMaximum - axis.ScaleView.ViewMinimum;

                    Double vMin = axis.ScaleView.ViewMinimum + diff * 0.1 * (1.0 + Math.Pow(1.5, newScale));
                    Double vMax = axis.ScaleView.ViewMaximum - diff * 0.1 * (1.0 + Math.Pow(1.5, newScale));

                    if (vMin >= vMax)
                    {
                        return;
                    }

                    axis.ScaleView.Zoom(vMin, vMax - vMin, DateTimeIntervalType.Number, true);
                }
                else if (newScale < 0 && scale > 0)
                {
                    axis.ScaleView.ZoomReset();
                }

                status.Scales[type] = newScale;
            }
        }

        internal static void SetChartMouseUp(Chart chart)
        {
            chart.Cursor = Cursors.Default;
        }

        internal static void SetChartMouseMoveLeftOrRight(Chart chart, Int32 moveDelta)
        {
            Axis axis = chart.ChartAreas[0].AxisX;
            Double delta = (axis.ScaleView.ViewMaximum - axis.ScaleView.ViewMinimum) / chart.Width * moveDelta;

            if (delta > 0 && axis.ScaleView.ViewMinimum - delta < axis.Minimum)//Mouse To Right
            {
                axis.ScaleView.Position += axis.Minimum - axis.ScaleView.ViewMinimum;
            }
            else if (delta < 0 && axis.ScaleView.ViewMaximum - delta > axis.Maximum)//Mouse To Left
            {
                axis.ScaleView.Position += axis.Maximum - axis.ScaleView.ViewMaximum;
            }
            else
            {
                axis.ScaleView.Position -= delta;
            }

            chart.Cursor = Cursors.Hand;
        }

        internal static void SetChartMouseMoveTopOrBottom(Chart chart, Int32 moveDelta)
        {
            Axis axis = chart.ChartAreas[0].AxisY;
            Double delta = (axis.ScaleView.ViewMaximum - axis.ScaleView.ViewMinimum) / chart.Height * moveDelta;

            if (delta > 0 && axis.ScaleView.ViewMaximum + delta > axis.Maximum)//Mouse To Down
            {
                axis.ScaleView.Position += axis.Maximum - axis.ScaleView.ViewMaximum;
            }
            else if (delta < 0 && axis.ScaleView.ViewMinimum + delta < axis.Minimum)//Mouse To Up
            {
                axis.ScaleView.Position += axis.Minimum - axis.ScaleView.ViewMinimum;
            }
            else
            {
                axis.ScaleView.Position += delta;
            }

            chart.Cursor = Cursors.Hand;
        }
    }
}