using System;
using System.Windows.Forms;

namespace StickyNotesApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Inicia o formulário principal
            Application.Run(new MainForm());
        }
    }
}