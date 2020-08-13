using System;
using System.Net.Http;
using System.Threading.Tasks;
using MonitorarSpTrans.Application.IService;
using Serilog;

namespace MonitorarSpTrans.Application.Service
{
    public class WebServiceSpTransEmpresas : IWebService
    {
        public WebServiceSpTransEmpresas()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
        }

        public async Task<string> GetServiceAsync(string urlPost, HttpClient httpcliente)
        {            
            try
            {
                HttpResponseMessage response2 = await httpcliente.GetAsync(urlPost).ConfigureAwait(false);
                string content2 = await response2.Content.ReadAsStringAsync();
                //Console.WriteLine(content2.ToString());
                //Log.Information($"Result-Consulta={content2.ToString()}");
                return content2;

            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return "";
        }
    }
}