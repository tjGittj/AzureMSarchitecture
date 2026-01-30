
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props ./Directory.Build.props
COPY src/common ./src/common
COPY src/notify-svc ./src/notify-svc
RUN dotnet restore ./src/notify-svc/NotifyService.csproj
RUN dotnet publish ./src/notify-svc/NotifyService.csproj -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "NotifyService.dll"]
