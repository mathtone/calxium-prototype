using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calxium.UI.Core.Services {

	public class BlazorUIHostService : IHostedService {

		WebApplication? _app;

		public Task StartAsync(CancellationToken cancellationToken) {

			var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
				Args = Environment.GetCommandLineArgs(),
				
			});

			Configure(builder.Services);
			_app = Configure(builder.Build());
			return _app.StartAsync(cancellationToken);
		}

		public static IServiceCollection Configure(IServiceCollection services) {
			services.ConfigureOptions<ContentConfigurationOptions>();
			services.AddRazorPages();
			services.AddServerSideBlazor();
			return services;
		}

		public static WebApplication Configure(WebApplication app) {

			if (!app.Environment.IsDevelopment()) {
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.MapBlazorHub();
			app.MapFallbackToPage("/_Host");

			return app;
		}

		public Task StopAsync(CancellationToken cancellationToken) =>
			_app?.StopAsync(cancellationToken) ?? Task.CompletedTask;
	}

	public class ContentConfigurationOptions : IPostConfigureOptions<StaticFileOptions> {

		public IHostEnvironment Environment { get; }

		public ContentConfigurationOptions(IHostEnvironment environment) {
			Environment = environment;
		}

		public void PostConfigure(string name, StaticFileOptions options) {

			_ = name ?? throw new ArgumentNullException(nameof(name));
			_= options ?? throw new ArgumentNullException(nameof(options));
			options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();
			if (options.FileProvider == null && Environment.ContentRootFileProvider== null) {
				throw new InvalidOperationException("Missing FileProvider.");
			}

			options.FileProvider ??= Environment.ContentRootFileProvider;

			var basePath = "wwwroot";
			//_ = new ManifestEmbeddedFileProvider(GetType().Assembly, basePath);
			options.FileProvider = new CompositeFileProvider(options.FileProvider, new ManifestEmbeddedFileProvider(GetType().Assembly, basePath));
		}
	}
}