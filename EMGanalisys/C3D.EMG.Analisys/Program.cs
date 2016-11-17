using System;
using System.Windows.Forms;

namespace C3D.EMG.Analisys
{
    static class Program
    {
        [STAThread]
        static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm((args != null && args.Length == 1) ? args[0] : String.Empty));
        }
    }
}