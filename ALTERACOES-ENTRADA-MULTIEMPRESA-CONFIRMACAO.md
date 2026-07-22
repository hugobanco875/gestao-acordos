# Alterações aplicadas

## 1. Valor de entrada nos acordos
- Novo campo **Valor da entrada** e **Data da entrada**.
- O valor das parcelas agora é calculado sobre o saldo: `valor total - entrada`.
- A tela mostra resumo com total, entrada e saldo parcelado.
- A entrada é considerada no total recebido e no saldo em aberto do acordo.

## 2. Cliente vinculado a várias empresas
- O cadastro de cliente agora permite marcar uma ou mais empresas.
- A listagem e o detalhe mostram todas as empresas vinculadas.
- Ao criar um acordo, é necessário escolher a empresa específica daquele acordo.
- Filtros de parcelas e relatórios consideram os novos vínculos.

## 3. Confirmação de baixa e estorno
- Tanto **Baixar** quanto **Estornar** abrem uma tela de confirmação.
- A confirmação mostra cliente, parcela, valor e data da baixa.
- A operação somente é executada após clicar em **Sim**.

## Banco de dados
A migration `20260721230000_EntradaClienteMultiEmpresaConfirmacao` será aplicada automaticamente ao iniciar o sistema.
