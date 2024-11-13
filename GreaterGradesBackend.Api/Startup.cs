using AutoMapper;
using GreaterGradesBackend.Common.Extensions;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Infrastructure;
using GreaterGradesBackend.Infrastructure.Repositories;
using GreaterGradesBackend.Infrastructure.UnitOfWork;
using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GreaterGradesBackend.Services.Interfaces;
using GreaterGradesBackend.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

namespace GreaterGradesBackend.Api
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
            services.AddDbContext<GreaterGradesBackendDbContext>(options =>
                options.UseSqlServer("Server=tcp:greatergradesserver.database.windows.net,1433;Initial Catalog=greatergradesdatabase;Persist Security Info=False;User ID=greatergrades;Password=GGpassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IClassService, ClassService>();
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<IGradeService, GradeService>();
            services.AddScoped<IInstitutionService, InstitutionService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            var jwtSection = Configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSection);

            var jwtSettings = jwtSection.Get<JwtSettings>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<IUserService>(provider => 
                new UserService(
                    provider.GetRequiredService<IUnitOfWork>(),
                    provider.GetRequiredService<IMapper>(),
                    provider.GetRequiredService<IPasswordHasher<User>>(),
                    jwtSettings));
            
            services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:5000", "http://localhost:3000") // We can change this line to allow whatever origin we need
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddAutoMapper(typeof(Startup));

            services.AddCommonServices();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "GreaterGradesBackend API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }



        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, GreaterGradesBackendDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GreaterGradesBackend API V1"));
            }

            dbContext.Database.Migrate();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("FrontendPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
