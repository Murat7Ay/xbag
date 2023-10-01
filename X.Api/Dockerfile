FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["X.Api/X.Api.csproj", "X.Api/"]
RUN dotnet restore "X.Api/X.Api.csproj"
COPY . .
WORKDIR "/src/X.Api"
RUN dotnet build "X.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "X.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "X.Api.dll"]
