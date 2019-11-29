using Example.Library.DataStores.MySql.Records;
using Example.Library.Models;
using XPike.Contracts;

namespace Example.Library.DataStores.MySql
{
    public class MySqlUserRecordMapper
        : IMapRecord<MySqlUserRecord, User>,
            IMapModel<User, MySqlUserRecord>
    {
        public User Map(MySqlUserRecord input) =>
            input == null
                ? null
                : new User
                {
                    UserId = input.UserId,
                    Username = input.Username
                };

        public MySqlUserRecord Map(User input) =>
            input == null
                ? null
                : new MySqlUserRecord
                {
                    UserId = input.UserId,
                    Username = input.Username
                };
    }
}