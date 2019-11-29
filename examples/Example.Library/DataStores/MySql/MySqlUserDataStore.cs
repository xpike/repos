using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Example.Library.DataStores.MySql.Records;
using Example.Library.Models;
using XPike.DataStores;
using XPike.DataStores.Dapper;
using XPike.DataStores.MySql;

namespace Example.Library.DataStores.MySql
{
    public class MySqlUserDataStore
        : DapperDataStoreBase,
            IMySqlUserDataStore
    {
        private const int _DEFAULT_GET_TIMEOUT_SEC = 10;

        private static readonly MySqlUserRecordMapper _recordMapper = new MySqlUserRecordMapper();

        protected override string ConnectionString => "UserDb";

        public MySqlUserDataStore(IMySqlDataConnectionProvider provider,
            ISettingsConnectionStringManager connectionStringManager)
            : base(provider, connectionStringManager)
        {
        }

        public Task<User> GetUserAsync(int userId, TimeSpan? timeout = null, CancellationToken? ct = null) =>
            WithSqlConnectionAsync(async connection =>
                _recordMapper.Map((await connection.QueryAsync<MySqlUserRecord>(MySqlUserDataStoreSql.GET_USER_SQL,
                        new
                        {
                            userId
                        },
                        commandTimeout: (int) timeout.GetValueOrDefault(TimeSpan.FromSeconds(_DEFAULT_GET_TIMEOUT_SEC))
                            .TotalSeconds))
                    .SingleOrDefault()));

        public Task<bool> CreateUserAsync(User user, TimeSpan? timeout = null, CancellationToken? ct = null) =>
            WithSqlConnectionAsync(async connection =>
                (await connection.QueryAsync<int?>(MySqlUserDataStoreSql.CREATE_USER_SQL,
                    _recordMapper.Map(user),
                    commandTimeout: (int?) timeout?.TotalSeconds))
                .SingleOrDefault()
                .GetValueOrDefault() == 1);

        public Task<bool> UpdateUserAsync(User user, TimeSpan? timeout = null, CancellationToken? ct = null) =>
            WithSqlConnectionAsync(async connection =>
                (await connection.QueryAsync<int?>(MySqlUserDataStoreSql.UPDATE_USER_SQL,
                    _recordMapper.Map(user),
                    commandTimeout: (int?) timeout?.TotalSeconds))
                .SingleOrDefault()
                .GetValueOrDefault() == 1);

        public Task<bool> DeleteUserAsync(int userId, TimeSpan? timeout = null, CancellationToken? ct = null) =>
            WithSqlConnectionAsync(async connection =>
                (await connection.QueryAsync<int?>(MySqlUserDataStoreSql.DELETE_USER_SQL,
                    new
                    {
                        userId
                    },
                    commandTimeout: (int?) timeout?.TotalSeconds))
                .SingleOrDefault()
                .GetValueOrDefault() == 1);
    }
}