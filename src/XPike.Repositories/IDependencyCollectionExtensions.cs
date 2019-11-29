using XPike.IoC;

namespace XPike.Repositories
{
    public static class IDependencyCollectionExtensions
    {
        public static IDependencyCollection AddXPikeRepositories(this IDependencyCollection collection) =>
            collection.LoadPackage(new XPike.Repositories.Package());
    }
}