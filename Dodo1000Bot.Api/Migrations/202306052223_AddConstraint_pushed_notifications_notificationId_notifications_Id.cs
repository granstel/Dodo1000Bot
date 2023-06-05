using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202306052223)]
public class AddConstraint_pushed_notifications_notificationId_notifications_Id: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE pushed_notifications
            ADD CONSTRAINT `pushed_notifications_notificationId_notifications_Id`
              FOREIGN KEY (`NotificationId`)
              REFERENCES notifications (`Id`)
              ON DELETE NO ACTION
              ON UPDATE NO ACTION;
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}