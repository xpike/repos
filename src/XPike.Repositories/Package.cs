using XPike.Caching;
using XPike.DataStores;
using XPike.IoC;

namespace XPike.Repositories
{
    public class Package
        : IDependencyPackage
    {
        public void RegisterPackage(IDependencyCollection dependencyCollection)
        {
            dependencyCollection.AddXPikeDataStores();
            dependencyCollection.AddXPikeCaching();

            dependencyCollection.RegisterSingleton<NullRepositoryDataSource, NullRepositoryDataSource>();
            dependencyCollection.RegisterSingleton<IRepositorySettingsManager, RepositorySettingsManager>();
        }
    }
}