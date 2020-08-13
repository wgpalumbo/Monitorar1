using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MonitorarSpTrans.Application.IRepository;
using MonitorarSpTrans.Application.IService;
using MonitorarSpTrans.Application.Repository;
using MonitorarSpTrans.Application.Service;
using Newtonsoft.Json;
using Serilog;

namespace MonitorarSpTrans.Application
{
    class MonitorarSpTrans
    {
        public static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
             .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
             .MinimumLevel.Debug()
             .Enrich.FromLogContext()
             .CreateLogger();

            try
            {
                MainAsync(args).Wait();
                return;
            }
            catch (Exception e)
            {
                Log.Information("Não Foi Possivel Execuatar o Aplicativo !");
                Log.Information(e.Message);
                return;
            }
        }

        static async Task MainAsync(string[] args)
        {

            if (args.Length != 2)
            {
                Log.Error("Por Favor, Indique a Data desejada e o o tipo de Serviço.");
                Log.Error($"Acrescentar: dd/mm/yyyy serviço");
                //Console.WriteLine("Por Favor, Indique o tipo de Serviço Externo e a Data desejada.");
                //Console.WriteLine($"Acrescentar: dd/mm/yyyy serviço");
                return;
            }

            DateTime tempDate;
            if (!DateTime.TryParse(args[0], out tempDate))
            {
                Log.Error($"{args[0]} => Por Favor, Informe a data corretamente.");
                //Console.WriteLine($"{args[0]} => Por Favor, Informe a data corretamente.");
                return;
            }
            string qualData = tempDate.ToString("dd/MM/yyyy");
            string qualServico = args[1].ToLower();

            // Create service collection
            Log.Information("Creating service collection");
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Log.Information("Building service provider");
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider(true);
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            try
            {
                // Verificando se as palavras de entrada via teclado estao certas                
                Dictionary<string, string> settingsWebInfo = configuration.GetSection("sptrans:servicos").Get<Dictionary<string, string>>();
                string[] servicosArray = settingsWebInfo.Keys.ToArray();

                if (!servicosArray.Any(x => x == qualServico))
                {
                    Log.Error($"{qualServico}? => Por Favor, Informe uma operação valida.");
                    //Console.WriteLine($"{qualServico}? => Por Favor, Informe uma operação valida.");
                    return;
                }

                // Iniciando um namespace IWebAuthorization
                using (IWebAuthorization webAuthorization = serviceProvider.GetService<IWebAuthorization>())
                {
                    // Create Cliente Http From Factory
                    var httpcliente = httpClientFactory.CreateClient("sptrans");

                    // Obtendo paramentros de Autorização
                    string tmpUrlUri = configuration.GetConnectionString("urlAutorizacao").ToString();
                    string tmpToken = configuration.GetConnectionString("tokenkey").ToString();
                    var tokenLiberacao = new Tuple<string, string>(tmpUrlUri, tmpToken);

                    if (!Convert.ToBoolean(await webAuthorization.GetAutorizacaoTokenAsync(tokenLiberacao, httpcliente).ConfigureAwait(false)))
                    {
                        Log.Error($"{args[1]}-{args[0]} => Serviço Não Autorizado.");
                        //Console.WriteLine($"{args[1]}-{args[0]} => Serviço Não Autorizado.");
                        return;
                    }

                    // Obtendo Serviço da Fabrica
                    using (IWebService webService = serviceProvider.GetService<IWebFactoryService>().ServiceSelector("lerweb"))
                    {
                        string uriServico = settingsWebInfo[qualServico];
                        string resultado = await webService.GetServiceAsync(uriServico, httpcliente);
                        //Armazenando
                        using (IRepoService repoService = serviceProvider.GetService<IRepoService>())
                        {
                            repoService.Incluir(resultado.ToString(), qualServico);

                        }

                    }

                    Log.Information($"Finalizado Consulta {qualServico}-{qualData}");
                }

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error running service");
                throw ex;
            }
            finally
            {
                Log.Information("Encerrando App");
                Log.CloseAndFlush();

            }


            // try
            // {
            //     Log.Information("Starting service");
            //     await serviceProvider.GetService<App>().Run();
            //     Log.Information("Ending service");
            // }
            // catch (Exception ex)
            // {
            //     Log.Fatal(ex, "Error running service");
            //     throw ex;
            // }
            // finally
            // {
            //     Log.CloseAndFlush();
            // }

            // Verificando Uri-Base
            //Log.Information("Uri-Base:" + configuration.GetSection("sptrans").GetValue<string>("urlBase"));



        }



        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(dispose: true);
            }));

            serviceCollection.AddLogging();

            //Console.WriteLine(Directory.GetCurrentDirectory());

            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();


            // Add access to generic IConfigurationRoot
            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);

            serviceCollection.AddHttpClient("");
            serviceCollection.AddHttpClient("sptrans", c =>
            {
                c.BaseAddress = new Uri(configuration.GetConnectionString("urlBase").ToString());
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                //c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
            });
            serviceCollection.AddSingleton<IWebAuthorization, WebAuthorizationSpTrans>();
            serviceCollection.AddSingleton<IWebFactoryService, WebFactoryService>();
            serviceCollection.AddSingleton<IRepoService, RepoService>();


            // Add app
            //serviceCollection.AddTransient<App>();
        }


    }
}
