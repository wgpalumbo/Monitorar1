using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MonitorarSpTrans.Application.IService
{
    public interface IWebService : IDisposable
    {
        Task<string> GetServiceAsync(string urlPost, HttpClient httpcliente);
    }
}