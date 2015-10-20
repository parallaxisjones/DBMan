namespace Playverse.Data
{

    public class StagingDatabase : DBConnection
    {
        public StagingDatabase(Datastore ds)
        {
            this.DataStore = ds;
            this.Environment = DBEnvironment.Staging;
            switch (this.DataStore)
            {
                case Datastore.Monitoring:
                    this.User = "playverseAdmin";
                    this.Database = "Moniverse";
                    this.Password = "C5ebresWequ8Ef=deTH6vast";
                    this.Server = "staging-monitoring.cegaxwwurgzs.us-east-1.rds.amazonaws.com";
                    break;
                case Datastore.General:
                    this.User = "playverseAdmin";
                    this.Database = "playverseDB";
                    this.Password = "C5ebresWequ8Ef=deTH6vast";
                    this.Server = "pvdb-prod-general-s0.cegaxwwurgzs.us-east-1.rds.amazonaws.com";
                    break;
            }
            SetConnectionString();
        }
    }

    public class LocalDatabase : DBConnection
    {
        public LocalDatabase(Datastore ds)
        {
            this.Environment = DBEnvironment.Local;
            this.DataStore = ds;
            switch (this.DataStore)
            {
                case Datastore.Monitoring:
                    this.User = "root";
                    this.Password = "admin";
                    this.Database = "Moniverse";
                    this.Server = "instinctserver";
                    break;
                case Datastore.General:
                    this.User = "playverseAdmin";
                    this.Password = "C5ebresWequ8Ef=deTH6vast";
                    this.Database = "playverseDB";
                    this.Server = "pvdb-prod-general-s0.cegaxwwurgzs.us-east-1.rds.amazonaws.com";
                    break;
                case Datastore.Validation:
                    this.User = "root";
                    this.Password = "admin";
                    this.Database = "ValidationTests";
                    this.Server = "instinctserver";
                    break;
                default:
                    this.User = "root";
                    this.Password = "admin";
                    this.Database = "playverseDB";
                    this.Server = "instinctserver";
                    break;
            }
            SetConnectionString();
        }
    }

    public class ProductionDatabase : DBConnection
    {
        public ProductionDatabase(Datastore ds)
        {
            this.Environment = DBEnvironment.Production;
            this.DataStore = ds;
            switch (this.DataStore)
            {
                case Datastore.Monitoring:
                    this.User = "playverseAdmin";
                    this.Password = "fMZTYts4Yyjakiz";
                    this.Database = "Moniverse";
                    this.Server = "prod-monitoring.cegaxwwurgzs.us-east-1.rds.amazonaws.com";
                    break;
                case Datastore.General:
                    this.User = "playverseAdmin";
                    this.Password = "C5ebresWequ8Ef=deTH6vast";
                    this.Database = "playverseDB";
                    this.Server = "pvdb-prod-general-s0.cegaxwwurgzs.us-east-1.rds.amazonaws.com";
                    break;
                default:
                    this.Server = "instinctserver";
                    break;
            }
            SetConnectionString();
        }
    }
}
