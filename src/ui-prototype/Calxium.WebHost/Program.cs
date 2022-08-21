
using Calxium.UI.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Calxium.WebHost {
	public class Program {
		public static void Main(string[] args) {
			Host.CreateDefaultBuilder()
				.ConfigureServices(svc => svc.AddHostedService<BlazorUIHostService>())
				.Build()
				.Run();
		}
	}
}