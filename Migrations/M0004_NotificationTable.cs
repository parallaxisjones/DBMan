using FluentMigrator;
using FluentMigrator.Model;

namespace Playverse.Data.Migrations
{
    [Migration(4,"10/09/2015 Adding the notifications table that is written by sending SNS notifications")]
    public class M0004_NotificationTable : Migration
    {
        public override void Up()
        {
            Execute.Sql(
            @"
CREATE TABLE IF NOT EXISTS Notification(
  Id INT(255) NOT NULL AUTO_INCREMENT,
  GameId VARCHAR(255) NOT NULL DEFAULT '0-000000000000000000000000',  
  Subject VARCHAR(255) DEFAULT NULL,
  Message VARCHAR(255) DEFAULT NULL,
  Topic VARCHAR(255) DEFAULT NULL,
  Level VARCHAR(255) DEFAULT NULL,  
  ARN VARCHAR(255) DEFAULT NULL,    
  CreatedAt DateTime DEFAULT NULL,
  PRIMARY KEY (Id)
);
"
            );
        }

        public override void Down()
        {
            Execute.Sql(
@"
drop table if exists Notification;
"
);
        }
    }
}