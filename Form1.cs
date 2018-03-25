using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.IO;

using System.Collections;
using System.Threading;
using System.Data.OleDb;
using System.Net.Sockets;

using System.Management;
using System.Collections;
using System.Net;

namespace myping
{








    public partial class Form1 : Form
    {


        //日志处理
        StreamWriter sw = new StreamWriter("mypinglog.txt", true);

        public static string messagetxt = "abcdefghsdafsfsdfdsfsafdsfdsf";

        public static string iptxt = null;


        public Form1()
        {

            InitializeComponent();

            //herolist
            listView1.View = View.Details;
            listView1.LabelEdit = true;
            listView1.GridLines = true;
            listView1.Clear();
            listView1.Columns.Add("       日期时间    ");
            listView1.Columns.Add("    主机地址    ");
            listView1.Columns.Add("往返时间");
            listView1.Columns.Add("TTL");
            listView1.Columns.Add("大小");
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

             CheckForIllegalCrossThreadCalls = false;

            //一秒一次操作线程
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread m1Thread = new Thread(new ThreadStart(m1));
            m1Thread.IsBackground = true;
            m1Thread.Start();

            Networkcard_full();

        }

        private void button1_Click(object sender, EventArgs e)
        {
           



        }




        //一秒一次操作
        private void m1()
        {
            while (true)
            {
                Thread.Sleep(1000);

                try
                {

                    StartPing();
                }
                catch (Exception e) 
                {
                    MessageBox.Show(e.Message);
                
                }
            }
        }







        private void StartPing()
        {




            listView1.BeginUpdate();//工作线程用这个不会闪烁 


            if (listView1.Items.Count > 1000) listView1.Items.Clear();

            for (int i = 0; i < listBox1.Items.Count; i++)
         //   foreach (string s in listBox1.Items)
            {

                //远程服务器IP            
                string ipStr = listBox1.Items[i].ToString();
                //构造Ping实例
                Ping pingSender = new Ping();
                //Ping 选项设置
                PingOptions options = new PingOptions();
                options.DontFragment = true;
                //测试数据
                string data = messagetxt;
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                //设置超时时间
                int timeout = 120;
                //调用同步 send 方法发送数据,将返回结果保存至PingReply实例
                PingReply reply = pingSender.Send(ipStr, timeout, buffer, options);








                if (reply.Status == IPStatus.Success)
                {


                    ListViewItem item = new ListViewItem();//每读到一行就创建一项    
                    listView1.Items.Insert(0, item);//然后加到Listview  

                    item.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    item.SubItems.Add(reply.Address.ToString());
                    item.SubItems.Add(reply.RoundtripTime.ToString());
                    item.SubItems.Add(reply.Options.Ttl.ToString());
                    item.SubItems.Add(reply.Buffer.Length.ToString());

                    sw.Write(item.Text + " " + item.SubItems[1].Text + " " + item.SubItems[2].Text + " " + item.SubItems[3].Text + " " + item.SubItems[4].Text + Environment.NewLine);

                }
                else
                {
                    ListViewItem item = new ListViewItem();//每读到一行就创建一项    
                    listView1.Items.Insert(0, item);//然后加到Listview  
                    item.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    item.SubItems.Add(ipStr);
                    item.SubItems.Add("timeout");


                    sw.Write(item.Text + " " + item.SubItems[1].Text + " " + item.SubItems[2].Text + Environment.NewLine);
             

                }

             


                

            }

            listView1.EndUpdate();
           

        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = false;
            this.Close();
            this.Dispose();
            System.Environment.Exit(0);
            sw.Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
            {
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
            else
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            // 注意判断关闭事件reason来源于窗体按钮，否则用菜单退出时无法退出!
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //取消"关闭窗口"事件
                e.Cancel = true;
                //使关闭时窗口向右下角缩小的效果
                this.WindowState = FormWindowState.Minimized;
                this.notifyIcon1.Visible = true;
                this.Hide();
                return;

            }

        }



        public bool DisableNetWork(ManagementObject network)
        {

            try
            {

                network.InvokeMethod("Disable", null);
                return true;

            }

            catch
            {

                return false;

            }

        }



        public bool EnableNetWork(ManagementObject network)
        {

            try
            {
                network.InvokeMethod("Enable", null);
                return true;
            }

            catch
            {
                return false;
            }



        }
        


        public void Networkcard_full()
        {

            string netState = "SELECT * From Win32_NetworkAdapter";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(netState);

            ManagementObjectCollection collection = searcher.Get();

            foreach (ManagementObject manage in collection)
            {

                toolStripComboBox1.Items.Add(manage["Name"].ToString());

            }

          

        }


        public ManagementObject NetWork(string networkname)
        {

            string netState = "SELECT * From Win32_NetworkAdapter";



            ManagementObjectSearcher searcher = new ManagementObjectSearcher(netState);

            ManagementObjectCollection collection = searcher.Get();



            foreach (ManagementObject manage in collection)
            {

                if (manage["Name"].ToString() == networkname)
                {

                    return manage;

                }

            }





            return null;

        }



        //开启网卡
        private void button2_Click(object sender, EventArgs e)
        {

            
           // if (!EnableNetWork(NetWork("Intel(R) PRO/1000 MT Network Connection")))
            if (!EnableNetWork(NetWork(toolStripComboBox1.Text)))
            {

                ListViewItem item = new ListViewItem();//每读到一行就创建一项    
                listView1.Items.Insert(0, item);//然后加到Listview  
                item.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                item.SubItems.Add("网卡开失败");
            }
            else
            {


                ListViewItem item = new ListViewItem();//每读到一行就创建一项    
                listView1.Items.Insert(0, item);//然后加到Listview  
                item.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                item.SubItems.Add("网卡开成功");
            }


        }


        //禁用网卡
        private void button3_Click(object sender, EventArgs e)
        {


            if (!DisableNetWork(NetWork(toolStripComboBox1.Text)))
            {
                ListViewItem item = new ListViewItem();//每读到一行就创建一项    
                listView1.Items.Insert(0, item);//然后加到Listview  
                item.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                item.SubItems.Add("网卡禁用失败");
            }

            else
            {
                ListViewItem item = new ListViewItem();//每读到一行就创建一项    
                listView1.Items.Insert(0, item);//然后加到Listview  
                item.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                item.SubItems.Add("网卡禁用成功");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {

            //120秒一次操作线程
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread m120Thread = new Thread(new ThreadStart(m120));
            m120Thread.IsBackground = true;
            m120Thread.Start();
        }



        //秒一次操作
        private void m120()
        {
            int i = 0;
            while (true)
            {
                Thread.Sleep(1000);
                i++;
                label2.Text = i.ToString();



                if (i > Int32.Parse(textBox1.Text))
                {
                    button3.PerformClick();
                    Thread.Sleep(Int32.Parse(textBox2.Text)*1000);
                    button2.PerformClick();
                    i = 0;
                }



            }
        }

        //加IP
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form2add w = new Form2add();
            w.ShowDialog();

            IPAddress ip;
            if (IPAddress.TryParse(iptxt, out ip) && iptxt.Split('.').Length == 4)
            {
              //  MessageBox.Show("合法！");

                listBox1.Items.Insert(0, iptxt);
            }

            
         
        }
        //加文本内容
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Form_txt w = new Form_txt();
            w.ShowDialog();
        }

        //删除IP
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);

            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }


    }







//双缓冲 LISTVIEW
    public class ListViewNF : System.Windows.Forms.ListView
    {
        public ListViewNF()
        {
            // 开启双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }




}
