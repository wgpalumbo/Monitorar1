using System;

namespace MonitorarSpTrans.Application.IRepository
{
    public interface IRepoService : IDisposable
    {
        void Incluir(string oqueincluir, string ondeincluir);
    }
}