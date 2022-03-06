using MongoDB.Driver;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Play.Common.Settings;

namespace Play.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            services.AddSingleton(seviceProvider => 
            {
                var configuration = seviceProvider.GetService<IConfiguration>();
                if (configuration == null) throw new ArgumentNullException(nameof(configuration));
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                
                return mongoClient.GetDatabase(mongoDbSettings.DatabaseName);
            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T: IEntity
        {
            services.AddSingleton<IRepository<T>>(seviceProvider => 
            {
                var database = seviceProvider.GetService<IMongoDatabase>();
                if (database == null) throw new ArgumentNullException(nameof(database));
                return new MongoRepository<T>(database, collectionName);
            });

            return services;
        }
    }
}