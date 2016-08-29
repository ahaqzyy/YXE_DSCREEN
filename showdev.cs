using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using AForge.Imaging;

namespace YXE_DSCREEN
{
    public partial class showdev : Form
    {
        VideoCaptureDevice vvs = null;
        int VideoResSelect = 0;

        public showdev(Screen showscreen,FilterInfo showdevice)
        {
            InitializeComponent();
            this.Text += ":" + showdevice.Name;
            if (videoSourcePlayer1.VideoSource != null)
            {
                videoSourcePlayer1.SignalToStop();
                videoSourcePlayer1.WaitForStop();
            }
            vvs = new VideoCaptureDevice(showdevice.MonikerString);
            vvs.VideoResolution = vvs.VideoCapabilities[VideoResSelect];
            OpenSource(vvs);
        }

        private void OpenSource(IVideoSource source)
        {
            videoSourcePlayer1.SignalToStop();
            videoSourcePlayer1.WaitForStop();
            videoSourcePlayer1.VideoSource = source;
            videoSourcePlayer1.Start();
        }

        private void videoSourcePlayer1_Click(object sender, EventArgs e)
        {
            int allcat = vvs.VideoCapabilities.Count();
            if (VideoResSelect < allcat - 1)
            {
                VideoResSelect += 1;
            }
            else
            {
                VideoResSelect = 0;
            }
            vvs.VideoResolution = vvs.VideoCapabilities[VideoResSelect];
        }

        private void showdev_FormClosing(object sender, FormClosingEventArgs e)
        {
            videoSourcePlayer1.Stop();
            videoSourcePlayer1.VideoSource = null;
        }
    }
}
