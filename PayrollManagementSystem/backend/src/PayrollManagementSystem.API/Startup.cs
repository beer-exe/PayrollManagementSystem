using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using PayrollManagementSystem.API.Middlewares;
using PayrollManagementSystem.Application;
using PayrollManagementSystem.Infrastructure;
using PayrollManagementSystem.Infrastructure.Hubs;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PayrollManagementSystem.Infrastructure.Persistence;

namespace PayrollManagementSystem.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(Configuration);  
            services.AddInfrastructureServices(Configuration);
            services.AddSignalR();

            services.AddControllers().AddJsonOptions(options => 
            { 
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            });

            services.AddHttpContextAccessor();
            services.AddScoped<PayrollManagementSystem.Application.Common.Interfaces.ICurrentUserService, PayrollManagementSystem.API.Services.CurrentUserService>();

            services.AddEndpointsApiExplorer();

            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "Payroll Management System API",
                        Version = "v1",
                        Description = "Hệ thống quản lý lương theo phương pháp 3P"
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Description = "Nhập JWT Token vào đây (không cần tiền tố Bearer)",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    };

                    return Task.CompletedTask;
                });
            });

            IConfigurationSection? jwtSettings = Configuration.GetSection("JwtSettings");
            byte[]? secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "sGraw5@|K1aQFW+?fo.T*/fBI)4Jy8P60:wdRtncyO@KFme/2J&toDLz!U#/x$4kb6hIkq16Boo.wx(elXB>EySOik!^Vz%!%!L2URXr&8Ksmj*oWt&7As(b:jut9+|VUBM9OcJtfco[1Hzq;TsBY+kasYrzvu?Tm4FUcLvm9$EWW#A:Iv3fD{CE$f>uI4WKlA7zDrJJehF.f[|4CbA%k#e^v5A.[$J]vyo[wu%C=p1G[Q#%G{rrxJxCaD?c5}o}slmG1L1>&)xaRgGHUzU-)t,JtLzx?eMo=eqptS&{@OkQ=Z)PSorxKzaP=@I:w<0=U*d3lC+)plY,;$<pss)uvE1>jb8m?!$czGc]52sC,C{tmmRgd@)bQqybG&%GY).[e}8kGWk5-@86GA[WOy|7KmA}%Udbcv.X5)_3.-7xiq6,{=,4WVCrc#-:[8:/2&)Y;inTJDuqjgy@UNRN5/1zh;rA{$JGVPvOG7E<{nb*Gl%w,2K)ws7;Rp00:lNd-xC[");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ClockSkew = TimeSpan.Zero,

                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string? accessToken = context.Request.Query["access_token"];
                        PathString path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/monitor"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.SetIsOriginAllowed(_ => true)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            services.AddRateLimiter(options =>
            {
                options.AddPolicy("LoginRateLimit", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
                    var jsonOptions = new System.Text.Json.JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                    var result = System.Text.Json.JsonSerializer.Serialize(new { Succeeded = false, Message = "Vui lòng thử lại sau 1 phút." }, jsonOptions);
                    await context.HttpContext.Response.WriteAsync(result, token);
                };
            });

            var healthChecks = services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>("PostgreSQL Database");

            if (Configuration["CacheSettings:Provider"] == "Redis")
            {
                healthChecks.AddRedis(Configuration["CacheSettings:RedisConnectionString"] ?? "127.0.0.1:6379", name: "Redis Cache");
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();


            app.UseHttpsRedirection();
            
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseRateLimiter();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<LogMonitorHub>("/hubs/monitor");
                endpoints.MapHealthChecks("/api/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                // Scalar UI & OpenAPI document (chỉ hiển thị trong Development)
                if (env.IsDevelopment())
                {
                    endpoints.MapOpenApi();           // /openapi/v1.json
                    endpoints.MapScalarApiReference(); // /scalar
                }
            });
        }
    }
}
