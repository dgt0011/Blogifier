FROM mcr.microsoft.com/dotnet/aspnet:6.0 as base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Blogifier/Blogifier.csproj", "src/Blogifier/"]
COPY ["src/Blogifier.Admin/Blogifier.Admin.csproj", "src/Blogifier.Admin/"]
COPY ["src/Blogifier.Shared/Blogifier.Shared.csproj", "src/Blogifier.Shared/"]
COPY ["src/Blogifier.Core/Blogifier.Core.csproj", "src/Blogifier.Core/"]
RUN dotnet restore "src/Blogifier/Blogifier.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "src/Blogifier/Blogifier.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "src/Blogifier/Blogifier.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blogifier.dll"]
