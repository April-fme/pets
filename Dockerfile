FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["PetsAPI/PetsAPI.csproj", "PetsAPI/"]
RUN dotnet restore "PetsAPI/PetsAPI.csproj"
COPY PetsAPI/ PetsAPI/
WORKDIR /src/PetsAPI
RUN dotnet build "PetsAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PetsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PetsAPI.dll"]
