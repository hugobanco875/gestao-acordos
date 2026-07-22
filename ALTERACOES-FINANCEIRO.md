# Evolução do financeiro do acordo

- Entrada passou a ser uma movimentação financeira real, identificada no extrato como **Entrada**.
- Entradas antigas são convertidas automaticamente pela migration `20260722003000_MovimentacoesFinanceiras`.
- Nova entrada inicial aceita forma de pagamento e observação.
- Tela do acordo permite registrar entradas adicionais com data, forma de pagamento, observação e comprovante.
- Comprovantes aceitos: PDF, JPG, PNG e WebP, até 10 MB.
- Entradas e parcelas podem ser baixadas ou estornadas com confirmação.
- Dashboard contabiliza entradas nos recebimentos, mas mantém os cards de contas a receber apenas com parcelas.
- Relatórios em tela, Excel e PDF agora exibem entradas e parcelas, tipo de movimento e forma de pagamento.
- O extrato do acordo apresenta todas as movimentações em ordem cronológica.
