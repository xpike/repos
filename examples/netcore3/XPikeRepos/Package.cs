using XPike.IoC;

namespace XPikeRepos
{
    public class Package
        : IDependencyPackage
    {
        public void RegisterPackage(IDependencyCollection dependencyCollection)
        {
            dependencyCollection.LoadPackage(new Example.Library.Package());
        }
    }
}