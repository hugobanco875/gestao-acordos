# Forma de pagamento na baixa de parcelas

- Adicionada seleção da forma de pagamento no modal de confirmação da baixa.
- A forma escolhida é gravada na própria parcela e utilizada nos relatórios e no extrato financeiro.
- A listagem de baixa agora mostra a forma registrada nas parcelas já pagas.
- Ao estornar uma baixa, a forma de pagamento volta para "Não informada" junto com a data e o valor recebido.
- Não exige migration, pois o campo FormaPagamento já existia no modelo e no banco.
