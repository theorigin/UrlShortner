using System;
using LiteDB;

namespace UrlShortner.DataAccess
{
    public interface IUrlRepository
    {
        string? GetById(string id);
        void Save(string id, string url);
    }

    public class UrlRepository : IUrlRepository
    {
        private readonly ILiteDatabase _liteDatabase;

        public UrlRepository(ILiteDatabase liteDatabase)
        {
            if (liteDatabase == null)
                throw new ArgumentNullException(nameof(liteDatabase));

            _liteDatabase = liteDatabase;
        }

        public string? GetById(string id)
        {            
            var urlDocument = GetUrlCollection().FindById(id);
            
            return urlDocument == null 
                ? null 
                : urlDocument["value"];
        }

        public void Save(string id, string url)
        {
            var urlDocument = new BsonDocument
            {
                ["_id"] = id,
                ["value"] = url
            };

            GetUrlCollection().Insert(urlDocument);
        }

        private ILiteCollection<BsonDocument> GetUrlCollection()
        {
            return _liteDatabase.GetCollection("Url");
        }
    }
}