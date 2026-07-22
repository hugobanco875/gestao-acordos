# Correção de backup e anexos

## Backup e restauração
- O backup passa a incluir todos os registros de `AcordoAnexos`, inclusive o conteúdo binário dos arquivos.
- A restauração remapeia o identificador antigo do acordo para o novo antes de inserir cada anexo.
- Os comprovantes de parcelas também foram incluídos no backup completo e restaurados com remapeamento da parcela.
- Backups antigos continuam compatíveis: listas de anexos ou comprovantes ausentes são tratadas como vazias.

## Inclusão de arquivos no acordo
- Durante a leitura dos arquivos, o botão de salvar fica bloqueado para impedir salvamento antes do término do upload.
- Assim que a importação termina, os arquivos aparecem imediatamente na lista como prontos para salvar.
- No novo acordo, acordo, parcelas e anexos são gravados na mesma operação e transação.
- Na edição, alterações e novos anexos também são gravados em uma única transação.
- Em caso de erro, a operação inteira é desfeita, evitando acordo salvo sem anexos.
