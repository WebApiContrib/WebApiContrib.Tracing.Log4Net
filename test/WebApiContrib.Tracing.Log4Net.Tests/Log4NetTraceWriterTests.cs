using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Tracing;
using NUnit.Framework;
using Should;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace WebApiContrib.Tracing.Log4Net.Tests
{
	public class Log4NetTraceWriterTests
	{
		private static MemoryAppender _appender;
		private static CancellationTokenSource _cts;
		private static HttpMessageInvoker _client;

		static Log4NetTraceWriterTests()
		{
			_appender = new MemoryAppender { Name = "MemoryAppender" };
			_appender.ActivateOptions();
			var repository = (Hierarchy) LogManager.GetRepository();
			repository.Root.AddAppender(_appender);
			repository.Root.Level = Level.All;
			repository.Configured = true;
			repository.RaiseConfigurationChanged(EventArgs.Empty);

			_cts = new CancellationTokenSource();

			GlobalConfiguration.Configuration.Routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional });
			GlobalConfiguration.Configuration.Services.Replace(typeof (ITraceWriter), new Log4NetTraceWriter());
			var server = new HttpServer(GlobalConfiguration.Configuration);
			_client = new HttpMessageInvoker(server);
		}

		[Test]
		public static void TestGetAll()
		{
			var _ = _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://example.org/test"), _cts.Token).Result;
			var events = _appender.GetEvents();

			events.ShouldNotBeEmpty();
			events.Select(x => x.RenderedMessage).ShouldContain(" GET http://example.org/test WebApiContrib.Tracing.Log4Net.Tests.Controllers.TestController Info loaded: 2");
		}

		[Test]
		public static void TestGetWithId0()
		{
			var _ = _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://example.org/test/0"), _cts.Token).Result;
			var events = _appender.GetEvents();

			events.ShouldNotBeEmpty();
			events.Select(x => x.RenderedMessage).ShouldContain(" GET http://example.org/test/0 WebApiContrib.Tracing.Log4Net.Tests.Controllers.TestController Could not find: 0");
		}
	}
}
