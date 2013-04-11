using System;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace NetworkReplayer
{
	public class Replayer
	{
		HttpListener listener;
		string basePath;

		public Replayer (HttpListener listener, string basePath)
		{
			this.listener = listener;
			this.basePath = basePath;
		}

		public void Action ()
		{
			while (true) {
				var ctx = listener.GetContext ();
				var req = ctx.Request;

				var originalUrl = req.Headers [RequestUtils.ForwardToHeader];
				if (string.IsNullOrEmpty (originalUrl))
					continue;

				var remoteUrl = originalUrl + req.RawUrl;
				Console.WriteLine ("Replaying {0}", remoteUrl);

				var filePath = Path.Combine (basePath, RequestUtils.ToFileName (originalUrl, req));
				var resp = ctx.Response;

				if (!File.Exists (filePath)) {
					resp.StatusCode = 404;
					resp.Close ();
				} else {
					using (var output = resp.OutputStream)
						using (var input = new BufferedStream (File.OpenRead (filePath)))
							input.CopyTo (output);
				}
			}
		}
	}
}

