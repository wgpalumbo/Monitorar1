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
            Task<string> retorno = Task.FromResult(string.Empty);
            try
            {

                string urlPost = tokenLiberacao.Item1;
                string keyPost = tokenLiberacao.Item2;

                //var byteArray = Encoding.UTF8.GetBytes(keyPost);

                // var content = new StringContent(keyPost, Encoding.UTF8, "application/json");
                // content.Headers.Add("Content-Length", keyPost.Length.ToString());
                // content.Headers.ContentLength = keyPost.Length;

                HttpResponseMessage response = await httpClient.PostAsync(urlPost + keyPost, null).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();

                // HttpResponseMessage response2 = await _httpcliente.GetAsync("http://api.olhovivo.sptrans.com.br/v2.1/Empresa").ConfigureAwait(false);
                // string content2 = await response2.Content.ReadAsStringAsync();
                // Console.WriteLine(content2.ToString());

                return result.ToString().ToLower().Equals("true");

            }
            catch (Exception e) { Console.WriteLine(e.Message); }

            return false;
        }
    }
}