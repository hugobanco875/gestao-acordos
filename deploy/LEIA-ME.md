# 🚀 Rodar no seu servidor com Docker (mais fácil)

Sobe **banco + aplicação juntos** com um comando. Serve para Windows, Linux ou qualquer servidor com Docker.

## Pré-requisito
Instalar o **Docker**:
- Windows/Mac: **Docker Desktop** → https://www.docker.com/products/docker-desktop/
- Linux: `curl -fsSL https://get.docker.com | sh`

## Passo a passo
1. Copie a pasta do projeto para o servidor.
2. Entre na pasta `deploy`:
   ```bash
   cd deploy
   ```
3. Crie o arquivo de senha:
   ```bash
   cp .env.exemplo .env        # no Windows (PowerShell): copy .env.exemplo .env
   ```
   Abra o `.env` e troque `DB_PASSWORD` por uma senha forte.
4. Suba tudo:
   ```bash
   docker compose up -d --build
   ```
   (a primeira vez baixa/compila — leva alguns minutos)
5. Acesse: **http://SEU_SERVIDOR:8080**
   - Na mesma máquina: http://localhost:8080
   - O app cria as tabelas sozinho na primeira subida.
6. Crie sua conta em **"Criar conta"** e use.

## Comandos úteis
```bash
docker compose logs -f app      # ver logs do app
docker compose ps               # ver o que está rodando
docker compose down             # parar (dados do banco continuam salvos)
docker compose up -d --build    # atualizar após mudar o código
```

## Backup do banco (Docker)
```bash
docker exec gestaoacordos-db pg_dump -U gestao -d gestaoacordos -F c -f /tmp/backup.dump
docker cp gestaoacordos-db:/tmp/backup.dump ./backup.dump
```
> Ou use o botão **Backup** dentro do sistema (baixa dados + PDFs).

## HTTPS (opcional)
Para acesso seguro por domínio, coloque um **Nginx** ou **Caddy** na frente (proxy reverso) apontando para a porta 8080. O Caddy já emite certificado grátis automaticamente.
