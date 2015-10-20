using FluentMigrator;
using FluentMigrator.Model;

namespace Playverse.Data.Migrations
{
    [Migration(5,"10/14/2015 Adding the user table")]
    public class M0005_UserTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(
            @"
CREATE TABLE IF NOT EXISTS `User` (   `Id` varchar(255) NOT NULL,   `Username` varchar(255) NOT NULL,   `PasswordHash` varchar(255) NOT NULL,   `Email` varchar(255) NOT NULL,   `LastNotificationSeen` int(11) DEFAULT NULL,   `DashContent` blob ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
ALTER TABLE User
ADD CONSTRAINT uk_UserID UNIQUE (Id, Email);
insert into User values ('8479eba7-576c-4cc7-aca5-3bc9453895a5', 'playtricsuser', 'AHkkCJ80eLmj8eukgY3bEYc2LSl7DqEEZv8iLqAb9d3OTGCM1EFSjqSGwjbknhbbeQ==','parker.jones@trendyent.com', null, null);
"
            );
        }

        public override void Down()
        {
            Execute.Sql(
@"
drop table if exists User;
"
);
        }
    }
}