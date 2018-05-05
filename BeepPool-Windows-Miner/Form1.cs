using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeepPool_Windows_Miner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(@"settings.txt");

                if (lines.Length == 3)
                {
                    textBox3.Text = lines[0];
                    textBox1.Text = lines[1];
                    textBox2.Text = lines[2];
                }
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var wallet = textBox3.Text;
            var cores = textBox1.Text;
            var worker = textBox2.Text;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"miner.exe";
            startInfo.Arguments = String.Format("miner --miner={0} --wallet-address=\"{1}\" --extra-data=\"{2}\"", cores, wallet, worker);
            Process.Start(startInfo);
            consoleControl1.StartProcess(startInfo.FileName, startInfo.Arguments);           

            using (StreamWriter bw = new StreamWriter(File.Create("settings.txt")))
            {
                bw.WriteLine(wallet);
                bw.WriteLine(cores);
                bw.WriteLine(worker);
                bw.Close();
            }

            button2.Enabled = true;
            button1.Enabled = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://beeppool.org/");
            Process.Start(sInfo);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            consoleControl1.StopProcess();
            button1.Enabled = true;
        }
    }
}
