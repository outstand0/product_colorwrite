using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace threadTest01
{
    public partial class passwordbox : Form
    {
        string passwordcode;
        string pw_config;
        public string filepath = null;

        IniReadWrite IniReader = new IniReadWrite();
        IniReadWrite IniWriter = new IniReadWrite();

        public passwordbox(string a)
        {
            InitializeComponent();

            lb_pwerror.Text = a;
            filepath = System.IO.Directory.GetCurrentDirectory() + "\\" + "config\\" + "config.ini";
            pw_config = IniReader.IniReadValue("CONFIG", "PASSWORD", filepath);
        }


        private void tb_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4)
            {
                e.Handled = true;
            }
        }

        private void tb_password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (tb_password.Text == pw_config)
                {
                    this.Close();
                }
                else
                {
                    tb_password.Clear();
                }
            }
        }
    }
}
