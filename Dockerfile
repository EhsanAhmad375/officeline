FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Project file copy karen
COPY ["officeline.csproj", "./"]
RUN dotnet restore "officeline.csproj"

# Baqi code copy karke build karen
COPY . .
RUN dotnet publish "officeline.csproj" -c Release -o /app

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "officeline.dll"]