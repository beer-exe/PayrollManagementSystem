using PayrollManagementSystem.Application.Common.Exceptions;
using PayrollManagementSystem.Application.Wrappers;
using System.Net;
using System.Text.Json;

namespace PayrollManagementSystem.API.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                HttpResponse? response = context.Response;
                response.ContentType = "application/json";

                Response<string>? responseModel = new Response<string>() { Succeeded = false, Message = error?.Message };

                switch (error)
                {
                    case ValidationException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Message = "Dữ liệu đầu vào không hợp lệ.";
                        responseModel.Errors = e.Errors?.SelectMany(x => x.Value).ToArray();
                        break;

                    case KeyNotFoundException e:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        responseModel.Message = "Không tìm thấy dữ liệu yêu cầu.";
                        break;

                    case ApiException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case UnauthorizedAccessException e:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        responseModel.Message = "Bạn không có quyền truy cập tài nguyên này.";
                        break;

                    default:
                        _logger.LogError(error, "An unhandled exception has occurred while executing the request.");

                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseModel.Message = "Đã xảy ra lỗi từ phía máy chủ. Vui lòng thử lại sau.";
                        break;
                }

                string result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
