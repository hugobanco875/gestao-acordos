# Gestão de Acordos — base V2 estável

## Correção principal

A migration `20260722003000_MovimentacoesFinanceiras` não possuía os atributos
usados pelo Entity Framework para identificá-la. Por isso o código novo era
executado, mas as colunas financeiras não eram criadas no PostgreSQL.

A versão atual:

- registra corretamente a migration financeira;
- cria as colunas de tipo, forma de pagamento e metadados do comprovante;
- cria a tabela separada para o conteúdo dos comprovantes;
- cria o índice financeiro;
- converte entradas antigas em movimentos recebidos;
- adiciona uma migration idempotente de validação para bancos parcialmente atualizados;
- preserva clientes, acordos, parcelas e recebimentos existentes.

## Atualização

1. Pare a aplicação com `Ctrl + C`.
2. Substitua os arquivos pelos desta versão.
3. Execute `dotnet restore`.
4. Execute `dotnet run`.
5. Aguarde a aplicação das migrations antes de abrir o navegador.

Não exclua o banco de dados e não recrie o contêiner PostgreSQL.
