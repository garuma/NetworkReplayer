using System;
using System.IO;
using System.Linq;
using System.Net;

using Mono.Options;

namespace NetworkReplayer
{
	class MainClass
	{
		enum Mode {
			Record,
			Replay
		}

		public static void Main (string[] args)
		{
			// House keeping
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

			int port = 8080;
			var op = Mode.Replay;
			string basePath = Path.Combine (Directory.GetCurrentDirectory (), "replay_cache");

			var options = new OptionSet {
				{ "record", v => op = Mode.Record },
				{ "replay", v => op = Mode.Replay },
				{ "port=|p=", v => int.TryParse (v, out port) }
			};
			var rest = options.Parse (args);
			var path = rest.FirstOrDefault ();
			if (!string.IsNullOrEmpty (path))
				basePath = path;
			if (!Directory.Exists (basePath))
				Directory.CreateDirectory (basePath);

			var listener = new HttpListener ();
			listener.Prefixes.Add ("http://+:" + port + "/");
			listener.Start ();

			Console.WriteLine ("Mode: {0}", op.ToString ());
			Console.WriteLine ("Listening on port {0}", port.ToString ());

			var action = op == Mode.Replay ?
				(Action)new Replayer (listener, basePath).Action : (Action)new Recorder (listener, basePath).Action;
			action ();
		}
	}
}
