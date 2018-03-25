using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace myping
{
    public partial class Form2add : Form
    {
        public Form2add()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myping.Form1.iptxt = textBox1.Text;
            this.Close();
        
            
        }
    }
}
