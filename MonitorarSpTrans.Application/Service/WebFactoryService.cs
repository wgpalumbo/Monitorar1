using System;
using MonitorarSpTrans.Application.IService;

namespace MonitorarSpTrans.Application.Service
{
    public class WebFactoryService : IWebFactoryService
    {

        public WebFactoryService()
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

        public IWebService ServiceSelector(string qual)
        {
            switch (qual)
            {                
                case "lerweb": return new WebServiceSpTransEmpresas();
                default: return null;
            }

        }
    }
}