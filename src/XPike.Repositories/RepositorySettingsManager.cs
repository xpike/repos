using XPike.Settings;

namespace XPike.Repositories
{
    public class RepositorySettingsManager 
        : IRepositorySettingsManager
    {
        private readonly ISettingsService _settingsService;

        public RepositorySettingsManager(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public ISettings<RepositorySettings<TImplementation>> GetRepositorySettings<TImplementation>()
            where TImplementation : IRepository =>
            _settingsService.GetSettings<RepositorySettings<TImplementation>>(typeof(TImplementation).FullName);
    }
}