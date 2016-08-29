using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;


namespace YXE_DSCREEN
{
    public partial class Form1 : Form
    {
        
        Screen[] screens = System.Windows.Forms.Screen.AllScreens;
        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public Form1()
        {
            InitializeComponent();
            if (videoDevices.Count < 1)
            {
                MessageBox.Show("没有检测到视频源");
            }
            else
            {
                GetVideoSource();
            }
            readConf();
        }

        void readConf()
        {
            ShowInfo showinfo = new ShowInfo();
            String[] cfstr = cfa.AppSettings.Settings.AllKeys;
            foreach (FilterInfo fitem in videoDevices)
            {
                if (ContainKey(fitem.Name))
                {
                    String sitem = cfa.AppSettings.Settings[fitem.Name].Value;

                    for (int j = 0; j < screens.Count(); j++)
                    {
                        if (screens[j].DeviceName == sitem)
                        {
                            showinfo.videoscreen = screens[j];
                        }
                    }
                    for (int j = 0; j < videoDevices.Count; j++)
                    {
                        if (videoDevices[j].Name == fitem.Name)
                        {
                            showinfo.videosource = videoDevices[j];
                        }
                    }

                    ParameterizedThreadStart Parstart = new ParameterizedThreadStart(threadwin);
                    object o = showinfo;
                    Thread threadnew = new Thread(Parstart);
                    threadnew.Start(o);
                }
            }
        }


        bool ContainKey(string key)
        {
            string[] allKey = cfa.AppSettings.Settings.AllKeys;
            foreach (string item in allKey)
            {
                if (item == key)
                {
                    return true;
                }
            }
            return false;
        }

        void GetVideoSource()
        {
            addBuginfo(null);
            addBuginfo("视频源:" + videoDevices.Count.ToString());
            for (int i = 0; i < videoDevices.Count; i++)
            {
                addBuginfo(videoDevices[i].MonikerString);
                addBuginfo(videoDevices[i].Name);
                addBuginfo("----------------------------");
                contextMenuStrip1.Items.Add(videoDevices[i].Name);
            }
            for (int i = 1; i < contextMenuStrip1.Items.Count; i++)
            {
                GetScreenCount(i);
            }
        }

        void GetScreenCount(int ddii)
        {
            addBuginfo("显示屏数：" + screens.Count());
            for (int i = 0; i < screens.Count(); i++)
            {
                addBuginfo("----------------------------");
                addBuginfo("显示屏[" + i.ToString() + "]" );
                addBuginfo(screens[i].WorkingArea.ToString());
                ((ToolStripDropDownItem)contextMenuStrip1.Items[ddii]).DropDownItems.Add(screens[i].DeviceName,null,contextMenuStrip1_ItemClick);
            }
        }

        void contextMenuStrip1_ItemClick(object sender, EventArgs e)
        {
            Screen showin = null;
            FilterInfo showit = null;
            ToolStripItem item = (ToolStripItem)sender;
            for (int j = 0; j < screens.Count(); j++)
            {
                if (screens[j].DeviceName == item.Text)
                {
                    showin = screens[j];
                }
            }
            for (int j = 0; j < videoDevices.Count; j++)
            {
                if (videoDevices[j].Name == item.OwnerItem.Text)
                {
                    showit = videoDevices[j];
                }
            }
            //MessageBox.Show("SHOW:" + showin.DeviceName + "\r\n" + "INPUT:" + showit.Name);
            ShowInfo showinfo = new ShowInfo();
            showinfo.videoscreen = showin;
            showinfo.videosource = showit;

            cfa.AppSettings.Settings.Add(showit.Name,showin.DeviceName);
            cfa.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            ParameterizedThreadStart Parstart = new ParameterizedThreadStart(threadwin);
            object o = showinfo;
            Thread threadnew = new Thread(Parstart);
            threadnew.Start(o);

        }

        void threadwin(object parainfo)
        {
            ShowInfo ss = (ShowInfo)parainfo;
            showdev showshow = new showdev(ss.videoscreen, ss.videosource);
            Size s = new Size(ss.videoscreen.WorkingArea.Width, ss.videoscreen.WorkingArea.Height);
            showshow.StartPosition = FormStartPosition.Manual;
            showshow.Size = s;
            showshow.Location = new Point(ss.videoscreen.WorkingArea.X, ss.videoscreen.WorkingArea.Y);
            showshow.ShowDialog();
        }

        void addBuginfo(String infostr)
        {
            DEBUGWIN.Text += infostr + "\r\n";
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    class ShowInfo
    {
        public FilterInfo videosource;
        public Screen videoscreen;
    }
}
