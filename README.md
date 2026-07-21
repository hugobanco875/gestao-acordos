# Gestão de Acordos

Sistema web (celular + computador) para gestão de **acordos jurídicos**, **parcelas** e **eventos**
(reuniões/audiências), com **relatórios** e **dashboard**.

- **Stack:** ASP.NET Core 9 + **Blazor Web App** (Interactive Server)
- **Banco:** PostgreSQL (**Supabase** — plano gratuito)
- **Login/cadastro:** ASP.NET Core Identity
- **Exportação:** Excel (ClosedXML) e PDF (QuestPDF)
- **PWA:** instalável no celular

## Funcionalidades
- Login e cadastro de usuários
- Cadastro de **empresas**
- Cadastro de **clientes** (vinculados a uma empresa)
- **Acordos** por cliente: número do processo, **PDF do acordo**, valor total e **parcelas** (data da 1ª parcela)
- **Baixa de parcelas** (com data de pagamento e estorno)
- **Eventos** (reuniões/audiências) com data/hora e vínculo a cliente/empresa
- **Relatórios** de parcelas baixadas por período, empresa e cliente — exportar **Excel/PDF**
- **Dashboard**: parcelas a receber e eventos por **dia, semana e mês**

---

## 1) Criar o banco no Supabase (grátis)

1. Acesse <https://supabase.com> e crie uma conta (login com GitHub é o mais rápido).
2. **New project** → dê um nome (ex.: `gestao-acordos`), defina uma **senha do banco** (guarde-a) e escolha a região mais próxima (ex.: *South America (São Paulo)*).
3. Aguarde ~2 min o projeto provisionar.
4. Menu **Connect** (botão no topo) → aba **Connection string** → seção **Session pooler** (compatível com IPv4).
   Você verá algo como:
   ```
   postgresql://postgres.abcdefgh:[YOUR-PASSWORD]@aws-0-sa-east-1.pooler.supabase.com:5432/postgres
   ```

## 2) Configurar a connection string (formato Npgsql)

O .NET usa formato *chave=valor*. Converta os dados acima para:

```
Host=aws-0-sa-east-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.abcdefgh;Password=SUA_SENHA;SSL Mode=Require;Trust Server Certificate=true
```

Guarde-a com **user-secrets** (não vai para o código):

```bash
cd F:\Projeto\GestaoAcordos
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Port=5432;Database=postgres;Username=postgres.abcdefgh;Password=SUA_SENHA;SSL Mode=Require;Trust Server Certificate=true"
```

## 3) Rodar localmente

```bash
dotnet run --project F:\Projeto\GestaoAcordos
```

- Na primeira execução as **tabelas são criadas automaticamente** (migrations).
- Abra o endereço mostrado no console (ex.: `https://localhost:xxxx`).
- Clique em **Criar conta**, cadastre seu usuário e faça login.

---

## 4) Publicar em nuvem grátis (Render + Docker)

O projeto já inclui um `Dockerfile`.

1. Suba o projeto para um repositório no GitHub.
2. Em <https://render.com> → **New → Web Service** → conecte o repositório.
3. **Runtime:** Docker (detectado automaticamente pelo `Dockerfile`).
4. Em **Environment**, adicione a variável:
   - `ConnectionStrings__DefaultConnection` = *(a mesma connection string do passo 2)*
5. **Create Web Service**. O deploy roda as migrations sozinho na primeira subida.

> Alternativas de hospedagem grátis: **Fly.io** (também via Docker) ou **Azure App Service** (F1).
> Obs.: no plano gratuito o serviço "dorme" após inatividade e leva alguns segundos para acordar.

---

## Observações técnicas
- Os **PDFs dos acordos** ficam salvos no próprio banco (tabela `AcordosPdf`), até 10 MB por arquivo.
  Para grande volume, dá para migrar depois para o **Supabase Storage**.
- Datas/hora funcionam no **horário local** (modo *legacy timestamp* do Npgsql).
- Cultura **pt-BR** (R$ e datas `dd/MM/yyyy`).
