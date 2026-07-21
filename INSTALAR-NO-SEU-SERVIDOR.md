# 🏠 Instalar o sistema no seu próprio servidor

Este guia mostra como rodar o **Gestão de Acordos** no **seu servidor** (Windows ou Linux),
com o **banco de dados** e a **aplicação** separados e organizados.

---

## 📁 Estrutura do projeto

```
GestaoAcordos/
├─ database/        🗄️  BANCO: schema.sql + guia de instalação do PostgreSQL
├─ deploy/          🚀  DEPLOY: docker-compose (banco + app juntos) + guia
├─ Components/  Models/  Services/  Data/  ...   →  código da aplicação (API/web)
├─ Dockerfile       📦  como a aplicação é empacotada
├─ INSTALAR-NO-SEU-SERVIDOR.md   ← este guia
└─ README.md
```

- **`database/`** = tudo do banco (instalar PostgreSQL, criar o banco, script `schema.sql`).
- **`deploy/`** = tudo para publicar (Docker Compose que sobe banco + app com 1 comando).
- **Raiz** = o código da aplicação em si.

> A configuração da nuvem (Render + Supabase) continua funcionando como está.
> Este guia é para quando você quiser rodar **na sua máquina/servidor**.

---

## ✅ Caminho A — Docker (recomendado, mais fácil)

Sobe **banco + aplicação juntos**, sem instalar nada além do Docker.

1. Instale o **Docker** (Docker Desktop no Windows, ou `curl -fsSL https://get.docker.com | sh` no Linux).
2. Copie a pasta do projeto para o servidor.
3. Siga o passo a passo em **[`deploy/LEIA-ME.md`](deploy/LEIA-ME.md)**:
   ```bash
   cd deploy
   copy .env.exemplo .env      # (Linux: cp .env.exemplo .env) e defina a senha
   docker compose up -d --build
   ```
4. Acesse **http://SEU_SERVIDOR:8080** → crie sua conta → pronto. 🎉

O banco fica salvo em um volume do Docker (não some ao reiniciar). O app cria as tabelas sozinho.

---

## 🔧 Caminho B — Manual (PostgreSQL + .NET, sem Docker)

Use este se preferir instalar direto no Windows Server / Linux, sem Docker.

### 1. Instalar o banco
Siga **[`database/LEIA-ME.md`](database/LEIA-ME.md)**: instalar PostgreSQL, criar o banco `gestaoacordos` e o usuário `gestao`.

### 2. Instalar o runtime do .NET 9 no servidor
- **Windows**: baixe o **ASP.NET Core Hosting Bundle 9** em https://dotnet.microsoft.com/download/dotnet/9.0
- **Linux (Ubuntu)**:
  ```bash
  sudo apt install -y aspnetcore-runtime-9.0
  ```

### 3. Publicar a aplicação
Numa máquina com o **SDK do .NET 9** (ex.: sua máquina de desenvolvimento), na raiz do projeto:
```bash
dotnet publish -c Release -o publicado
```
Copie a pasta **`publicado`** inteira para o servidor.

### 4. Configurar a conexão com o banco
No servidor, defina a variável de ambiente com a sua string de conexão:

- **Windows (PowerShell, como Administrador):**
  ```powershell
  setx ConnectionStrings__DefaultConnection "Host=localhost;Port=5432;Database=gestaoacordos;Username=gestao;Password=SuaSenha" /M
  setx ASPNETCORE_URLS "http://+:8080" /M
  ```
- **Linux:** adicione ao serviço (passo 5) as variáveis `ConnectionStrings__DefaultConnection` e `ASPNETCORE_URLS`.

### 5. Rodar
Dentro da pasta `publicado`:
```bash
dotnet GestaoAcordos.dll
```
Acesse **http://SEU_SERVIDOR:8080**. O app cria as tabelas sozinho na primeira vez.

### 6. Deixar rodando sempre (serviço)
Para não precisar deixar uma janela aberta:

- **Windows** — como serviço, com o **NSSM** (https://nssm.cc):
  ```
  nssm install GestaoAcordos "C:\Program Files\dotnet\dotnet.exe" "C:\caminho\publicado\GestaoAcordos.dll"
  nssm start GestaoAcordos
  ```
- **Linux** — crie `/etc/systemd/system/gestaoacordos.service`:
  ```ini
  [Unit]
  Description=Gestao de Acordos
  After=network.target postgresql.service

  [Service]
  WorkingDirectory=/opt/gestaoacordos/publicado
  ExecStart=/usr/bin/dotnet /opt/gestaoacordos/publicado/GestaoAcordos.dll
  Environment=ASPNETCORE_URLS=http://+:8080
  Environment=ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=gestaoacordos;Username=gestao;Password=SuaSenha
  Restart=always

  [Install]
  WantedBy=multi-user.target
  ```
  ```bash
  sudo systemctl enable --now gestaoacordos
  ```

---

## 🔒 Acesso por domínio + HTTPS (opcional)
Coloque um **Caddy** ou **Nginx** na frente (proxy reverso) apontando para a porta 8080.
O Caddy emite certificado HTTPS grátis automaticamente. Exemplo de `Caddyfile`:
```
seu-dominio.com.br {
    reverse_proxy localhost:8080
}
```

---

## 💾 Trocar da nuvem (Supabase) para o seu banco
1. No sistema atual (nuvem), use o botão **Backup** → baixa um arquivo `.json` com tudo.
2. Suba o sistema no seu servidor (Caminho A ou B).
3. No sistema novo, use **Backup → Restaurar** e envie o arquivo `.json`.
Pronto: seus dados (e PDFs) migram para o seu servidor. ✅
