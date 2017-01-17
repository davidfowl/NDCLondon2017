using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionDemos
{
    class Program
    {
        static void Main(string[] args)
        {
            // TransientDisposablesWithoutDispose();
            // DeadLockWithFactories();
            // CaptiveDependency();
            // ScopedServiceBecomesSingleton();
        }

        static void ScopedServiceBecomesSingleton()
        {
            var services = new ServiceCollection();
            services.AddScoped<B>();

            var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            var a = serviceProvider.GetRequiredService<B>();
        }

        static void CaptiveDependency()
        {
            // See http://blog.ploeh.dk/2014/06/02/captive-dependency/
            var services = new ServiceCollection();
            services.AddSingleton<A>();
            services.AddScoped<B>();

            var serviceProvider = services.BuildServiceProvider(validateScopes: true);
            // var serviceProvider = services.BuildServiceProvider(validateScopes: true);

            var a = serviceProvider.GetRequiredService<A>();
        }

        static void DeadLockWithFactories()
        {
            var services = new ServiceCollection();
            services.AddSingleton(s =>
            {
                var b = GetBAsync(s).Result;

                return new A(b);
            });

            services.AddSingleton<B>();

            var serviceProvider = services.BuildServiceProvider();
            var a = serviceProvider.GetRequiredService<A>();
        }

        static async Task<B> GetBAsync(IServiceProvider serviceProvider)
        {
            // Do something async
            await Task.Delay(1000);
            return serviceProvider.GetRequiredService<B>();
        }

        static void TransientDisposablesWithoutDispose()
        {
            var services = new ServiceCollection();
            services.AddTransient<SomethingDisposable>();
            var serviceProvider = services.BuildServiceProvider();

            for (int i = 0; i < 1000; i++)
            {
                serviceProvider.GetRequiredService<SomethingDisposable>();
            }
        }
    }

    public class A
    {
        public A(B b)
        {

        }
    }

    public class B
    {

    }

    public class SomethingDisposable : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("Disposed");
        }
    }
}