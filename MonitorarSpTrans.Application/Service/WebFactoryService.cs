using System;
using MonitorarSpTrans.Application.IRepository;
using MonitorarSpTrans.Application.IService;
using MonitorarSpTrans.Application.Repository;

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

        public object ServiceSelector(string qual)
        {
            switch (qual)
            {                
                case "lerweb": return (IWebService)new WebServiceSpTransEmpresas();
                case "mongodb": return (IRepoService)new RepoServiceMongoDB();
                default: return null;
            }

        }
    }
}