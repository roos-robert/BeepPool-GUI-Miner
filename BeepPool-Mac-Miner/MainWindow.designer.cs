// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BeepPool_Mac_Miner
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSButton button1 { get; set; }

		[Outlet]
		AppKit.NSButton button2 { get; set; }

		[Outlet]
		AppKit.NSTextView consoleControl1 { get; set; }

		[Outlet]
		AppKit.NSTextField label1 { get; set; }

		[Outlet]
		AppKit.NSTextField label2 { get; set; }

		[Outlet]
		AppKit.NSTextField label3 { get; set; }

		[Outlet]
		AppKit.NSTextField label4 { get; set; }

		[Outlet]
		AppKit.NSTextField label5 { get; set; }

		[Outlet]
		AppKit.NSButton pictureBox1 { get; set; }

		[Outlet]
		AppKit.NSTextField textBox1 { get; set; }

		[Outlet]
		AppKit.NSTextField textBox2 { get; set; }

		[Outlet]
		AppKit.NSTextField textBox3 { get; set; }

		[Action ("button1_Click:")]
		partial void button1_Click (AppKit.NSButton sender);

		[Action ("button2_Click:")]
		partial void button2_Click (AppKit.NSButton sender);

		[Action ("pictureBox1_Click:")]
		partial void pictureBox1_Click (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (button1 != null) {
				button1.Dispose ();
				button1 = null;
			}

			if (button2 != null) {
				button2.Dispose ();
				button2 = null;
			}

			if (consoleControl1 != null) {
				consoleControl1.Dispose ();
				consoleControl1 = null;
			}

			if (label1 != null) {
				label1.Dispose ();
				label1 = null;
			}

			if (label2 != null) {
				label2.Dispose ();
				label2 = null;
			}

			if (label3 != null) {
				label3.Dispose ();
				label3 = null;
			}

			if (label4 != null) {
				label4.Dispose ();
				label4 = null;
			}

			if (label5 != null) {
				label5.Dispose ();
				label5 = null;
			}

			if (pictureBox1 != null) {
				pictureBox1.Dispose ();
				pictureBox1 = null;
			}

			if (textBox1 != null) {
				textBox1.Dispose ();
				textBox1 = null;
			}

			if (textBox2 != null) {
				textBox2.Dispose ();
				textBox2 = null;
			}

			if (textBox3 != null) {
				textBox3.Dispose ();
				textBox3 = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
