using FluentMigrator;
using FluentMigrator.Model;

namespace Playverse.Data.Migrations
{
    [Migration(3,"10/06/2015 Additional retention metric columns for NURR CURR RURR")]
    public class M0003_RetentionMetricAdditions : Migration
    {
        public override void Up()
        {
            Execute.Sql(
            @"
alter table Retention add column WAU int(11) after Logins;
alter table Retention add column NUR int(11) after WAU;
alter table Retention add column CUR int(11) after NUR;
alter table Retention add column RUR int(11) after CUR;
"
            );
        }

        public override void Down()
        {
            Execute.Sql(
@"
alter table Retention drop column WAU;
alter table Retention drop column NUR;
alter table Retention drop column CUR;
alter table Retention drop column RUR;
"
);
        }
    }
}