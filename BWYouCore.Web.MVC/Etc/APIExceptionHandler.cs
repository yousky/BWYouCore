using BWYouCore.Web.MVC.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Web.MVC.Etc
{
    public class APIExceptionHandler
    {
        public async Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (ex == null) return;

            var vm = new ErrorResultViewModel(context.Response.StatusCode, ex);

            context.Response.ContentType = "application/json";

            using (var writer = new StreamWriter(context.Response.Body))
            {
                new JsonSerializer().Serialize(writer, vm);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}
