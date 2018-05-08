using System;
using Foundation;
using AppKit;
using System.IO;
using System.Diagnostics;

namespace BeepPool_Mac_Miner
{
	public partial class MainWindowController : NSWindowController
	{
		static Process minerProcess;
		static string minerCli;
		static bool mining;

		//strongly typed window accessor
		public new MainWindow Window
		{
			get
			{
				return (MainWindow)base.Window;
			}
		}

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
		public override void AwakeFromNib()
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
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				PrintToConsoleOutput($"Error: Could not find 'settings.txt'");
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
						StartMining();
					}
				});
			}
			else
			{
				StartMining();
			}
		}

		partial void button2_Click(NSButton sender)
		{
			StopMining();
		}

		partial void pictureBox1_Click(NSButton sender)
		{
			NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl("https://beeppool.org/"));
		}

		void console_outputReceived(object sender, DataReceivedEventArgs e)
		{
			if (e.Data != null)
			{
				InvokeOnMainThread(() =>
				{
					if (!mining)
					{
						if (!minerProcess.HasExited)
						{
							minerProcess.WaitForExit(500);
						}

						if (!minerProcess.HasExited)
						{
							PrintToConsoleOutput($"Started mining...");
						}

						mining = true;
					}
					PrintToConsoleOutput(e.Data);
				});
			}
		}

		void console_exited(object sender, EventArgs e)
		{
			InvokeOnMainThread(() =>
			{
				StopMining();
			});
		}

		void StartMining()
		{
			var wallet = textBox3.StringValue;
			var cores = textBox1.StringValue;
			var worker = textBox2.StringValue;

			var workingDirectory = Path.GetDirectoryName(minerCli);

			var startInfo = new ProcessStartInfo
			{
				FileName = minerCli,
				WorkingDirectory = workingDirectory,
				Arguments = String.Format("--miner={0} --wallet-address=\"{1}\" --extra-data=\"{2}\"", cores, wallet, worker),
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				ErrorDialog = false
			};
			startInfo.Environment["PATH"] = "/usr/bin:/bin:/usr/sbin:/sbin:/usr/local/bin";

			try
			{
				minerProcess = Process.Start(startInfo);

				minerProcess.OutputDataReceived += console_outputReceived;
				minerProcess.ErrorDataReceived += console_outputReceived;
				minerProcess.EnableRaisingEvents = true;
				minerProcess.Exited += console_exited;

				minerProcess.BeginOutputReadLine();
				minerProcess.BeginErrorReadLine();

				using (StreamWriter bw = new StreamWriter(File.Create("settings.txt")))
				{
					bw.WriteLine(wallet);
					bw.WriteLine(cores);
					bw.WriteLine(worker);
					bw.WriteLine(minerCli);
				}

				button1.Enabled = false;
				button2.Enabled = true;
				textBox3.Enabled = false;
				textBox1.Enabled = false;
				textBox2.Enabled = false;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				PrintToConsoleOutput($"Error: Could not find CLI miner at path '{minerCli}'");
			}
		}

		void StopMining()
		{
			if (mining)
			{
				mining = false;
				
				if (minerProcess != null && !minerProcess.HasExited)
				{
					KillProcessTree(minerProcess);
					PrintToConsoleOutput($"Stopped mining.");
				}
			}
			button1.Enabled = true;
			button2.Enabled = false;
			textBox3.Enabled = true;
			textBox1.Enabled = true;
			textBox2.Enabled = true;
		}

		static void KillProcessTree(Process process)
		{
			var startInfo = new ProcessStartInfo
			{
				FileName = "bash",
				Arguments = String.Format("-c \"ps -ef | grep {0} | grep -v grep | awk '{{print $2}}' | grep -v {0}\"", process.Id),
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				ErrorDialog = false
			};

			var p = Process.Start(startInfo);
			p.Start();
			p.WaitForExit();
			string output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();

			var split = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var subprocessIds = Array.ConvertAll(split, s => int.Parse(s));

			foreach (var subprocessId in subprocessIds)
			{
				KillProcessTree(Process.GetProcessById(subprocessId));
			}
			process.Kill();
		}

		void PrintToConsoleOutput(string message)
		{
			consoleControl1.TextStorage.MutableString.Append((NSString)(message + Environment.NewLine));
			consoleControl1.ScrollRangeToVisible(new NSRange(consoleControl1.String.Length, 0));
		}
	}
}
