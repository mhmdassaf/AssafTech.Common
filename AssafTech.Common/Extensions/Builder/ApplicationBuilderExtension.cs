﻿namespace AssafTech.Common.Extensions.Builder;

public static class ApplicationBuilderExtension
{
    public static WebApplication UseCheckAPI(this WebApplication app, string serviceName)
    {
        app.MapGet("/api/check", () => $"{serviceName} is working!");
        return app;
    }

	public static WebApplication ApplyMigration<TDbContext>(this WebApplication app) where TDbContext : DbContext
	{
		var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope();
		if (serviceScope == null) return app;

		var dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
		dbContext.Database.Migrate();
		return app;
	}

	public static WebApplicationBuilder UseNLog(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseNLog();

        return builder;
    }

    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var responseModel = new ResponseModel();
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                switch (exception)
                {
                    case UnauthorizedAccessException ex:
                        responseModel.HttpStatusCode = HttpStatusCode.Unauthorized;
                        responseModel.Errors.Add(new ErrorModel
                        {
                            Code = HttpStatusCode.Unauthorized.ToString(),
                            Message = ex.Message
                        });
                        break;
                    default:
                        responseModel.HttpStatusCode = HttpStatusCode.InternalServerError;
                        responseModel.Errors.Add(new ErrorModel
                        {
                            Code = HttpStatusCode.InternalServerError.ToString(),
                            Message = "Internal server error!"
                        });
                        break;
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)responseModel.HttpStatusCode;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(responseModel));
            });
        });
        return app;
    }
}
