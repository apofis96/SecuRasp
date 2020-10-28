using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using RaspSecure.Context;
using RaspSecure.Extensions;
using RaspSecure.Helpers;
using RaspSecure.Jobs;
using RaspSecure.Models.Auth;
using RaspSecure.Models.Mail;
using RaspSecure.Services;

namespace RaspSecure
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RaspSecureDbContext>( options => options.UseSqlite("Data Source=raspSecure.db"));
            services.AddDbContext<LogsDbContext>(options => options.UseSqlite("Data Source=logs.db"));
            services.AddMvc(options => options.Filters.Add(typeof(CustomExceptionFilterAttribute)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<CodeService>();

            services.AddSingleton<IJobFactory, JobsFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddSingleton<LogEraser>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(LogEraser),
                cronExpression: "0 0 3 * * ?")); //0 0 3 * * ? at 3am "0/5 * * * * ?"
            services.AddHostedService<QuartzHostedService>();
            services.AddScoped<JwtIssuerOptions>();
            services.AddScoped<JwtFactory>();
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<MailOptions>();
            services.AddScoped<MailService>();

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var secretKey = jwtAppSettingOptions["SecretJWTKey"];
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;

                configureOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            var mailOptions = Configuration.GetSection(nameof(MailOptions));
            services.Configure<MailOptions>(options =>
            {
                options.SmtpServer = mailOptions[nameof(MailOptions.SmtpServer)];
                options.SmtpUsername = mailOptions[nameof(MailOptions.SmtpUsername)];
                options.SmtpPort = Int32.Parse(mailOptions[nameof(MailOptions.SmtpPort)]);
                options.SmtpPassword = mailOptions[nameof(MailOptions.SmtpPassword)];
                options.SmtpAddress = mailOptions[nameof(MailOptions.SmtpAddress)];
                options.MailHeaderName = mailOptions[nameof(MailOptions.MailHeaderName)];
            });
            services.AddAuthorization();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseHsts(); //мож ненадо?
            }

            //app.UseHttpsRedirection(); //мож ненадо?
            app.UseAuthentication();
            app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .WithExposedHeaders("Token-Expired")
                /*.WithOrigins("http://localhost:4200")*/);
            app.UseMvc();
            /*app.UseHttpsRedirection();*/
            //app.UseAuthentication();

            /*app.UseEndpoint(cfg =>
            {
                cfg.MapControllers();
                cfg.MapHub<PostHub>("/notifications/post");
                cfg.MapHub<CommentHub>("/notifications/comment");
            });*/
            /*https://stackoverflow.com/questions/61404058/asp-net-core-dont-have-app-useendpoints-method*/
        }
    }
}
