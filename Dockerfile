FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY LivvaPlatform.API/*.csproj LivvaPlatform.API/
RUN dotnet restore ./LivvaPlatform.API
COPY . .
RUN dotnet publish ./LivvaPlatform.API -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "LivvaPlatform.API.dll"]