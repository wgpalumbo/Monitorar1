using System;

namespace MonitorarSpTrans.Application.IService
{
    public interface IWebFactoryService : IDisposable
    {
        IWebService ServiceSelector(string qual);
    }
}