using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MonitorarSpTrans.Application.IService;
using MonitorarSpTrans.Application.Service;
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
                Console.WriteLine("Por Favor, Indique o tipo de Serviço Externo e a Data desejada.");
                Console.WriteLine($"Acrescentar: dd/mm/yyyy serviço");
                return;
            }

            DateTime tempDate;
            if (!DateTime.TryParse(args[0], out tempDate))
            {
                Console.WriteLine($"{args[0]} => Por Favor, Informe a data corretamente.");
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
                string[] comandosArray = configuration.GetSection("sptrans:servicos").Get<string[]>();
                if (!comandosArray.Any(x => x == qualServico))
                {
                    Console.WriteLine($"{qualServico}? => Por Favor, Informe uma operação valida.");
                    return;
                }


                using (IWebAuthorization webAuthorization = serviceProvider.GetService<IWebAuthorization>())
                {
                    // Create Cliente Http From Factory
                    var httpcliente = httpClientFactory.CreateClient("");
                    httpcliente.DefaultRequestHeaders.Accept.Add(item: new MediaTypeWithQualityHeaderValue("application/json"));
                    httpcliente.BaseAddress = new Uri(configuration.GetConnectionString("urlBase").ToString());

                    string tmpUrlUri = configuration.GetConnectionString("urlAutorizacao").ToString();
                    string tmpToken = configuration.GetConnectionString("tokenkey").ToString();
                    var tokenLiberacao = new Tuple<string, string>(tmpUrlUri, tmpToken);
                    
                    string token = await webAuthorization.GetTokenAsync(tokenLiberacao, httpcliente).ConfigureAwait(false);
                                       
                    if (!String.IsNullOrEmpty(token))
                    {
                        if (!token.ToLower().Equals("true"))
                        {
                            Console.WriteLine($"{args[1]}-{args[0]} => Serviço Não Autorizado.");
                            return;
                        }
                    }
                    Console.WriteLine(token);

                    // if (webAuthorization.ObterAutorizacao())
                    // {
                    //     string resposta = webAuthorization.GetResposta();
                    //     Console.WriteLine(resposta);
                    //     using (IWebService webService = new WebFactoryService(httpcliente).ServiceSelector(qualServico))
                    //     {
                    //         webService.GetServiceAsync(resposta, qualData, qualConsulta);
                    //     }

                    // }
                    // else
                    // {
                    //     Console.WriteLine($"{args[0]} => Serviço Não Autorizado.");
                    //     return;
                    // }

                }



                // Print connection string to demonstrate configuration object is populated
                Console.WriteLine(configuration.GetConnectionString("DbConnection"));

                var dadosConfig = configuration.GetSection("sptrans").GetValue<string>("urlBase");
                Console.WriteLine(dadosConfig);

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error running service");
                throw ex;
            }
            finally
            {
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

        }



        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Add logging
            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder
                    .AddSerilog(dispose: true);
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
            serviceCollection.AddSingleton<IWebAuthorization, WebAuthorizationSpTrans>();

            // Add app
            //serviceCollection.AddTransient<App>();
        }
    }
}
