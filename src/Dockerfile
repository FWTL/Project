FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

RUN mkdir src
RUN mkdir temp
RUN apt-get update; exit 0;
RUN apt-get install rsync -y
COPY . /temp
RUN rsync -rh /temp/FWTL.* /src/ --exclude="appsettings.**.json"

WORKDIR /src/FWTL.Auth
RUN dotnet restore
RUN dotnet build --no-resotre

# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
# ARG CONFIGURATION=Release
# WORKDIR /src

# COPY ["WebApplication3/WebApplication3.csproj", "WebApplication3/"]
# RUN dotnet restore "WebApplication3/WebApplication3.csproj"
# COPY . .
# WORKDIR "/src/WebApplication3"
# RUN dotnet build "WebApplication3.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "WebApplication3.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "WebApplication3.dll"]