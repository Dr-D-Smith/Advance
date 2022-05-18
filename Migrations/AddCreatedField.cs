using System;
using FluentMigrator;


namespace AdvanceTest.Migrations
{
    [Migration(202205170001)]
    public class AddCreatedField : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("ExplanatoryNote").Column("CreatedDate").Exists())
            {
                this.Create.Column("CreatedDate").OnTable("ExplanatoryNote").AsDate();
            }
        }

        public override void Down()
        {
            
        }
    }
}