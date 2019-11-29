using Example.Library.Caching;
using Example.Library.DataStores;
using Example.Library.DataStores.MySql;
using Example.Library.Repositories;
using XPike.IoC;

namespace Example.Library
{
    public class Package
        : IDependencyPackage
    {
        public void RegisterPackage(IDependencyCollection dependencyCollection)
        {
            dependencyCollection.LoadPackage(new XPike.Repositories.Package());

            dependencyCollection.RegisterSingleton<IMySqlUserDataStore, MySqlUserDataStore>();
            dependencyCollection.RegisterSingleton<IUserDataStore>(collection =>
                collection.ResolveDependency<IMySqlUserDataStore>());

            dependencyCollection.RegisterSingleton<IUserCache, UserCache>();
            dependencyCollection.RegisterSingleton<IUserRepository, UserRepository>();
        }
    }
}