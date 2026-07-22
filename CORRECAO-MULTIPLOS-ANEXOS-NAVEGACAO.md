# Correção de múltiplos anexos e navegação

- A lista do `InputFile` é capturada antes de qualquer renderização do componente.
- Cada stream é validado pelo tamanho real lido e falhas individuais são exibidas.
- O sistema confirma no banco a quantidade de anexos antes de concluir a transação.
- Foram removidos os atributos `data-enhance-nav="false"` das rotas internas, que obrigavam recarga completa ao trocar de menu.
