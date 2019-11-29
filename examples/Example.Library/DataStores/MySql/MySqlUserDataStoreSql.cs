namespace Example.Library.DataStores.MySql
{
    public static class MySqlUserDataStoreSql
    {
        public const string GET_ALL_USERS_SQL = @"
SELECT
    *
FROM
    users.user;
";

        public const string GET_USER_SQL = @"
SELECT
    *
FROM
    users.user
WHERE
    userId = @userId;
";

        public const string CREATE_USER_SQL = @"
INSERT INTO users.user (
    Username
)
VALUES (
    @Username
);
SELECT LAST_INSERT_ID();";

        public const string UPDATE_USER_SQL = @"
UPDATE
    users.user
SET
    Username = @Username
WHERE
    UserId = @UserId;
SELECT ROW_COUNT();";

        public const string DELETE_USER_SQL = @"
DELETE FROM
    users.user
WHERE
    UserId = @UserId;
SELECT ROW_COUNT();";
    }
}