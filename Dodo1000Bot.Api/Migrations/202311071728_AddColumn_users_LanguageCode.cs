using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202311071728)]
public class AddColumn_users_LanguageCode: Migration
{
    public override void Up()
    {
        Execute.Sql
        (@"
            ALTER TABLE users 
            ADD COLUMN `LanguageCode` VARCHAR(2) NOT NULL DEFAULT 'en' AFTER `IsAdmin`;
        ");
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}