using FluentMigrator;
using FluentMigrator.Model;

namespace Playverse.Data.Migrations
{
    [Migration(2,"8/26/2015 Creating table for Whale Report")]
    public class M0002_Economy_GameCreditTransactions : Migration
    {
        public override void Up()
        {
            Execute.Sql(
@"
CREATE TABLE `Economy_GameCreditTransactions` (
  `RecordTimestamp` datetime NOT NULL,
  `TransactionId` varchar(36) NOT NULL,
  `UserId` varchar(36) NOT NULL,
  `GameId` varchar(36) NOT NULL,
  `ExternalOnlineService` varchar(255) NOT NULL,
  `ThirdPartyOrderId` varchar(255) NOT NULL,
  `Credits` int(11) NOT NULL,
  `PaymentProvider` int(11) NOT NULL,
  `PaymentTransactionId` varchar(255) NOT NULL,
  `TransactionType` int(11) NOT NULL,
  `CreditPackId` varchar(36) NOT NULL,
  `UserData` mediumtext,
  `Description` text,
  `CostAmount` decimal(20,2) NOT NULL,
  `Status` int(11) NOT NULL,
  `CreatedOn` datetime NOT NULL,
  `UpdatedOn` datetime NOT NULL,
  `Category` varchar(255) DEFAULT '',
  `ClientKey` varchar(36) DEFAULT NULL,
  PRIMARY KEY (`TransactionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
"
);
        }

        public override void Down()
        {
            Delete.Table("Economy_GameCreditTransactions");
        }
    }
}