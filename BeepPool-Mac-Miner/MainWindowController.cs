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
		static string minerCli;

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

				if (lines.Length == 4)
				{
					textBox3.StringValue = lines[0];
					textBox1.StringValue = lines[1];
					textBox2.StringValue = lines[2];
					minerCli = lines[3];
				}
			}
			catch
			{

			}

			consoleControl1.Font = NSFont.FromFontName("Courier", 11);
		}

		partial void button1_Click(NSButton sender)
		{
			if (string.IsNullOrEmpty(minerCli))
			{
				var openPanel = NSOpenPanel.OpenPanel;

				openPanel.BeginSheet(Window, (result) =>
				{
					if (openPanel.Url != null)
					{
						minerCli = openPanel.Url.Path;
					}
				});
			}

			var wallet = textBox3.StringValue;
			var cores = textBox1.StringValue;
			var worker = textBox2.StringValue;

			var startInfo = new ProcessStartInfo
			{
				FileName = minerCli,
				Arguments = String.Format("--miner={0} --wallet-address=\"{1}\" --extra-data=\"{2}\"", cores, wallet, worker),
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				ErrorDialog = false
			};

			try
			{
				process = Process.Start(startInfo);

				process.OutputDataReceived += console_outputReceived;
				process.ErrorDataReceived += console_outputReceived;
				process.EnableRaisingEvents = true;
				process.Exited += console_exited;

				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				using (StreamWriter bw = new StreamWriter(File.Create("settings.txt")))
				{
					bw.WriteLine(wallet);
					bw.WriteLine(cores);
					bw.WriteLine(worker);
					bw.WriteLine(minerCli);
				}

				button2.Enabled = true;
				button1.Enabled = false;
			}
			catch
			{
				consoleControl1.TextStorage.MutableString.Append((NSString)($"Error: Could not find CLI miner at path '{minerCli}'" + Environment.NewLine));
				consoleControl1.ScrollRangeToVisible(new NSRange(consoleControl1.String.Length, 0));
			}
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

		void console_exited(object sender, EventArgs e)
		{
			InvokeOnMainThread(() =>
			{
				button2.Enabled = false;
				button1.Enabled = true;
			});
		}

		//strongly typed window accessor
		public new MainWindow Window
		{
			get
			{
				return (MainWindow)base.Window;
			}
		}
	}
}
