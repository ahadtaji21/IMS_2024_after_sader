using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using jrs.DBContexts;
using jrs.Interfaces;
using jrs.Middlewares;
using jrs.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using VueCliMiddleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SystemOut.Swagger.EfHelpers;
using Core.Api.Infra.Swagger.Filters;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;


using jrs.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;


using System.Security.Claims;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;


using Microsoft.Net.Http.Headers;
using OData.Swagger.Services;
//using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;

//using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Routing;

using Microsoft.AspNetCore.OData.Authorization;
using JRSDBContext = jrs.DBContexts.JRSDBContext;

namespace jrs
{
	public class Startup
    {
        private readonly IWebHostEnvironment _env;


        public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
            _env = env;
        }

		public IConfiguration Configuration { 
			
			get; }

		readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			string[] alloweOrigins = Configuration.GetValue<String>("Cors:AllowOrigins").Split(";");

			services.AddControllers().AddNewtonsoftJson(options =>
			{
				options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			});
			// services.AddControllers().AddXmlSerializerFormatters();



			services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("ODataAuthDemo"));

			services.AddOData();

			services.AddScoped(_ =>
			{
				return new BlobServiceClient(Configuration.GetConnectionString("AzureBlobStorage"));
			});
			services.AddScoped<IBlobStorageService, BlobStorageService>();

			services.AddODataAuthorization(options =>
	   {
		   options.ScopesFinder = context =>
		   {
			   //    context.User.Claims.Append(new Claim("Scope","NavCoa.Read"));
			   //    var userScopes = context.User.FindAll("Scope").Select(claim => claim.Value);
			   var userScopes = new Claim("Scope", "NavCoa.Read");
			   List<string> scopes = new List<string>();
			   if (context.User.Identity.IsAuthenticated)
				   scopes.Add("NavCoa.Read");
			   return Task.FromResult(scopes as IEnumerable<string>);
		   };

		   options.ConfigureAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			   .AddCookie();
	   });
			// services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
			// .AddNewtonsoftJson(options =>{
			//     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			// })
			;

			// services.AddControllers(options => {
			//     options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());


			// });
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = $"v1(Build: {GetType().Assembly.GetName().Version})",
					Title = "JRSAPI",
					Description = "JRS Information Management System API",
					Contact = new OpenApiContact
					{
						Name = "BLogic s.r.l.",
						Email = "blogic@blogic.it",
						Url = new Uri("https://www.blogic.it")
					}

				});
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);

				c.SchemaFilter<ApllyIgnoreRelationshipInNamespace<jrs.Models.Menu>>();


				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					In = ParameterLocation.Header,
					Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement{
					{
						new OpenApiSecurityScheme{
							Reference = new OpenApiReference{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						new string[]{}
					}
				});
				// c.AddSecurityDefinition("Bearer", new ApiKeyScheme
				// {
				//     Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
				//     Name = "Authorization",
				//     In = "header",
				//     Type = "apiKey"
				// });
			});
			services.AddOdataSwaggerSupport();

			// services.AddSwaggerGenNewtonsoftSupport();


			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = "ClientApp/dist";
			});

			//Cors calls
			services.AddCors(options =>
			{
				options.AddPolicy(MyAllowSpecificOrigins, builder =>
				{
					builder.WithOrigins(
						// new string[]{"http://localhost:8080",
						// "http://localhost:5000",
						// "http://w2012.blogic.it/jrsapp",
						// "https://w2012.blogic.it/jrsapp"
						// });
						alloweOrigins).AllowAnyHeader()
								.AllowAnyMethod();
				});
				// options.AddPolicy()
			});

			//Toke authentication
			var secret = Configuration.GetValue<string>("AppSettings:secret");
			var key = Encoding.ASCII.GetBytes(secret);
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
		{
			options.RequireHttpsMetadata = false;
			options.SaveToken = true;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = false,
				ValidateAudience = false
			};

			//Handle Token expiration error event
			options.Events = new JwtBearerEvents
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

			services.AddDbContext<ApiLogDbContext>();
			services.AddDbContext<JRSDBContext>(opt =>
			{
				opt.UseSqlServer(Configuration.GetConnectionString("jrsdb"));
			});
			services.AddTransient<ApiLogService>();
			services.AddTransient<IRepository, Repository>();

			services.AddDbContext<BiodataContext>(opt =>
			{
				opt.UseSqlServer(Configuration.GetConnectionString("jrsdb"));
			});
			services.AddDbContext<PMSContext>(opt =>
			{
				opt.UseSqlServer(Configuration.GetConnectionString("jrsdb"));
			});
			services.AddDbContext<GeneralContext>(opt =>
			{
				
				
				
				
				
				opt.UseSqlServer(Configuration.GetConnectionString("jrsdb"));
                //opt.UseSqlServer(Configuration.GetConnectionString("LocalTestJrsdb"));

            });




			services.AddDbContext<IMSLogContext>(opt =>
			{
				opt.UseSqlServer(Configuration.GetConnectionString("jrsdb"));
               // opt.UseSqlServer(Configuration.GetConnectionString("LocalTestJrsdb"));

            });

           






            // 	services.AddDbContext<SfipContext>(opt =>
            //    {
            // 	   opt.UseSqlServer(Configuration.GetConnectionString("jrsdb"));
            //    });

            //    services.AddMvc()
            // .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            // .AddXmlSerializerFormatters();

        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			//if (env.IsDevelopment())
			//{
				app.UseDeveloperExceptionPage();
				app.UseCors(MyAllowSpecificOrigins);
			//}


           

            app.UseHttpsRedirection();

			app.UseRouting();
			app.UseMiddleware<ApiLogMiddleware>();

			app.UseAuthentication();
			//             app.Use(async(context, next)=>{
			//     if(context.User !=null && context.User.Identity.IsAuthenticated){
			//         // add claims here 
			//         context.User.Claims.Append(new Claim("Scope","NavCoa.Read"));
			//     }
			//     await next();
			// });
			app.UseAuthorization();
			app.UseODataAuthorization();



			//OLD
			/*
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "JRS API V1");
            });
            */
			app.UseSwagger(options =>
			{
				options.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
				{
					swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpRequest.Scheme}://{httpRequest.Host.Value}/{httpRequest.PathBase}" } };
				});
				// options.RouteTemplate = "{documentName}/swagger.json";
				//options.RouteTemplate = "swagger/v1/swagger.json";
			});

			//TO STUDY: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1355
			//TO STUDY: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/903

			app.UseSwaggerUI(options =>
			{
				var appVersion = typeof(Program).Assembly.GetName().Version.ToString(2);
				var appName = typeof(Program).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;

				options.SwaggerEndpoint(
					 // $"../v{appVersion}/swagger.json",
					 $"../swagger/v1/swagger.json",
					"My API v" + appVersion
				);
			});


			app.UseEndpoints(endpoints =>
			{
			 endpoints.MapControllers();
				// //endpoints.EnableDependencyInjection();
				// endpoints.Select().OrderBy().Filter();
				// // endpoints.MapODataRoute("odatacoa", "odatacoa", GetEdmModelCoa(app.ApplicationServices));

				// endpoints.MapODataRoute("odatacoa", "odatacoa", GetEdmModelCoa(app.ApplicationServices));


				//if (env.IsDevelopment())
				//{
				//    endpoints.MapToVueCliProxy(
				//        "{*path}",
				//        new SpaOptions { SourcePath = "ClientApp" },
				//        npmScript: "serve",
				//        regex: "Compiled successfully");
				//}
			});

		}

		// private static IEdmModel GetEdmModelCoa(IServiceProvider serviceProvider)
		// {
		//     var odataBuilder = new ODataConventionModelBuilder(serviceProvider);
		//     odataBuilder.EntitySet<NavCoa>("NavIms");

		//     return odataBuilder.GetEdmModel();
		// }
		private static IEdmModel GetEdmModelCoa(IServiceProvider serviceProvider)
		{
			var builder = new ODataConventionModelBuilder();
			var navcoa = builder.EntitySet<NavCoa>("NavCoa");

			navcoa.HasReadRestrictions()
				.HasPermissions(p =>
					p.HasSchemeName("Scheme").HasScopes(s => s.HasScope("NavCoa.Read")))
				.HasReadByKeyRestrictions(r => r.HasPermissions(p =>
					p.HasSchemeName("Scheme").HasScopes(s => s.HasScope("NavCoa.ReadByKey"))));

			navcoa.HasInsertRestrictions()
				.HasPermissions(p => p.HasSchemeName("Scheme").HasScopes(s => s.HasScope("NavCoa.Create")));

			navcoa.HasUpdateRestrictions()
				.HasPermissions(p => p.HasSchemeName("Scheme").HasScopes(s => s.HasScope("NavCoa.Update")));

			navcoa.HasDeleteRestrictions()
				.HasPermissions(p => p.HasSchemeName("Scheme").HasScopes(s => s.HasScope("NavCoa.Delete")));

			return builder.GetEdmModel();
			// var odataBuilder = new ODataConventionModelBuilder(serviceProvider);
			// odataBuilder.EntitySet<NavCoa>("NavBdg");

			// return odataBuilder.GetEdmModel();
		}

		// private static NewtonsoftJsonInputFormatter GetJsonPatchInputFormatter()
		// {
		//     var builder = new ServiceCollection()
		//         .AddLogging()
		//         .AddMvc()
		//         .AddNewtonsoftJson()
		//         .Services.BuildServiceProvider();

		//     return builder
		//         .GetRequiredService<IOptions<MvcOptions>>()
		//         .Value
		//         .InputFormatters
		//         .OfType<NewtonsoftJsonInputFormatter>()
		//         .First();
		// }
	}
}
