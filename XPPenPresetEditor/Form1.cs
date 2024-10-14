using SignAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XPPenPresetEditor
{
    public partial class Form1 : Form
    {
        private int HandleDataProc = 0;
        private int HandleNotifyProc = 0;
        Graphics g;
        Bitmap bmp;
        public Form1()
        {
            InitializeComponent();
            InitializeDevice();
        }

        private void InitializeDevice()
        {
            RECT screenRect = new RECT();
            int errorCode;
            errorCode = Win32SignAPI.signGetDeviceStatus();
            if (errorCode == (int)ErrorCode.ERR_OK)
            {
                errorCode = Win32SignAPI.signOpenDevice();
                if (errorCode == (int)ErrorCode.ERR_OK)
                {
                    errorCode = Win32SignAPI.signGetScreenRect(ref screenRect);
                    if (errorCode == (int)ErrorCode.ERR_OK)
                    {
                        numericUpDown1.Value = screenRect.left;
                        numericUpDown2.Value = screenRect.top;
                        numericUpDown3.Value = screenRect.Width;
                        numericUpDown4.Value = screenRect.Height;
                    }
                    else
                        MessageBox.Show($"Error: {(ErrorCode)errorCode}");
                }
                else
                    MessageBox.Show($"Error: {(ErrorCode)errorCode}");
            }
            else
                MessageBox.Show($"Error: {(ErrorCode)errorCode}");

            var screenBounds = Screen.FromControl(this).Bounds;
            numericUpDown5.Value = screenBounds.X;
            numericUpDown6.Value = screenBounds.Y;
            numericUpDown7.Value = screenBounds.Width;
            numericUpDown8.Value = screenBounds.Height;

            DrawScreen();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RECT screenRect = new RECT();
            if (Win32SignAPI.signGetScreenRect(ref screenRect) == (int)ErrorCode.ERR_OK)
            {
                screenRect.left = Convert.ToInt32(numericUpDown1.Value);
                screenRect.right = Convert.ToInt32(numericUpDown1.Value);
                screenRect.left = Convert.ToInt32(numericUpDown1.Value);
                screenRect.left = Convert.ToInt32(numericUpDown1.Value);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int errorCode = Win32SignAPI.signGetDeviceStatus();
            if (errorCode == (int)ErrorCode.ERR_OK)
            {
                TABLET_DEVICEINFO deviceInfo = new TABLET_DEVICEINFO();
                Win32SignAPI.signGetDeviceInfo(ref deviceInfo);
                var vendor = new string(deviceInfo.vendor);
                var product = new string(deviceInfo.product);
                MessageBox.Show($"Vendor: {vendor}\nProduct: {product}\n");
            }
            else
                MessageBox.Show($"Error: {(ErrorCode)errorCode}");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HandleDataProc > 0)
                Win32SignAPI.signUnregisterDataCallBack(HandleDataProc);
            if (HandleNotifyProc > 0)
                Win32SignAPI.signUnregisterDevNotifyCallBack(HandleNotifyProc);

            Win32SignAPI.signCloseDevice();
        }

        private void DrawScreen()
        {
            Size offset = new Size(25, 25);
            Brush lowAlphaBrush = new SolidBrush(Color.FromArgb(100, Color.Gray));
            var screenBounds = Screen.FromControl(this).Bounds;
            Bitmap bmp = new Bitmap(screenBounds.Width + offset.Width * 2, screenBounds.Height * 2 + offset.Height * 2);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(screenBounds.X, screenBounds.Y, 25, 25, screenBounds.Size);
            g.DrawRectangle(new Pen(Color.LightBlue, 10), screenBounds.X + offset.Width, screenBounds.Y + offset.Height, screenBounds.Width, screenBounds.Height);
            g.FillRectangle(lowAlphaBrush, (int)numericUpDown5.Value + offset.Width, (int)numericUpDown6.Value + offset.Height, (int)numericUpDown7.Value, (int)numericUpDown8.Value);

            offset = new Size(screenBounds.Width / 4, screenBounds.Height + 100);
            g.DrawRectangle(new Pen(Color.LightBlue, 10), screenBounds.X / 2 + offset.Width, screenBounds.Y / 2 + offset.Height, screenBounds.Width / 2, screenBounds.Height / 2);
            g.FillRectangle(lowAlphaBrush, (int)numericUpDown1.Value / 2 + offset.Width, (int)numericUpDown2.Value / 2 + offset.Height, (int)numericUpDown3.Value / 2, (int)numericUpDown4.Value / 2);

            Bitmap resizedBmp = new Bitmap(bmp, new Size(pictureBox1.Width, pictureBox1.Height));
            pictureBox1.Image = resizedBmp;
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            var screenBounds = Screen.FromControl(this).Bounds;
            numericUpDown3.Value = Math.Min(screenBounds.Width - numericUpDown1.Value, numericUpDown3.Value);
            numericUpDown4.Value = Math.Min(screenBounds.Height - numericUpDown2.Value, numericUpDown4.Value);
            numericUpDown7.Value = Math.Min(screenBounds.Width - numericUpDown5.Value, numericUpDown7.Value);
            numericUpDown8.Value = Math.Min(screenBounds.Height - numericUpDown6.Value, numericUpDown8.Value);
            DrawScreen();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            DrawScreen();
        }
    }
}
