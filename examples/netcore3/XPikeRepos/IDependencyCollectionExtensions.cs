using XPike.IoC;

namespace XPikeRepos
{
    public static class IDependencyCollectionExtensions
    {
        public static IDependencyCollection AddXPikeRepos(this IDependencyCollection dependencyCollection) =>
            dependencyCollection.LoadPackage(new XPikeRepos.Package());
    }
}