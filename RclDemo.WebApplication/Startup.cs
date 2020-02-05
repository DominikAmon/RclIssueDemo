using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace RclDemo.WebApplication
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
			services.AddControllersWithViews();
			ConfigureMvcParts(services, typeof(RclDemo.Library.Areas.Sample.Controllers.HomeController));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");


				MapSampleRoute(endpoints);
			});
		}

		/// <summary>
		/// Register AssemblyPart
		/// </summary>
		/// <param name="services"></param>
		/// <param name="containingClassType"></param>
		public static void ConfigureMvcParts(IServiceCollection services, Type containingClassType)
		{
			Assembly assembly = containingClassType.Assembly;
			AssemblyPart assemblyPart = new AssemblyPart(assembly);
			EmbeddedFileProvider fileProvider = new EmbeddedFileProvider(assembly);
			services.AddControllersWithViews().ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(assemblyPart));
			services.Configure<MvcRazorRuntimeCompilationOptions>(options => options.FileProviders.Add(fileProvider));
		}

		/// <summary>
		/// Map Area
		/// </summary>
		/// <param name="endpoints"></param>
		public static void MapSampleRoute(IEndpointRouteBuilder endpoints)
		{
			endpoints.MapAreaControllerRoute(
				name: "Sample",
				areaName: "Sample",
				pattern: "Sample/{controller=Home}/{action=Index}/{id?}");
		}

	}
}
