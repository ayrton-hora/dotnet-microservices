namespace Play.Common.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; init; } = null!;

        public string DatabaseName { get; init; } = null!;

        public string EntityCollectionName {get; set;} = null!;
    }
}