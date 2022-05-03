FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EntityFrameworkCoreMultiTenancy.csproj", "./"]
RUN dotnet restore "EntityFrameworkCoreMultiTenancy.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "EntityFrameworkCoreMultiTenancy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EntityFrameworkCoreMultiTenancy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EntityFrameworkCoreMultiTenancy.dll"]
