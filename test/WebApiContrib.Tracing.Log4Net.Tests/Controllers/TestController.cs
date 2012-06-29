using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Tracing;

namespace WebApiContrib.Tracing.Log4Net.Tests.Controllers
{
	public class TestController : ApiController
	{
		private static readonly string[] _items = new[] { "value1", "value2" };
		private readonly ITraceWriter _traceWriter;

		public TestController()
		{
			_traceWriter = GlobalConfiguration.Configuration.Services.GetTraceWriter();
		}

		public IEnumerable<string> GetAll()
		{
			_traceWriter.Info(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, "Info loaded: " + _items.Length);
			return _items;
		}

		public string Get(int id)
		{
			if (id < 1 || id > _items.Length)
			{
				_traceWriter.Warn(Request, ControllerContext.ControllerDescriptor.ControllerType.FullName, "Could not find: " + id);
			}

			return _items[id - 1];
		}
	}
}
