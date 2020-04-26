using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace CW_3_v2.Middlewares
{
	public class LoggingMiddleware
	{
		private readonly RequestDelegate _next;

		public LoggingMiddleware (RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync (HttpContext context)
		{
			context.Request.EnableBuffering();

			if(context.Request != null)
			{
				string path = context.Request.Path;
				string metohd = context.Request.Method;
				string queryString = context.Request.QueryString;
				string bodyString = "";

				FileInfo outputFile = new FileInfo(@"log.txt");

				using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, 1024, true))
				{
					bodyString = await reader.ReadToEndAsync();
					context.Request.Body.Position = 0;
				}

				try
                {
                    StreamWriter logEvent = new StreamWriter(f.OpenWrite());
                    logEvent.Write(bodyString);
                    logEvent.Close();
                }
                catch(Exception ex)
				{
					
				}
			}
			if (_next! = null) await _next(context);
		}
	}
}
