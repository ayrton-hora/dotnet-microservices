using System.Linq.Expressions;

using MongoDB.Driver;

namespace Play.Common.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T: IEntity
    {
        private readonly IMongoCollection<T> _dbCollection;

        private readonly FilterDefinitionBuilder<T> FilterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await _dbCollection.Find(FilterBuilder.Empty).ToListAsync();
        }
        
        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbCollection.Find(filter).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = FilterBuilder.Eq(e => e.Id, id);
            return await _dbCollection.Find(filter).FirstOrDefaultAsync();
        }
        
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            else await _dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            else
            {
                FilterDefinition<T> filter = FilterBuilder.Eq(e => e.Id, entity.Id);
                await _dbCollection.ReplaceOneAsync(filter, entity);
            }
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = FilterBuilder.Eq(e => e.Id, id);
            await _dbCollection.DeleteOneAsync(filter);
        }
    }
}