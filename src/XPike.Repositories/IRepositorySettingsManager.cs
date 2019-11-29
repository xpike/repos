using XPike.Settings;

namespace XPike.Repositories
{
    public interface IRepositorySettingsManager
    {
        ISettings<RepositorySettings<TImplementation>> GetRepositorySettings<TImplementation>()
            where TImplementation : IRepository;
    }
}