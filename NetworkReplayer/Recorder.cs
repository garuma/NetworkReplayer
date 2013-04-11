using System;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace NetworkReplayer
{
	public class Recorder
	{
		HttpListener listener;
		string basePath;

		public Recorder (HttpListener listener, string basePath)
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
				Console.WriteLine ("Recording {0}", remoteUrl);

				var filePath = Path.Combine (basePath, RequestUtils.ToFileName (originalUrl, req));
				var remoteReq = (HttpWebRequest)WebRequest.Create (remoteUrl);
				// Add the headers we can use
				foreach (var key in req.Headers.AllKeys) {
					if (key == RequestUtils.ForwardToHeader)
						continue;
					try {
						remoteReq.Headers.Add (key, req.Headers.Get (key));
					} catch {}
				}

				try {
					var remoteResponse = remoteReq.GetResponse ();
					using (var dataStream = new BufferedStream (remoteResponse.GetResponseStream ()))
						using (var outStream = File.Create (filePath))
							dataStream.CopyTo (outStream);

					var resp = ctx.Response;
					resp.Headers = remoteResponse.Headers;
					using (var output = resp.OutputStream)
						using (var input = new BufferedStream (File.OpenRead (filePath)))
							input.CopyTo (output);
				} catch (WebException e) {
					var r = (HttpWebResponse)e.Response;
					ctx.Response.StatusCode = (int)r.StatusCode;
					ctx.Response.Close ();
				}
			}
		}
	}
}

