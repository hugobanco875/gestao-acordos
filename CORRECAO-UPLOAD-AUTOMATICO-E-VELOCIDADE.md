# Upload automático, bloqueio do Salvar e ganho de velocidade

- O upload agora começa **automaticamente** assim que os arquivos são selecionados no explorador de arquivos — não é mais necessário clicar em um botão "Importar arquivos".
- Cada arquivo aparece imediatamente na lista com nome e barra de progresso individual (0% a 100%).
- O botão **Salvar acordo** fica desabilitado assim que pelo menos um arquivo é selecionado e continua bloqueado até que todos os uploads em andamento terminem (com sucesso ou falha). Sem nenhum arquivo pendente, o botão permanece liberado normalmente.
- Arquivos maiores que 10 MB ou em formato não permitido são rejeitados imediatamente no navegador, sem gastar tempo enviando-os pela rede até o servidor recusar.
- O número de uploads simultâneos aumentou de 3 para 4, reduzindo o tempo total para lotes com vários arquivos.
- As atualizações de progresso enviadas ao servidor foram limitadas a saltos de 5%, evitando comunicação excessiva entre navegador e servidor durante o envio.
