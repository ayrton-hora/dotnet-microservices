using MongoDB.Driver;

using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public class ItemsRepository
    {
        private const string CollectionName = "items";

        private readonly IMongoCollection<Item> DbCollection;

        private readonly FilterDefinitionBuilder<Item> FilterBuilder = Builders<Item>.Filter;

        public ItemsRepository()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("Catalog");
            DbCollection = database.GetCollection<Item>(CollectionName);
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            return await DbCollection.Find(FilterBuilder.Empty).ToListAsync();
        }

        public async Task<Item> GetAsync(Guid id)
        {
            FilterDefinition<Item> filter = FilterBuilder.Eq(entity => entity.Id, id);
            return await DbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            else await DbCollection.InsertOneAsync(item);
        }

        public async Task UpdateAsync(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            else 
            {
                FilterDefinition<Item> filter = FilterBuilder.Eq(entity => entity.Id, item.Id);
                await DbCollection.ReplaceOneAsync(filter, item);
            }
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<Item> filter = FilterBuilder.Eq(entity => entity.Id, id);
            await DbCollection.DeleteOneAsync(filter);
        }
    }
}