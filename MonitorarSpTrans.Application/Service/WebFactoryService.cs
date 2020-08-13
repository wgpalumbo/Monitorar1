using System;
using MonitorarSpTrans.Application.IService;

namespace MonitorarSpTrans.Application.Service
{
    public class WebFactoryService : IWebFactoryService
    {

        public WebFactoryService()
        {
        }
        
        // teria algum motivo para manter o dispose nessa classe ?  
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
        }

        // Está interessante a aplicação da factory aqui, porém imagine que a WebServiceSpTransEmpresas dependa de dois repositorios, como voce faria ?
        
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
