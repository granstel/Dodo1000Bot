using System.Collections.Generic;
using Dodo1000Bot.Models;
using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202311142035)]
public class AddNewUnitsTemplates: Migration
{
    public override void Up()
    {
        var templates = new Dictionary<string, string>[]
        {
            new()
            {
                { "NotificationType", NotificationType.NewUnit.ToString("D") },
                { "MessengerType", Source.Telegram.ToString("D") },
                { "LanguageCode", "'en'" },
                { "Template", "\"Wow! There is a new {brandWithEmoji} store in {localityWithFlag}! You can find it on the map 👆 \r\n" +
                              "It's the {restaurantsCountAtBrand} restaurant of {brand} and the {totalOverall} of the entire Dodo Brands 🔥\""}
            },
        };

        foreach (var template in templates)
        {
            Execute.Script("Migrations/Scripts/AddTemplate.sql", template);
        }
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }
}