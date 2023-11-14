using System.Collections.Generic;
using Dodo1000Bot.Models;
using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202311142143)]
public class _202311142143_AddNewUnitsTemplates: Migration
{
    public override void Up()
    {
        var templates = new Dictionary<string, string>[]
        {
            new()
            {
                { "NotificationType", NotificationType.NewUnit.ToString("D") },
                { "MessengerType", Source.Telegram.ToString("D") },
                { "LanguageCode", "'ru'" },
                { "Template", "\"Ух ты! Открылся новый ресторан {brandWithEmoji} в городе {localityWithFlag}! Ты сможешь найти его на карте 👆 \r\n" +
                              "Это {restaurantsCountAtBrand}-й ресторан {brand} и {totalOverall}-й во всей сети Dodo brands 🔥\""}
            },
            new()
            {
                { "NotificationType", NotificationType.NewUnit.ToString("D") },
                { "MessengerType", Source.Telegram.ToString("D") },
                { "LanguageCode", "'ru'" },
                { "Template", "\"Ух ты! Открылся новый ресторан {brandWithEmoji} в городе {localityWithFlag}! Можешь найти его на карте 👆 \r\n" +
                              "Это {restaurantsCountAtBrand}-й ресторан {brand} и {totalOverall}-й во всей сети Dodo brands 🔥\""}
            },
            new()
            {
                { "NotificationType", NotificationType.NewUnit.ToString("D") },
                { "MessengerType", Source.Telegram.ToString("D") },
                { "LanguageCode", "'ru'" },
                { "Template", "\"Вау! Открылся новый ресторан {brandWithEmoji} в городе {localityWithFlag}! Ты сможешь найти его на карте 👆 \r\n" +
                              "Это {restaurantsCountAtBrand}-й ресторан {brand} и {totalOverall}-й из всех в Dodo brands 🔥\""}
            },
            new()
            {
                { "NotificationType", NotificationType.NewUnit.ToString("D") },
                { "MessengerType", Source.Telegram.ToString("D") },
                { "LanguageCode", "'ru'" },
                { "Template", "\"Вау! Открылся новый ресторан {brandWithEmoji} в городе {localityWithFlag}! Можешь найти его на карте 👆 \r\n" +
                              "Это {restaurantsCountAtBrand}-й ресторан {brand} и {totalOverall}-й во всей сети Dodo brands 🔥\""}
            },
            new()
            {
                { "NotificationType", NotificationType.NewUnit.ToString("D") },
                { "MessengerType", Source.Telegram.ToString("D") },
                { "LanguageCode", "'ru'" },
                { "Template", "\"Вау! Появился новый ресторан {brandWithEmoji} в городе {localityWithFlag}! Ты сможешь найти его на карте 👆 \r\n" +
                              "Это {restaurantsCountAtBrand}-й ресторан {brand} и {totalOverall}-й во всей сети Dodo brands 🔥\""}
            },
            new()
            {
                { "NotificationType", NotificationType.NewUnit.ToString("D") },
                { "MessengerType", Source.Telegram.ToString("D") },
                { "LanguageCode", "'ru'" },
                { "Template", "\"Вау! Появился новый ресторан {brandWithEmoji} в городе {localityWithFlag}! Можешь найти его на карте 👆 \r\n" +
                              "Это {restaurantsCountAtBrand}-й ресторан {brand} и {totalOverall}-й во всей сети Dodo brands 🔥\""}
            },
            new()
            {
                { "NotificationType", NotificationType.NewUnit.ToString("D") },
                { "MessengerType", Source.Telegram.ToString("D") },
                { "LanguageCode", "'ru'" },
                { "Template", "\"Смотри! Новый ресторан {brandWithEmoji} в городе {localityWithFlag}! Ты сможешь найти его на карте 👆 \r\n" +
                              "Это {restaurantsCountAtBrand}-й ресторан {brand} и {totalOverall}-й во всей сети Dodo brands 🔥\""}
            },
            new()
            {
                { "NotificationType", NotificationType.NewUnit.ToString("D") },
                { "MessengerType", Source.Telegram.ToString("D") },
                { "LanguageCode", "'ru'" },
                { "Template", "\"Смотри! Новый ресторан {brandWithEmoji} в городе {localityWithFlag}! Можешь найти его на карте 👆 \r\n" +
                              "Это {restaurantsCountAtBrand}-й ресторан {brand} и {totalOverall}-й во всей сети Dodo brands 🔥\""}
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