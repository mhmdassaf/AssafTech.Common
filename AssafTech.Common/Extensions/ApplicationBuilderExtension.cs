namespace AssafTech.Common.Extensions;

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

                var httpStatusCode = HttpStatusCode.OK;
                switch (exception)
                {
                    case UnauthorizedAccessException ex:
						httpStatusCode = HttpStatusCode.Unauthorized;
                        responseModel.Errors.Add(new ErrorModel((int)HttpStatusCode.Unauthorized, ex.Message));
                        break;
                    default:
						httpStatusCode = HttpStatusCode.InternalServerError;
                        responseModel.Errors.Add(new ErrorModel((int)HttpStatusCode.InternalServerError,exception?.Message ?? "Internal server error!"));
                        break;
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)httpStatusCode;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(responseModel));
            });
        });
        return app;
    }

    public static WebApplication ApplyCors(this WebApplication app, params string[] origins)
    {
        app.UseCors(builder =>
        {
            builder.AllowCredentials()
                   .WithOrigins(origins)
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
        return app;
    }
}
