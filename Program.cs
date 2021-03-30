using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stat_app
{
    static class MyProgram
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form Statistics = new StatApp();
            //Statistics.FormBorderStyle = FormBorderStyle.FixedDialog;
            Application.Run(Statistics);
        }

    }
}
