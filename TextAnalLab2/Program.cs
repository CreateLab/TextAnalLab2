using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TextAnalLab2.Models;
using TextAnalLab2.Services;

namespace TextAnalLab2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();
            var services = new ServiceCollection();
            services.AddTransient<NGramBuilder>();
            services.AddTransient<NGramProbBuilder>();
            services.AddTransient<Predicter>();
            services.AddTransient<TextCleaner>();
            services.AddTransient<VocabularyCorpusBuilder>();
            services.AddTransient<LangModelTrainer>();
            var trainer = services.BuildServiceProvider().GetService<LangModelTrainer>();

            var alphas = new[] { 1, 0.5, 0.2 };
            var length = new[] { 5, 4, 3};
            var backoff = new Dictionary<int, decimal>[]
            {
                new() { { 5, new decimal(0.6) },{ 4, new decimal(0.1) },{ 3, new decimal(0.1) }, { 2, new decimal(0.1) }, { 1, new decimal(0.1) } },
                new() { { 4, new decimal(0.7) },{ 3, new decimal(0.1) }, { 2, new decimal(0.1) }, { 1, new decimal(0.1) } },
                new() { { 3, new decimal(0.8) }, { 2, new decimal(0.1) }, { 1, new decimal(0.1) } }
            };
            
            var devText = File.ReadAllText(@"C:\Users\f98f9\Desktop\devSet.txt");
            var devSentences = devText.Split(".", StringSplitOptions.RemoveEmptyEntries);
            var trainSet = File.ReadAllText(@"C:\Users\f98f9\Desktop\trainSet.txt");
            /*for (int i = 0; i < alphas.Length; i++)
            {
                for (int j = 0; j < length.Length; j++)
                {
                    var model = trainer!.Train(trainSet, new decimal(alphas[i]), length[j], backoff[j]);
                    var results = devSentences.Select(model.Assess);
                    var sum = results.Sum();
                    Console.WriteLine($"{alphas[i]}, {length[j]} -- {sum}");
                }
            }*/
          
            var testSet = File.ReadAllText(@"C:\Users\f98f9\Desktop\testSet.txt");
            var best = trainer!.Train(trainSet, new decimal(0.2), length[2], backoff[2]);
            var supportLine = "викинг имел плоть";
            for (int i = 0; i < 50; i++)
            {
                supportLine += " " + best.PredictNext(supportLine);
            }

            Console.WriteLine(supportLine);
            //var testSentencies = testSet.Split(".", StringSplitOptions.RemoveEmptyEntries);
            
            //Console.WriteLine(testSentencies.Select(best.Assess).Sum());


            /*
            var devModel = trainer.Train(devText, new decimal(1), 3, new Dictionary<int, decimal>()
            {
                { 3, new decimal(0.8) }, { 2, new decimal(0.1) }, { 1, new decimal(0.1) }
            });
            
            
            
            
            Console.WriteLine(devModel.Assess("p p p"));
            Console.WriteLine(devModel.Assess("миша пес барбос"));
            Console.WriteLine(devModel.Assess("он любил зубр"));
            Console.WriteLine(devModel.Assess("он он он"));
            Console.WriteLine(devModel.Assess("и и и"));
            Console.WriteLine(devModel.Assess("и "));*/
            /*Console.WriteLine(devModel.Assess("уготована юной кельтской"));*/


            /*var service = services.BuildServiceProvider().GetService<Lab2>();
            var result = service.Main("нежно он  поцеловал", @"C:\Users\f98f9\Desktop\9268976.txt", 20);
            Console.WriteLine(result);
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000.0);*/
        }
        
    }
}