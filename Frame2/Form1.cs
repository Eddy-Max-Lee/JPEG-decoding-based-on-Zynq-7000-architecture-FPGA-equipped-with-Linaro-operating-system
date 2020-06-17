using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//指示詞
using System.IO.Ports;
using System.Threading;

namespace Frame2
{
    public partial class Form1 : Form
    {
        //console 參數
        private SerialPort My_SerialPort;
        private bool Console_receiving = false;
        private Thread t;
        private int index=0;
        //使用委派顯示 Console 畫面
        delegate void Display(string buffer);

        public Form1()
        {
            InitializeComponent();
        }
        public void ConsoleShow(string buffer)
        {
            Console_Input.AppendText(buffer);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //連接 Console
            //Console_Connect("COM12", 115200);
            Console_Input.Text = "尚未連結COM";
            SerialPort serialPort1 = new SerialPort();
            String[] portnames = SerialPort.GetPortNames();
            foreach (var item in portnames)
            {
                combobox1.Items.Add(item);
            }
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            //執行命令
            if (Console_Output.Text.Contains("./ffjpeg"))
            {
                Console_Input.Text = "";
            }
            //送出字串
            SendData(Encoding.ASCII.GetBytes(Console_Output.Text + "\r"));

            //


        }


        //連接 Console
        public void Console_Connect(string COM, Int32 baud)
        {
            try
            {
                My_SerialPort = new SerialPort();

                if (My_SerialPort.IsOpen)
                {
                    My_SerialPort.Close();
                }

                //設定 Serial Port 參數
                My_SerialPort.PortName = COM;
                My_SerialPort.BaudRate = baud;
                My_SerialPort.DataBits = 8;
                My_SerialPort.StopBits = StopBits.One;

                if (!My_SerialPort.IsOpen)
                {
                    //開啟 Serial Port
                    My_SerialPort.Open();

                    Console_receiving = true;

                    //開啟執行續做接收動作
                    t = new Thread(DoReceive);
                    t.IsBackground = true;
                    t.Start();
                    Console_Input.Text = "";
                    Console_Input.Text = "連結成功";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //關閉 Console
        public void CloseComport()
        {
            try
            {
                My_SerialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Console 接收資料
        private void DoReceive()
        {
            Byte[] buffer = new Byte[1024];

            try
            {
                while (Console_receiving)
                {
                    if (My_SerialPort.BytesToRead > 0)
                    {
                        Int32 length = My_SerialPort.Read(buffer, 0, buffer.Length);

                        string buf = Encoding.ASCII.GetString(buffer);

                        Array.Resize(ref buffer, length);
                        Display d = new Display(ConsoleShow);
                        this.Invoke(d, new Object[] { buf });
                        Array.Resize(ref buffer, 1024);
                    }

                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Console 發送資料
        public void SendData(Object sendBuffer)
        {
            if (sendBuffer != null)
            {
                Byte[] buffer = sendBuffer as Byte[];

                try
                {
                    My_SerialPort.Write(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    CloseComport();
                    MessageBox.Show(ex.Message);
                }
                buffer = null;
            }
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console_receiving = false;
            //關閉 Serial Port
            CloseComport();
        }

        // btn_get_small_image
        private void btn_get_image_Click(object sender, EventArgs e)
        {
            // btn_get_small_image
            //送出字串
            Console_Input.Text = "";
            Thread.Sleep(500);
            SendData(Encoding.ASCII.GetBytes("cd User_App/ff-s2/src"+"\r"));
            Thread.Sleep(500);

            SendData(Encoding.ASCII.GetBytes("./ffjpeg -d "+ TB_jpgName.Text +".jpg"+ "\r"));
            Thread.Sleep(1000);
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String console_sub;
            String w_sub;
            String h_sub;
            String time_sub;
            String image_sub;
            Bitmap frame;
         
            Console_Input.Text = Console_Input.Text.Substring(Console_Input.Text.IndexOf("root"));
            w_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("{") + 1, Console_Input.Text.IndexOf("}") - Console_Input.Text.IndexOf("{") - 1);
            TB_W.Text = w_sub;
            h_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("(") + 1, Console_Input.Text.IndexOf(")") - Console_Input.Text.IndexOf("(") - 1);
            TB_H.Text = h_sub;

            image_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("[") + 1, Console_Input.Text.IndexOf("]") - Console_Input.Text.IndexOf("[") - 1);
            List<string> each_pixel = new List<string>(image_sub.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

            time_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("*") + 1, Console_Input.Text.IndexOf("+") - Console_Input.Text.IndexOf("*") - 1);
            TB_T.Text = time_sub;
            frame = new Bitmap(Convert.ToInt32(w_sub), Convert.ToInt32(h_sub));

            int x_count = 0;
            int y_count = 0;
            foreach (string element in each_pixel)
            {
                Color color = new Color();

                try
                {
                    /*
                    if (element.Contains("R") && element.Contains("G") && element.Length <= 11 && Convert.ToInt32(element.Substring(0, element.IndexOf("R"))) <= 255 && Convert.ToInt32(element.Substring(element.IndexOf("R") + 1, element.IndexOf("G") - element.IndexOf("R") - 1)) <= 255 && Convert.ToInt32(element.Substring(element.IndexOf("G") + 1)) <= 255 && element.Substring(0, element.IndexOf("R")) != "" && (element.IndexOf("G") - element.IndexOf("R")) != 0 && element.Substring(element.IndexOf("G") + 1) != "")
                    {

                        Byte R = Convert.ToByte(element.Substring(0, element.IndexOf("R")));
                        Byte G = Convert.ToByte(element.Substring(element.IndexOf("R") + 1, element.IndexOf("G") - element.IndexOf("R") - 1));
                        Byte B = Convert.ToByte(element.Substring(element.IndexOf("G") + 1));
                        color = Color.FromArgb(R, G, B);



                    }
                    else if (element.Length > 11)
                    {
                        x_count++;
                        color = Color.FromArgb(253, 248, 228);
                    }
                    else
                    {
                        color = Color.FromArgb(253, 248, 228);
                    }*/
                    Byte R = Convert.ToByte(element.Substring(0, element.IndexOf("R")));
                    Byte G = Convert.ToByte(element.Substring(element.IndexOf("R") + 1, element.IndexOf("G") - element.IndexOf("R") - 1));
                    Byte B = Convert.ToByte(element.Substring(element.IndexOf("G") + 1));
                    color = Color.FromArgb(R, G, B);

                }
                catch (Exception ex)
                {
                    //  CloseComport();
                    //  MessageBox.Show(ex.Message);
                    color = Color.FromArgb(253, 248, 228);
                    x_count++;
                }


                    if (y_count < Convert.ToInt32(h_sub))
                    {

                        if (x_count < Convert.ToInt32(w_sub)) frame.SetPixel(x_count, y_count, color);
                    }
        
                x_count++;
                if (x_count == Convert.ToInt32(w_sub))
                {
                    x_count = 0;
                    y_count++;
                }

                // Console.WriteLine($"Element #: {element}");
            }
            pictureBox1.Image = frame;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendData(Encoding.ASCII.GetBytes("^c" + "\r"));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Console_Input.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // btn_get_small_image
            //送出字串
            Console_Input.Text = "";
            Thread.Sleep(1000);
            SendData(Encoding.ASCII.GetBytes("cd /" + "\r"));
            Thread.Sleep(500);
            SendData(Encoding.ASCII.GetBytes("cd root" + "\r"));
            Thread.Sleep(500);

            SendData(Encoding.ASCII.GetBytes("./User_App/ff-s2/src/ffjpeg -d " + TB_jpgName.Text + ".jpg" + "\r"));
            Thread.Sleep(1000);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            String time_sub;
            time_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("*") + 1, Console_Input.Text.IndexOf("+") - Console_Input.Text.IndexOf("*") - 1);
            TB_T.Text = time_sub;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Console_Input.Text = "";
            Thread.Sleep(1000);
            SendData(Encoding.ASCII.GetBytes("cd User_App" + "\r"));
            Thread.Sleep(1000);
            SendData(Encoding.ASCII.GetBytes("insmod dct.ko" + "\r"));
            Thread.Sleep(500);
            SendData(Encoding.ASCII.GetBytes("insmod zz_4.ko" + "\r"));
            Thread.Sleep(500);
            SendData(Encoding.ASCII.GetBytes("insmod bitstr.ko" + "\r"));
            Thread.Sleep(500);
            SendData(Encoding.ASCII.GetBytes("cd ff-h3/src" + "\r"));
            Thread.Sleep(500);
            //SendData(Encoding.ASCII.GetBytes("make clean" + "\r"));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Console_Input.Text = "";
            Thread.Sleep(1000);
            SendData(Encoding.ASCII.GetBytes("make" + "\r"));
            Thread.Sleep(1000);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String console_sub;
            String w_sub;
            String h_sub;
            String time_sub;
            String image_sub;
            Bitmap frame;
            // btn_get_small_image
            //送出字串
            /*  
             Console_Input.Text = "";
              Thread.Sleep(500);
              SendData(Encoding.ASCII.GetBytes("cd User_App/ff-s2/src" + "\r"));
              Thread.Sleep(500);

              SendData(Encoding.ASCII.GetBytes("./ffjpeg -d " + TB_jpgName.Text + ".jpg" + "\r"));
              Thread.Sleep(500);
              Thread.Sleep(500);
              */
            try
            {

                Console_Input.Text = Console_Input.Text.Substring(Console_Input.Text.IndexOf("root"));
                w_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("{") + 1, Console_Input.Text.IndexOf("}") - Console_Input.Text.IndexOf("{") - 1);
                TB_W.Text = w_sub;
                h_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("(") + 1, Console_Input.Text.IndexOf(")") - Console_Input.Text.IndexOf("(") - 1);
                TB_H.Text = h_sub;

                image_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("[") + 1, Console_Input.Text.IndexOf("]") - Console_Input.Text.IndexOf("[") - 1);
                List<string> each_pixel = new List<string>(image_sub.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                time_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("*") + 1, Console_Input.Text.IndexOf("+") - Console_Input.Text.IndexOf("*") - 1);
                TB_T.Text = time_sub;
                frame = new Bitmap(Convert.ToInt32(w_sub), Convert.ToInt32(h_sub));

                int x_count = 0;
                int y_count = 0;
                foreach (string element in each_pixel)
                {
                    Color color = new Color();

                    try
                    {

                        Byte R = Convert.ToByte(element.Substring(0, element.IndexOf("R")));
                        Byte G = Convert.ToByte(element.Substring(element.IndexOf("R") + 1, element.IndexOf("G") - element.IndexOf("R") - 1));
                        Byte B = Convert.ToByte(element.Substring(element.IndexOf("G") + 1));
                        color = Color.FromArgb(R, G, B);

                    }
                    catch (Exception ex)
                    {
                        //  CloseComport();
                        //  MessageBox.Show(ex.Message);
                        color = Color.FromArgb(253, 248, 228);
                        x_count++;
                    }


                    if (y_count < Convert.ToInt32(h_sub))
                    {

                        if (x_count < Convert.ToInt32(w_sub)) frame.SetPixel(x_count, y_count, color);
                    }

                    x_count++;
                    if (x_count == Convert.ToInt32(w_sub))
                    {
                        x_count = 0;
                        y_count++;
                    }

                    // Console.WriteLine($"Element #: {element}");
                }
                pictureBox1.Image = frame;

                Console_Input.Text = "";
                TB_jpgName.Text = "p" + index;
                index++;
                button3.Visible = false;
                button3.Visible = true;
            }
            catch (Exception ex) { 
            
            
            }
        }
       
        private void button3_MouseHover(object sender, EventArgs e)
        {
            //  Console_Input.Text = "";
            // Thread.Sleep(500);
            button3.Text = "...";
            button3.Enabled = false;
            SendData(Encoding.ASCII.GetBytes("cd User_App/ff-s2/src" + "\r"));
            Thread.Sleep(500);

            SendData(Encoding.ASCII.GetBytes("./ffjpeg -d " + TB_jpgName.Text + ".jpg" + "\r"));
            Thread.Sleep(500);
              Thread.Sleep(500);
            button3.Text = "▶";
            button3.Enabled = true;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {           
           /* Console_Input.Text = "";
            TB_jpgName.Text="p"+ index;
            index++;*/
        }

        private void Leftbutton_Click(object sender, EventArgs e)
        {
            String console_sub;
            String w_sub;
            String h_sub;
            String time_sub;
            String image_sub;
            Bitmap frame;
            // btn_get_small_image
            //送出字串
            /*  
             Console_Input.Text = "";
              Thread.Sleep(500);
              SendData(Encoding.ASCII.GetBytes("cd User_App/ff-s2/src" + "\r"));
              Thread.Sleep(500);

              SendData(Encoding.ASCII.GetBytes("./ffjpeg -d " + TB_jpgName.Text + ".jpg" + "\r"));
              Thread.Sleep(500);
              Thread.Sleep(500);
              */
            try
            {

                Console_Input.Text = Console_Input.Text.Substring(Console_Input.Text.IndexOf("root"));
                w_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("{") + 1, Console_Input.Text.IndexOf("}") - Console_Input.Text.IndexOf("{") - 1);
                TB_W.Text = w_sub;
                h_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("(") + 1, Console_Input.Text.IndexOf(")") - Console_Input.Text.IndexOf("(") - 1);
                TB_H.Text = h_sub;

                image_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("[") + 1, Console_Input.Text.IndexOf("]") - Console_Input.Text.IndexOf("[") - 1);
                List<string> each_pixel = new List<string>(image_sub.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                time_sub = Console_Input.Text.Substring(Console_Input.Text.IndexOf("*") + 1, Console_Input.Text.IndexOf("+") - Console_Input.Text.IndexOf("*") - 1);
                TB_T.Text = time_sub;
                frame = new Bitmap(Convert.ToInt32(w_sub), Convert.ToInt32(h_sub));

                int x_count = 0;
                int y_count = 0;
                foreach (string element in each_pixel)
                {
                    Color color = new Color();

                    try
                    {

                        Byte R = Convert.ToByte(element.Substring(0, element.IndexOf("R")));
                        Byte G = Convert.ToByte(element.Substring(element.IndexOf("R") + 1, element.IndexOf("G") - element.IndexOf("R") - 1));
                        Byte B = Convert.ToByte(element.Substring(element.IndexOf("G") + 1));
                        color = Color.FromArgb(R, G, B);

                    }
                    catch (Exception ex)
                    {
                        //  CloseComport();
                        //  MessageBox.Show(ex.Message);
                        color = Color.FromArgb(253, 248, 228);
                        x_count++;
                    }


                    if (y_count < Convert.ToInt32(h_sub))
                    {

                        if (x_count < Convert.ToInt32(w_sub)) frame.SetPixel(x_count, y_count, color);
                    }

                    x_count++;
                    if (x_count == Convert.ToInt32(w_sub))
                    {
                        x_count = 0;
                        y_count++;
                    }

                    // Console.WriteLine($"Element #: {element}");
                }
                pictureBox1.Image = frame;
                button3.Visible = false;
                button3.Visible = true;
            }
            catch (Exception ex)
            {


            }
        }

        private void Leftbutton_Leave(object sender, EventArgs e)
        {
            Console_Input.Text = "";
            TB_jpgName.Text = "p" + index;
            index--;
        }

        private void Connectbutton_Click(object sender, EventArgs e)
        {
            Console_Connect(combobox1.Text, 115200);
        }
    }
}
