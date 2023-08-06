﻿using FluentMigrator;

namespace Dodo1000Bot.Api.Migrations;

[Migration(202308061803)]
public class CreateCountriesTable: Migration
{
    public override void Up()
    {
        Create.Table("countries")
            .WithColumn("Id").AsInt32().NotNullable().Indexed().Unique()
            .WithColumn("Name").AsString(64).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("countries");
    }
}