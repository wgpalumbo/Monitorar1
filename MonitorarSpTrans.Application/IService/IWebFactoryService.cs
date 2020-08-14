using System;

namespace MonitorarSpTrans.Application.IService
{
    public interface IWebFactoryService : IDisposable
    {
        object ServiceSelector(string qual);
    }
}