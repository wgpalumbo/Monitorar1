using System;
using System.Net.Http;
using System.Threading.Tasks;
using MonitorarSpTrans.Application.IService;

namespace MonitorarSpTrans.Application.Service
{
    public class WebAuthorizationSpTrans : IWebAuthorization
    {
        public WebAuthorizationSpTrans()
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

        public async Task<object> GetAutorizacaoTokenAsync(Tuple<string, string> tokenLiberacao, HttpClient httpClient)
        {

            try
            {

                string urlPost = tokenLiberacao.Item1;
                string keyPost = tokenLiberacao.Item2;

                HttpResponseMessage response = await httpClient.PostAsync(urlPost + keyPost, null).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                // Console.WriteLine(result.ToString());
                return result.ToString().ToLower().Equals("true");

            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return false;
        }
    }
}