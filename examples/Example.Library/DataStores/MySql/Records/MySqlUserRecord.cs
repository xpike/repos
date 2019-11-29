using XPike.Contracts;

namespace Example.Library.DataStores.MySql.Records
{
    public class MySqlUserRecord
        : IRecord
    {
        public int UserId { get; set; }
        
        public string Username { get; set; }
    }
}