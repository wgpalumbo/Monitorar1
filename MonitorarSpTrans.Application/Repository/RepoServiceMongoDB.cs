using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MonitorarSpTrans.Application.IRepository;
using Serilog;

namespace MonitorarSpTrans.Application.Repository
{
    public class RepoServiceMongoDB : IRepoService
    {
        private string mongoDB_Conexao;
        private string mongoDB_Database;

        public RepoServiceMongoDB()
        {
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetConfig(string _mongoDB_Conexao, string _mongoDB_Database)
        {
            mongoDB_Conexao = _mongoDB_Conexao;
            mongoDB_Database = _mongoDB_Database;
        }

        public void Incluir(string oqueincluir, string ondeincluir)
        {
            //Log.Information(oqueincluir);
            var client  = new MongoClient(mongoDB_Conexao);
            var database = client.GetDatabase(mongoDB_Database);            
            var collection = database.GetCollection<BsonDocument>(ondeincluir);
            //BsonDocument document = BsonDocument.Parse(oqueincluir);
            BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(oqueincluir);           
            var notificacao = Task.Run(() => collection.InsertOneAsync(document));
            notificacao.Wait();

        }

        protected virtual void Dispose(bool disposing)
        {
        }

    }
}