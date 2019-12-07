using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProcastinationKiller.Helpers;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services;
using ProcastinationKiller.Services.Abstract;
using AWS.Logger.Core;
using Microsoft.OpenApi.Models;

namespace ProcastinationKiller
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
            
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://securetoken.google.com/proc-killer";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://securetoken.google.com/proc-killer",
                        ValidateAudience = true,
                        ValidAudience = "proc-killer",
                        ValidateLifetime = true
                    };
                });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.IncludeXmlComments("ProcastinationKiller.xml");
            });


            var hashingPassword = new SecureString();

            var pass = Configuration.GetValue<string>("Encoding:Activation");

            foreach (var c in pass)
            {
                hashingPassword.AppendChar(c);
            }

            services.AddSingleton<IEncryptor>(sp => new Encryptor(hashingPassword));
            services.AddScoped<IMailProvider, MailProvider>();
            services.AddScoped<ITemplateProvider, FileTemplateProvider>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IRewardService, RewardService>();
            services.AddSingleton<IMailingService, MailingService>();
            services.Configure<MailingOptions>(opt =>
            {
                opt.Address = Configuration.GetValue<string>("Mail:Address");
                opt.Password = Configuration.GetValue<string>("Mail:Password");
            });

            //services.AddDbContext<UsersContext>();
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<UsersContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("SystemDb")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            /*
            services.AddCors(o => o.AddPolicy("any", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .AllowAnyHeader();
            }));
            */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                 app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            else
            {
                app.UseHsts();
            }



            // global cors policy
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
