# 🪟 Rodar como serviço no Windows (scripts prontos)

Estes scripts publicam a aplicação e a instalam como **serviço do Windows**
(inicia sozinho quando o Windows liga, sem precisar deixar janela aberta).

> Antes, tenha o **banco PostgreSQL** pronto — veja `../../database/LEIA-ME.md`.

---

## Passo a passo

### 1. Publicar (numa máquina com o **.NET 9 SDK**)
Dê dois cliques em **`publicar.bat`**.
- Ele cria a pasta **`publicado`** aqui do lado.
- Se o próprio servidor tiver o SDK, pode rodar lá mesmo.

### 2. Copiar para o servidor
Copie a pasta **`publicado`** para o servidor, em **`C:\GestaoAcordos`**.
(Se usar outro caminho, ajuste a variável `APP` no próximo passo.)

### 3. Instalar o serviço (no servidor, como Administrador)
1. Abra **`instalar-servico.bat`** num editor de texto e **edite o topo**:
   - `CONEXAO` → a string de conexão do seu banco (usuário, senha, host).
   - `PORTA` → a porta (padrão 8080).
   - `APP` → caminho do `GestaoAcordos.exe` (padrão `C:\GestaoAcordos\GestaoAcordos.exe`).
2. Clique com o **botão direito** no `instalar-servico.bat` → **Executar como administrador**.

Pronto! O sistema sobe automaticamente. Acesse **http://localhost:8080**
(ou `http://IP-DO-SERVIDOR:8080` de outro aparelho).

---

## Gerenciar o serviço
- **Ver/parar/iniciar**: abra o `services.msc` e procure **"Gestao de Acordos"**.
- **Remover**: rode **`desinstalar-servico.bat`** como Administrador (não apaga o banco).
- **Atualizar o sistema**: rode `publicar.bat` de novo, copie a pasta `publicado`
  por cima em `C:\GestaoAcordos`, e reinicie o serviço (no `services.msc` ou
  `sc stop GestaoAcordos` + `sc start GestaoAcordos`).

## Pré-requisitos no servidor
- **Runtime ASP.NET Core 9** (Hosting Bundle): https://dotnet.microsoft.com/download/dotnet/9.0
- **PostgreSQL** instalado e o banco criado (`../../database/LEIA-ME.md`).
