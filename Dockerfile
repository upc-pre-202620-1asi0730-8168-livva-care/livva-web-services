FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY LivvaPlatform.API/*.csproj LivvaPlatform.API/
RUN dotnet restore ./LivvaPlatform.API
COPY . .
RUN dotnet publish ./LivvaPlatform.API -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
# Railway provides the listening port in the PORT environment variable (8080 by default).
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} exec dotnet LivvaPlatform.API.dll"]