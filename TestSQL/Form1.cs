using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestSQL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string c = textBox1.Text;
            DBUtility.Conntection.testConnection = c;
            LoggerHelper.Info("main123", "start01", "start0", "start0", true, Guid.Empty);

        }
    }
}
