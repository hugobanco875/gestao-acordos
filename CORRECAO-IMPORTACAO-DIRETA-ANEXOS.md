# Correção da importação de anexos

## Novo fluxo

1. Selecione um ou vários arquivos.
2. Clique em **Importar arquivos**.
3. Cada arquivo é enviado diretamente ao servidor por HTTP, com progresso individual.
4. Os arquivos concluídos aparecem em **Arquivos anexados**.
5. Clique em **Salvar acordo** para persistir o acordo e todos os anexos no PostgreSQL/Supabase.

## Alterações técnicas

- O conteúdo dos arquivos não trafega mais pelo circuito SignalR do Blazor Server.
- Foi criado o endpoint autenticado `/api/acordos/upload-temporario`.
- Até três arquivos são transferidos simultaneamente.
- Os arquivos ficam em armazenamento temporário por até duas horas e são removidos após o salvamento ou ao sair da tela.
- O botão **Salvar acordo** fica bloqueado enquanto existirem arquivos selecionados ainda não importados.
- Limite mantido em 10 MB por arquivo.
