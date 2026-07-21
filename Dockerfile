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
# Dependências nativas para o QuestPDF (SkiaSharp) gerar PDF no Linux.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 fonts-dejavu-core \
    && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
# Render/Fly.io injetam a variável PORT; o Program.cs a utiliza. 8080 é o padrão local do container.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "GestaoAcordos.dll"]
