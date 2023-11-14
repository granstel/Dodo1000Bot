using Dodo1000Bot.Models;
using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202311142035)]
public class AddNewUnitsTemplates: Migration
{
    public override void Up()
    {
        var templates = new TemplateMigrationParameter[]
        {
            new()
            {
                NotificationType = NotificationType.NewUnit,
                MessengerType = Source.Telegram,
                LanguageCode = "en",
                Template =
                    "Wow! There is a new {brandWithEmoji} store in {localityWithFlag}! You can find it on the map 👆 \r\n" +
                    "It's the {restaurantsCountAtBrand} restaurant of {brand} and the {totalOverall} of the entire Dodo Brands 🔥",
            },
        };

        foreach (var template in templates)
        {
            Execute.EmbeddedScript(@"
                INSERT INTO notification_templates
                (
                    `NotificationType`,
                    `MessengerType`,
                    `LanguageCode`,
                    `Template`
                )
                VALUES
                (
                    @NotificationType,
                    @MessengerType,
                    @LanguageCode,
                    @Template
                );
            ");
        }
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}