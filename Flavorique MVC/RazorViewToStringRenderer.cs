using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Flavorique_MVC
{
	public class RazorViewToStringRenderer
	{
		private readonly IRazorViewEngine _razorViewEngine;
		private readonly IServiceProvider _serviceProvider;

		public RazorViewToStringRenderer(IRazorViewEngine razorViewEngine, IServiceProvider serviceProvider)
		{
			_razorViewEngine = razorViewEngine ?? throw new ArgumentNullException(nameof(razorViewEngine));
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public async Task<string> RenderViewToStringAsync(string viewName, object model)
		{
			var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
			var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

			var viewEngineResult = _razorViewEngine.GetView(null, viewName, false);

			if (!viewEngineResult.Success)
			{
				throw new InvalidOperationException($"Couldn't find view '{viewName}'");
			}

			var view = viewEngineResult.View;

			// Resolve ITempDataProvider from the service provider
			var tempDataProvider = (ITempDataProvider)_serviceProvider.GetRequiredService(typeof(ITempDataProvider));

			using (var sw = new StringWriter())
			{
				var viewContext = new ViewContext(actionContext, view, new ViewDataDictionary<object>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model }, new TempDataDictionary(httpContext, tempDataProvider), sw, new HtmlHelperOptions());

				await view.RenderAsync(viewContext);

				return sw.ToString();
			}
		}
	}
}