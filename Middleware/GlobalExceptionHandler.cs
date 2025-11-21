using System;
using System.Net;
using Calmind.Api.DTOs;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Calmind.Api.Middleware
{
    /// <summary>
    /// Handler global para tratamento de exceções não capturadas
    /// Equivalente ao @ControllerAdvice do Spring Boot
    /// </summary>
    public static class GlobalExceptionHandler
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var exception = contextFeature.Error;

                        logger.LogError($"Erro não tratado: {exception.Message}");
                        logger.LogError($"StackTrace: {exception.StackTrace}");

                        var response = ApiResponse<object>.ErrorResponse(
                            app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                                ? new List<string> { exception.Message, exception.StackTrace ?? "" }
                                : new List<string> { "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde." }
                        );

                        await context.Response.WriteAsJsonAsync(response);
                    }
                });
            });
        }
    }
}