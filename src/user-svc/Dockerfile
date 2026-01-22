
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/common ./src/common
COPY src/user-svc ./src/user-svc
RUN dotnet restore ./src/user-svc/UserService.csproj
RUN dotnet publish ./src/user-svc/UserService.csproj -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "UserService.dll"]
