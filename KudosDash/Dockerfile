FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["KudosDash/KudosDash.csproj", "KudosDash/"]
RUN dotnet restore "./KudosDash/./KudosDash.csproj"
COPY . .
RUN chmod 777 ./KudosDash/database.db
WORKDIR "/src/KudosDash"
RUN dotnet build "./KudosDash.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KudosDash.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /src/KudosDash/database.db .
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KudosDash.dll"]
