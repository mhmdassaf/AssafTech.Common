namespace AssafTech.Common.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCommonApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<BaseUrl>(configuration.GetSection(nameof(BaseUrl)));
        return services;
    }
    public static IServiceCollection AddSwagger(this IServiceCollection services, SwaggerModel swaggerModel)
    {
        var baseUrl = services.BuildServiceProvider().GetService<IOptions<BaseUrl>>();
        if (baseUrl == null) { throw new ArgumentNullException(nameof(baseUrl)); }

        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc(swaggerModel.Version, new OpenApiInfo { Title = swaggerModel.Title, Version = swaggerModel.Version });
            option.AddSecurityDefinition(SecuritySchemeType.OAuth2.GetDisplayName(), new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{baseUrl.Value.Gateway}/{ServiceName.Identity}/connect/authorize"),
                        TokenUrl = new Uri($"{baseUrl.Value.Gateway}/{ServiceName.Identity}/connect/token"),
                        Scopes = swaggerModel.IdentityServerScopes
					}
                }
            });
            option.OperationFilter<AuthorizeCheckOperationFilter>();
            //option.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {
            //            Reference = new OpenApiReference
            //            {
            //                Type= ReferenceType.SecurityScheme,
            //                Id= JwtBearerDefaults.AuthenticationScheme
            //            }
            //        },
            //        new string[]{}
            //    }
            //});
        });

        return services;
    }
    public static IServiceCollection AddAuth(this IServiceCollection services, string clientId, string clientSecret)
    {
        var baseUrl = services.BuildServiceProvider().GetService<IOptions<BaseUrl>>();
        var identityBaseUrl = baseUrl?.Value.Identity;
        if (identityBaseUrl == null) { throw new ArgumentNullException(nameof(identityBaseUrl)); }

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        // Register the OpenIddict validation components.
        services.AddOpenIddict()
                .AddValidation(options =>
                {
                    // Note: the validation handler uses OpenID Connect discovery
                    // to retrieve the address of the introspection endpoint.
                    options.SetIssuer(identityBaseUrl);
                    //options.AddAudiences(clientId);

                    // Configure the validation handler to use introspection and register the client
                    // credentials used when communicating with the remote introspection endpoint.
                    options.UseIntrospection()
                           .SetClientId(clientId)
                           .SetClientSecret(clientSecret);

                    // Register the System.Net.Http integration.
                    options.UseSystemNetHttp();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

        return services;
    }
    public static IServiceCollection AddDatabase<TContext>(this IServiceCollection services, ConfigurationManager configuration)
        where TContext : DbContext
    {
		services.AddDbContext<TContext>(config =>
		{
			config.UseNpgsql(configuration.GetConnectionString(nameof(ConnectionStrings.AssafTechConnection)));
		});

        return services;
	}
}

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context?.MethodInfo?.DeclaringType == null) return;

        var hasAuthorize =
          context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
          || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme {Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = SecuritySchemeType.OAuth2.GetDisplayName()}
                        }
                    ] = new[] {IdentityServerApi.Name.Identity }
                }
            };

        }
    }
}