using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace MonitorarSpTrans.Application.IService
{
    public interface IWebAuthorization : IDisposable
    {
        Task<string> GetTokenAsync(Tuple<string, string> tokenLiberacao, HttpClient httpClient);
    }
}