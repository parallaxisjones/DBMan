using FluentMigrator;

namespace Playverse.Data.Migrations
{
    [Migration(1)]
    public class M0001_InitializeDatabase : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Economy_PurchaseBreakdownRaw").Exists())
            {
                Execute.EmbeddedScript("initTables.sql");
                
            }
        }

        public override void Down()
        {
            Delete.Table("Economy_PurchaseBreakdownRaw");
            Delete.Table("EventLog");
            Delete.Table("GameAverageSessionLength");
            Delete.Table("GameMonitoringConfig");
            Delete.Table("GameSessionMeta");
            Delete.Table("GameSessionUserStats");
            Delete.Table("GameSessionUserStats_12hour");
            Delete.Table("GameSessionUserStats_15min");
            Delete.Table("GameSessionUserStats_24hour");
            Delete.Table("GameSessionUserStats_30min");
            Delete.Table("GameSessionUserStats_5min");
            Delete.Table("GameSessionUserStats_6hour");
            Delete.Table("GameSessionUserStats_hour");
            Delete.Table("GameUserActivity");
            Delete.Table("GameUserActivity_12hour");
            Delete.Table("GameUserActivity_15min");
            Delete.Table("GameUserActivity_24hour");
            Delete.Table("GameUserActivity_30min");
            Delete.Table("GameUserActivity_5min");
            Delete.Table("GameUserActivity_6hour");
            Delete.Table("GameUserActivity_hour");
            Delete.Table("HostingInstance_ComputeRaw"); 
            Delete.Table("HostingInstanceMeta");
            Delete.Table("MonitoringStats");
            Delete.Table("Retention");
            Delete.Table("Retention_DailyCounts");
            Delete.Table("Retention_FirstLogin");
            Delete.Table("Retention_Login");
            Delete.Table("Retention_ReturnersView");
            Delete.Table("Retention_TwoWeekView");
            Delete.Table("UserSessionMeta");
        }
    }
}
