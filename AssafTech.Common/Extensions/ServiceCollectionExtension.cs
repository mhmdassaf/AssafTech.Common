namespace AssafTech.Common.Extensions;

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

    /// <summary>
    /// Add Authentication to your app
    /// </summary>
    /// <param name="services"></param>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <param name="canActingAsClient">true to allow the application to make http request 
    /// to another micro-services or to request any other external api</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddAuth(this IServiceCollection services, string clientId, string clientSecret, bool canActingAsClient)
    {
        var baseUrl = services.BuildServiceProvider().GetService<IOptions<BaseUrl>>();
        var identityBaseUrl = baseUrl?.Value.Identity;
        if (identityBaseUrl == null) { throw new ArgumentNullException(nameof(identityBaseUrl)); }

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        // Register the OpenIddict validation components.
        var openIddictBuilder = services.AddOpenIddict()
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

        //to allow the application to make http request to another micro-services
        if (canActingAsClient)
        {
            openIddictBuilder.AddClient(options =>
            {
                // Allow grant_type=client_credentials to be negotiated.
                options.AllowClientCredentialsFlow();

                // Disable token storage, which is not necessary for non-interactive flows like
                // grant_type=password, grant_type=client_credentials or grant_type=refresh_token.
                options.DisableTokenStorage();

                // Register the System.Net.Http integration and use the identity of the current
                // assembly as a more specific user agent, which can be useful when dealing with
                // providers that use the user agent as a way to throttle requests (e.g Reddit).
                options.UseSystemNetHttp();

                // Add a client registration matching the client application definition in the server project.
                options.AddRegistration(new OpenIddictClientRegistration
                {
                    Issuer = new Uri(identityBaseUrl, UriKind.Absolute),
                    ClientId = clientId,
                    ClientSecret = clientSecret
                });
            });
            services.RegisterHttpClient();
        }

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

    #region Private
    private static IServiceCollection RegisterHttpClient(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var baseUrl = provider.GetService<IOptions<BaseUrl>>();
        if (baseUrl == null ||
            baseUrl.Value == null ||
            baseUrl.Value.Gateway == null)
        {
            throw new ArgumentNullException(nameof(baseUrl.Value.Gateway));
        }

        var service = provider.GetRequiredService<OpenIddictClientService>();

        var result = service.AuthenticateWithClientCredentialsAsync(new()).Result;
        var accessToken = result.AccessToken;

        services.AddHttpClient(HttpClientName.AssafTechApiClient, client =>
        {
            //client.BaseAddress = new Uri("http://localhost:44384");//crm base url
            client.BaseAddress = new Uri(baseUrl.Value.Gateway);//crm base url
            // Additional configuration such as headers, timeouts, etc. can be done here
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        });

        services.AddScoped<IApiService, HttpClientService>();
        return services;
    }
    #endregion
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