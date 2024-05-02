using jrs.DBContexts;
using jrs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace jrs.Middlewares
{
	internal class ApiLogMiddleware
	{
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiLogMiddleware> _logger;
        private ApiLogService _apiLogService;

        public ApiLogMiddleware(RequestDelegate next, ILogger<ApiLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, ApiLogService apiLogService)
        {
            try
            {
                _apiLogService = apiLogService;

                var request = httpContext.Request;
                var stopWatch = Stopwatch.StartNew();
                var requestTime = DateTime.UtcNow;
                var requestBodyContent = await ReadRequestBody(request);
                var originalBodyStream = httpContext.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    var response = httpContext.Response;
                    response.Body = responseBody;
                    await _next(httpContext);
                    stopWatch.Stop();

                    string responseBodyContent = null;
                    responseBodyContent = await ReadResponseBody(response);
                    await responseBody.CopyToAsync(originalBodyStream);

                    var dict = new Dictionary<string, string>(); 
                    try{
                      foreach (var h in request.Headers)
                     {

                        dict.Add(h.Key, string.Join(",", h.Value.ToArray()));
                     }
                    }catch(Exception ex){
             
                    }
      
                    var headers = JsonSerializer.Serialize(dict);

                    var dictV = new Dictionary<string, string>();

                    dictV.Add("RemoteIpAddress",
                        request.HttpContext.Connection.RemoteIpAddress.ToString());

                    var serverVariables = JsonSerializer.Serialize(dictV);
                    if (response.Headers.ContainsKey("Content-Type") &&
                       response.Headers["Content-Type"].Count == 1 &&
                       response.Headers["Content-Type"][0] == "application/octet-stream")
                    {
                        responseBodyContent = "";
                    }

                    await SafeLog(requestTime,
                        stopWatch.ElapsedMilliseconds,
                        response.StatusCode,
                        request.Method,
                        headers,
                        serverVariables,
                        request.Path,
                        request.QueryString.ToString(),
                        requestBodyContent,
                        responseBodyContent);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Errore ApiLogMiddleware: {httpContext.Request.Path}");
                await _next(httpContext);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task SafeLog(DateTime requestTime,
                            long responseMillis,
                            int statusCode,
                            string method,
                            string headers,
                            string serverVariables,
                            string path,
                            string queryString,
                            string requestBody,
                            string responseBody)
        {
            // TODO: da rivedere 
            if (path.ToLower().StartsWith("/Auth/Login", true,
                CultureInfo.InvariantCulture))
            {
                requestBody = "(disabled for Auth)";
                responseBody = "(disabled)";
                queryString = "(disabled)";
            }

            if (requestBody.Length > 200)
            {
                requestBody = $"(Truncated to 100 chars) {requestBody.Substring(0, 200)}";
            }

            if (responseBody.Length > 200)
            {
                responseBody = $"(Truncated to 100 chars) {responseBody.Substring(0, 200)}";
            }

            if (queryString.Length > 256)
            {
                queryString = $"|{queryString.Substring(queryString.Length - 256, 256)}";
            }

            await _apiLogService.Log(new JRSApiLog
            {
                RequestTime = requestTime,
                ResponseMillis = responseMillis,
                StatusCode = statusCode,
                Method = method,
                Headers = headers,
                ServerVariables = serverVariables,
                Path = path,
                QueryString = queryString,
                RequestBody = requestBody,
                ResponseBody = responseBody
            });
        }
    }
}