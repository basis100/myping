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
    public partial class Form_txt : Form
    {
        public Form_txt()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Form1.messagetxt = textBox2.Text;
            this.Close();
            
          
        }
    }
}
