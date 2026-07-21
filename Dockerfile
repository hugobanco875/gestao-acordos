# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["GestaoAcordos.csproj", "./"]
RUN dotnet restore "GestaoAcordos.csproj"
COPY . .
RUN dotnet publish "GestaoAcordos.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---- Runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Render/Fly.io injetam a variável PORT; o Program.cs a utiliza. 8080 é o padrão local do container.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "GestaoAcordos.dll"]
