using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TextAnalLab2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var services = new ServiceCollection();
            services.AddTransient<Lab2>();
            services.AddTransient<NGramBuilder>();
            services.AddTransient<NGramProbBuilder>();
            services.AddTransient<Predicter>();
            services.AddTransient<TextCleaner>();
            services.AddTransient<VocabularyCorpusBuilder>();

            var service = services.BuildServiceProvider().GetService<Lab2>();
            var result = service.Main("нежно он  поцеловал", @"C:\Users\f98f9\Desktop\9268976.txt", 20);
            Console.WriteLine(result);
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0);
            
        }
    }
}