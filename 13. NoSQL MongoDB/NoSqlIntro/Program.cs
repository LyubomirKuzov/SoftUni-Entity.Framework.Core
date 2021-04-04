using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace NoSqlIntro
{
    class Program
    {
        static void Main(string[] args)
        {
            //MongoDB!!!

            var client = new MongoClient("mongodb://127.0.0.1:27017");

            var database = client.GetDatabase("MongoDBIntro");

            var collection = database.GetCollection<BsonDocument>("Articles");

            //01.Read

            List<BsonDocument> articles = collection.Find(new BsonDocument { }).ToList();

            foreach (var article in articles)
            {
                string name = article.GetElement("name").Value.ToString();

                Console.WriteLine(name);
            }

            //02.Create a new article

            collection.InsertOne(new BsonDocument()
            {
                {"author", "Steve Jobs" },
                {"date", "05-05-2005" },
                {"name", "The story of Apple" },
                {"rating", "60" }
            });

            // 03. Update

            foreach (var article in articles)
            {
                string name = article.GetElement("name").Value.ToString();

                int newRating = int.Parse(article.GetElement("rating").Value.AsString) + 10;

                var filterQuery = Builders<BsonDocument>
                    .Filter
                    .Eq("_id", article.GetElement("_id").Value);

                var updateQuery = Builders<BsonDocument>
                    .Update
                    .Set("rating", newRating.ToString());

                var item = collection.UpdateOne(filterQuery, updateQuery);

                Console.WriteLine($"{name} : rating: {article.GetElement("rating").Value}");
            }

            // 04. Delete

            var deleteQuery = Builders<BsonDocument>.Filter.Lt("rating", 50);
            collection.DeleteMany(deleteQuery);
        }
    }
}
