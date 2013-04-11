using System;
using System.Net;
using System.Linq;

namespace NetworkReplayer
{
	public static class RequestUtils
	{
		public const string ForwardToHeader = "X-Forward-To";

		public static string ToFileName (string originalUrl, HttpListenerRequest req)
		{
			var hash = originalUrl.GetHashCode ().ToString ("X");
			hash += "-";
			hash += req.RawUrl.GetHashCode ().ToString ("X");
			hash += "-";
			hash += req.Headers.AllKeys
				.Select (k => ((long)k.GetHashCode ()) << 31 + req.Headers.Get (k).GetHashCode ())
				.Aggregate ((h1, h2) => h1 ^ h2)
				.ToString ("X");

			return hash;
		}
	}
}

