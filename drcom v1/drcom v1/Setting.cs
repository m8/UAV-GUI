using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace drcom_v1
{
    public partial class Setting : Form
    {
        int time;
        public Form1 mainForm;
        Timer test = new Timer();
        public Setting()
        {

            InitializeComponent();
            test.Tick += new EventHandler(test_tick);
            test.Interval = 100;
            //test.Start();
        }

        private void test_tick(object sender, EventArgs e)
        {
            Console.WriteLine("serial open:" + serialPort1.IsOpen);
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            foreach (string item in System.IO.Ports.SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (button1.Text == "Connect" && !serialPort1.IsOpen)
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    //serialPort1.DataReceived += serialport1_datareceived;
                    serialPort1.Open();

                    //Form1 frm2 = new Form1();
                    mainForm.setSerialPort(serialPort1);
                    //frm2.Show();


                    button1.Text = "Disconnect";
                    label2.Text = "Status: Connected";
                    label2.BackColor = Color.GreenYellow;
                }
                else if (button1.Text == "Disconnect" && serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    button1.Text = "Connect";
                    label2.Text = "Status: None";
                    label2.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void sensorsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        Stopwatch sw = new Stopwatch();
        private void button3_Click(object sender, EventArgs e)
        {
            sw.Reset();
            sw.Start();
            serialPort1.Write("1");
            
            
        }

        //private void serialport1_datareceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        //{
        //    sw.Stop();
        //    Console.WriteLine("Elapsed:" + sw.ElapsedMilliseconds);
        //}

        //    string data = serialport1.readline();
        //    data = data.trimstart('\0').trimend('\r');
        //    //data = data.trimstart('')
        //    //byte[] utf8bytes= encoding.utf8.getbytes
        //    //string vas = system.text.unicodeencoding(data);
        //    listbox1.ınvoke((methodınvoker)(() => listbox1.ıtems.add(data)));
        //    listbox1.ınvoke((methodınvoker)(() => listbox1.selectedındex = listbox1.ıtems.count - 1));
        //    listbox1.ınvoke((methodınvoker)(() => listbox1.selectedındex = -1));
        //    console.writeline("read");
        //}

        //private void serialPort1_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        //{

        //}


    }
}
