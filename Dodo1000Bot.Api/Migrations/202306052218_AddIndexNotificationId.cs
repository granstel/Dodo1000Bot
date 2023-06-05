using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202306052218)]
public class AddIndexNotificationId: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE pushed_notifications 
                ADD INDEX `NotificationsId` (`NotificationId` ASC) VISIBLE;
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}