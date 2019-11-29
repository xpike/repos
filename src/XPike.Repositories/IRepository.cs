namespace XPike.Repositories
{
    public interface IRepository
    {
    }

    public interface IRepository<TDataSource>
        : IRepository
    {
    }
}