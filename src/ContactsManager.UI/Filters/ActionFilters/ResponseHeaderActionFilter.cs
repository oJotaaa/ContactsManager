using Microsoft.AspNetCore.Mvc.Filters;
using System.Runtime.CompilerServices;

namespace ContactsManager.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public string? Key { get; set; }
        public string? Value { get; set; }
        public int Order { get; set; }

        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order) 
        {
            Key = key;
            Value = value; 
            Order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();
            filter.Key = Key;
            filter.Value = Value;
            filter.Order = Order;

            return filter;
        }
    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public int Order { get; set; }

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("Before logic - {FilterName}", nameof(ResponseHeaderActionFilter));

            await next();

            _logger.LogInformation("After logic - {FilterName}", nameof(ResponseHeaderActionFilter));

            context.HttpContext.Response.Headers[Key!] = Value;
        }
    }
}
