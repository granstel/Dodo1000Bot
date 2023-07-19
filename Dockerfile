FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS build
COPY /output .
ENTRYPOINT ["dotnet", "Dodo1000Bot.Api.dll"]
