using System;
using System.Text;
using System.Windows.Forms;

namespace C3D.EMG.Analisys.Helper
{
    internal static class ListViewExtension
    {
        internal static String GetTableContent(this ListView listView)
        {
            if (listView == null)
            {
                throw new ArgumentNullException("listView");
            }

            StringBuilder sb = new StringBuilder();

            for (Int32 i = 0; i < listView.Columns.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append('\t');
                }

                sb.Append(listView.Columns[i].Text);
            }

            sb.AppendLine();

            for (Int32 i = 0; i < listView.Items.Count; i++)
            {
                for (Int32 j = 0; j < listView.Items[i].SubItems.Count; j++)
                {
                    if (j > 0)
                    {
                        sb.Append('\t');
                    }

                    sb.Append(listView.Items[i].SubItems[j].Text);
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}