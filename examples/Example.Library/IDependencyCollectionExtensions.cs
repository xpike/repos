using XPike.IoC;

namespace Example.Library
{
    public static class IDependencyCollectionExtensions
    {
        public static IDependencyCollection AddExampleLibrary(this IDependencyCollection collection) =>
            collection.LoadPackage(new Example.Library.Package());
    }
}