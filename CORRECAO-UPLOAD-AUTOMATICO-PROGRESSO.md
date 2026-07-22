# Correção do upload automático de anexos

- A importação passa a iniciar automaticamente após a seleção dos arquivos.
- Cada arquivo possui barra de progresso individual de 0% a 100%.
- Até três arquivos são processados simultaneamente para reduzir o tempo total.
- As atualizações visuais são limitadas a intervalos de 5%, evitando renderizações excessivas.
- O botão Salvar acordo permanece bloqueado até o término de todos os uploads.
- Somente arquivos concluídos e validados são adicionados à lista do acordo.
- Erros de formato, tamanho ou leitura são exibidos no item correspondente.
