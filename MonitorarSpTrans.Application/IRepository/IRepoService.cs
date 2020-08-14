using System;

namespace MonitorarSpTrans.Application.IRepository
{
    public interface IRepoService : IDisposable
    {
        void SetConfig(string _mongoDB_Conexao, string _mongoDB_Database);
        void Incluir(string oqueincluir, string ondeincluir);
    }
}