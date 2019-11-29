using Example.Library.Caching;
using Example.Library.DataStores;
using Example.Library.DataStores.MySql;
using Example.Library.Repositories;
using XPike.IoC;
using XPike.Repositories;

namespace Example.Library
{
    public class Package
        : IDependencyPackage
    {
        public void RegisterPackage(IDependencyCollection dependencyCollection)
        {
            dependencyCollection.AddXPikeRepositories();

            dependencyCollection.RegisterSingleton<IMySqlUserDataStore, MySqlUserDataStore>();
            dependencyCollection.RegisterSingleton<IUserDataStore>(collection =>
                collection.ResolveDependency<IMySqlUserDataStore>());

            dependencyCollection.RegisterSingleton<IUserCache, UserCache>();
            dependencyCollection.RegisterSingleton<IUserRepository, UserRepository>();
        }
    }
}