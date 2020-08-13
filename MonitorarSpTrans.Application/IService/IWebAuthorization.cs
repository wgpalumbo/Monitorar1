using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace MonitorarSpTrans.Application.IService
{
    public interface IWebAuthorization : IDisposable
    {
        Task<object> GetAutorizacaoTokenAsync(Tuple<string, string> tokenLiberacao, HttpClient httpClient);
    }
}