using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202308290945)]
public class DropForeignKeyToUserId: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE pushed_notifications 
            DROP FOREIGN KEY `pushed_notifications_userId_users_Id`;
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}