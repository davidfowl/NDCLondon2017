using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EndToEndTesting
{
    public class PipelineFacts
    {
        [Fact]
        public async Task VerifyResponse()
        {
            var builder = new WebHostBuilder()
                    .UseStartup<Startup>();

            var server = new TestServer(builder);
            var client = server.CreateClient();

            var response = await client.GetAsync("http://something");

            Assert.Equal("Hello World", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task VerifyTestEnvironmentResponse()
        {
            var builder = new WebHostBuilder()
                    .UseEnvironment("Test")
                    .UseStartup<StartupWithEnvironment>();

            var server = new TestServer(builder);
            var client = server.CreateClient();

            var response = await client.GetAsync("http://something");

            Assert.Equal("Test", await response.Content.ReadAsStringAsync());
        }
    }

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World");
            });
        }
    }

    public class StartupWithEnvironment
    {
        private readonly IHostingEnvironment _env;

        public StartupWithEnvironment(IHostingEnvironment env)
        {
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsEnvironment("Test"))
            {
                services.AddSingleton<IService, TestService>();
            }
            else
            {
                services.AddSingleton<IService, ProductionService>();
            }
        }

        public void Configure(IApplicationBuilder app, IService service)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync(service.GetData());
            });
        }

        public interface IService
        {
            string GetData();
        }

        public class ProductionService : IService
        {
            public string GetData() => "Production";
        }

        public class TestService : IService
        {
            public string GetData() => "Test";
        }
    }
}
