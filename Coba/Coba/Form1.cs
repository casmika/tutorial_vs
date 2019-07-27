using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace Coba
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            getAvailablePort();
        }

        void getAvailablePort()
        {
            comboBox1.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            comboBox1.Items.Add("Refresh");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            serialPort1.Write(TextInput.Text);
        }

        private void ComboBox1_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text == "Refresh")
            {
                getAvailablePort();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Start")
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = int.Parse(comboBox2.Text);
                serialPort1.ReadTimeout = 1000;
                serialPort1.WriteTimeout = 1000;
                serialPort1.DataReceived += SerialPort1_DataReceived;
                serialPort1.Open();

                button2.Text = "Stop";
            }
            else
            {
                serialPort1.Close();
                button2.Text = "Start";
            }
        }

        string DataMasuk = "";

        int dataIn;
        byte[] dataBuf = new byte[10];

        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
            try
            {
                SerialPort sp = (SerialPort)sender;
                //DataMasuk = serialPort1.ReadLine();
                dataIn = serialPort1.ReadByte();
                this.Invoke(new EventHandler(tampilin2));
            }
            catch (TimeoutException)
            {
                serialPort1.DiscardInBuffer();
            }
        }

        bool sudahKetemuHeader = false;
        int index = 0;
        void tampilin2(object sender, EventArgs e)
        {
            if(dataIn == 0x7E)
            {
                sudahKetemuHeader = true;
            }

            if(sudahKetemuHeader == true)
            {
                dataBuf[index] = (byte)dataIn;
                index++;
            }
            if(index > 6)
            {
                index = 0;
                sudahKetemuHeader = false;


                textBox2.Text = "Data1 = " + dataBuf[1].ToString("x") + "\t Data2 = " + dataBuf[2].ToString("x") + "\r\n";
                // Hasil Kali Data ke-1 dan data ke-2
                int hasil = dataBuf[1] * dataBuf[2];
                textBox2.Text += "Hasil Kali = " + hasil.ToString() + "\r\n";
            }
        }

        void tampilin(object sender, EventArgs e)
        {
            
            //DATA=25;80
            char[] splitter = { '=', ';' };
            string[] dataLine = DataMasuk.Split(splitter);

            textBox2.Text += "Temperature = " + dataLine[1] + " dan Kelembapan = " + dataLine[2];
            // Temp x Kelembapan

            double ujiDouble;
            if (double.TryParse(dataLine[1], out ujiDouble) && double.TryParse(dataLine[2], out ujiDouble)) {

                double T = double.Parse(dataLine[1]);
                double H = double.Parse(dataLine[2]);
                double Hasil = T * H;
                textBox2.Text += "\t" + Hasil.ToString();
            } else
            {
                textBox2.Text += "\tError";
            }
            textBox2.Text += "\r\n";

            dataGridView1.Rows.Add(dataLine[1], dataLine[2]);

            chart1.Series["T"].Points.Add(double.Parse(dataLine[1]));
            chart1.Series["H"].Points.Add(double.Parse(dataLine[2]));

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            dataGridView1.Rows.Clear();
            chart1.Series["T"].Points.Clear();
            chart1.Series["H"].Points.Clear();
        }
    }
}
