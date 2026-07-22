# Revisão funcional V2

## Correção principal do relatório financeiro

O filtro **Todas** agora combina corretamente duas regras:

- movimentações pagas: entram no período pela data de pagamento;
- movimentações abertas: entram no período pela data de vencimento.

Antes, o filtro Todas usava somente a data de vencimento. Isso fazia parcelas baixadas desaparecerem quando o pagamento estava no período selecionado, mas o vencimento estava fora dele.

## Melhorias adicionais no relatório

- filtro por tipo: entradas e parcelas, somente entradas ou somente parcelas;
- descrição clara de qual data está sendo considerada;
- filtros de situação, tipo, empresa e cliente também constam no PDF e Excel;
- totais recebido e em aberto continuam calculados conforme a situação real de cada movimentação.

## Módulos revisados estaticamente

- Dashboard: entradas e parcelas pagas são somadas como recebimentos; somente parcelas abertas compõem o contas a receber.
- Acordos: entradas são movimentações separadas e integram o recebido e o saldo.
- Baixa de parcelas: filtros distinguem pagas e abertas e as ações usam confirmação.
- Clientes/empresas: vínculos múltiplos permanecem preservados.
- Relatórios: regra de período corrigida e filtro de tipo incluído.

Esta revisão não cria migration nem altera a estrutura do PostgreSQL.
