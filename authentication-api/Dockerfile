FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore "authentication-api.csproj"

WORKDIR "/src"
RUN dotnet build "authentication-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "authentication-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
COPY private.key /app/
COPY public.key /app/

ENV TZ='Asia/Bangkok'
RUN apk add --no-cache tzdata

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs


ENTRYPOINT ["dotnet", "authentication-api.dll"]