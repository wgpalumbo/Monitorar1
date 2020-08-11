using MonitorarSpTrans.Application.IService;

namespace MonitorarSpTrans.Application.Service
{
    public class WebMontaUri : IWebMontaUri
    {
        //1o arg = qualServiço
        //2o arg = qualData

        public string GetUriMontadaServiceAsync(string qualServico)
        {
            switch (qualServico)
            {
                case "posicaogps": return "Posicao/Garagem?codigoEmpresa=38";
                case "empresa": return "Empresa";
                case "termosbusca": return "Linha/Buscar?termosBusca=8000";                
                default: return "";

            }

        }
    }
}