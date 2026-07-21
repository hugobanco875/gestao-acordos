# 🗄️ Banco de dados (PostgreSQL)

O sistema usa **PostgreSQL**. Aqui está tudo para preparar o banco no **seu servidor**.

> 💡 Você **não precisa** rodar o `schema.sql` na mão: o app **cria/atualiza as tabelas sozinho** ao iniciar (basta um banco vazio). O `schema.sql` está aqui como referência e para quem quiser criar as tabelas manualmente.

---

## 1. Instalar o PostgreSQL

### Windows
1. Baixe o instalador em: https://www.postgresql.org/download/windows/
2. Instale (Next, Next...). Quando pedir:
   - **Senha do usuário `postgres`**: anote (ex.: `postgres123`).
   - **Porta**: `5432` (padrão).
3. Ao final, você terá o **pgAdmin** (interface visual) e o **SQL Shell (psql)**.

### Linux (Ubuntu/Debian)
```bash
sudo apt update
sudo apt install -y postgresql
sudo systemctl enable --now postgresql
```

---

## 2. Criar o banco e o usuário do sistema

Abra o **SQL Shell (psql)** no Windows, ou no Linux:
```bash
sudo -u postgres psql
```

E rode (troque a senha por uma forte):
```sql
CREATE DATABASE gestaoacordos;
CREATE USER gestao WITH PASSWORD 'TroqueEstaSenha';
GRANT ALL PRIVILEGES ON DATABASE gestaoacordos TO gestao;
-- Postgres 15+: garante permissão no schema public
\c gestaoacordos
GRANT ALL ON SCHEMA public TO gestao;
```

---

## 3. (Opcional) Criar as tabelas manualmente

Só se você **não** for deixar o app criar sozinho:
```bash
psql -U gestao -d gestaoacordos -h localhost -f schema.sql
```

---

## 4. String de conexão

É isso que o aplicativo precisa para achar o banco (ver a pasta `../deploy`):
```
Host=localhost;Port=5432;Database=gestaoacordos;Username=gestao;Password=TroqueEstaSenha
```
- Rodando na **mesma máquina** do banco → `Host=localhost`
- Banco em **outro servidor** → `Host=IP_DO_SERVIDOR` (e libere a porta 5432 no firewall + `pg_hba.conf`)
- Usando **docker-compose** (recomendado) → `Host=db` (já vem configurado)

---

## 5. Fazer backup do banco (linha de comando)

```bash
pg_dump -U gestao -d gestaoacordos -h localhost -F c -f backup.dump
```
> O sistema também tem um botão **Backup** na tela que baixa tudo (dados + PDFs) — veja o menu "Backup".
