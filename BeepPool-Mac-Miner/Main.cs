using AppKit;

namespace BeepPool_Mac_Miner
{
	class MainClass
	{
	    /// <summary>
        /// The main entry point for the application.
        /// </summary>
		static void Main (string[] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}