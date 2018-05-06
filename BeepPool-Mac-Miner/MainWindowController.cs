using System;
using Foundation;
using AppKit;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using CoreGraphics;

namespace BeepPool_Mac_Miner
{
	public partial class MainWindowController : NSWindowController
	{
		static Process process;

		// Called when created from unmanaged code
		public MainWindowController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Call to load from the XIB/NIB file
		public MainWindowController() : base("MainWindow")
		{
			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{
		}
		
		[Export("awakeFromNib")]
		public void AwakeFromNib()
		{
			try
			{
				string[] lines = System.IO.File.ReadAllLines(@"settings.txt");

				if (lines.Length == 3)
				{
					textBox3.StringValue = lines[0];
					textBox1.StringValue = lines[1];
					textBox2.StringValue = lines[2];
				}
			}
			catch
			{

			}
		}

		partial void button1_Click(NSButton sender)
		{
			var wallet = textBox3.StringValue;
			var cores = textBox1.StringValue;
			var worker = textBox2.StringValue;

			var startInfo = new ProcessStartInfo
			{
				FileName = "miner",
				Arguments = String.Format("--miner={0} --wallet-address=\"{1}\" --extra-data=\"{2}\"", cores, wallet, worker),
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				ErrorDialog = false
			};

			process = Process.Start(startInfo);

			process.OutputDataReceived += console_outputReceived;
			process.ErrorDataReceived += console_outputReceived;

			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			using (StreamWriter bw = new StreamWriter(File.Create("settings.txt")))
			{
				bw.WriteLine(wallet);
				bw.WriteLine(cores);
				bw.WriteLine(worker);
			}

			button2.Enabled = true;
			button1.Enabled = false;
		}

		partial void pictureBox1_Click(NSButton sender)
		{
			NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl("https://beeppool.org/"));
		}

		partial void button2_Click(NSButton sender)
		{
			process?.Kill();
			button2.Enabled = false;
			button1.Enabled = true;
		}

		void console_outputReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				InvokeOnMainThread(() =>
				{
					consoleControl1.TextStorage.MutableString.Append((NSString)(e.Data + Environment.NewLine));
					consoleControl1.ScrollRangeToVisible(new NSRange(consoleControl1.String.Length, 0));
				});
			}
		}

		//strongly typed window accessor
		public new MainWindow Window
		{
			get
			{
				return (MainWindow)base.Window;
			}
		}

		public bool InvokeRequired => throw new NotImplementedException();
	}
}
