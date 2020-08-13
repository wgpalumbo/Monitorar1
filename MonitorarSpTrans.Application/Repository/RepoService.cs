using System;
using System.Threading.Tasks;
using MonitorarSpTrans.Application.IRepository;
using Serilog;

namespace MonitorarSpTrans.Application.Repository
{
    public class RepoService : IRepoService
    {
        public RepoService()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Incluir(string oqueincluir, string ondeincluir)
        {
            Log.Error($"{oqueincluir}-{ondeincluir} => Repositorio Não Pronto.");
        }

        protected virtual void Dispose(bool disposing)
        {
        }

    }
}