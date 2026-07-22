# Correção definitiva do tema ao trocar de menu

## Causa identificada
A navegação aprimorada do Blazor substituía o conteúdo das páginas sem executar novamente o script inicial do tema. Por isso o tema escuro permanecia salvo, mas a nova rota podia ser exibida com estilos claros até uma atualização manual.

## Correção aplicada
- A navegação aprimorada foi desativada somente nos menus principal e inferior.
- Cada troca de módulo passa a carregar a rota normalmente e o tema é aplicado antes da renderização visual.
- A preferência Claro/Escuro continua salva no `localStorage`.
- O script de tema recebeu uma nova versão para evitar cache antigo.
- Nenhuma migration ou alteração no banco foi criada.

## Resultado esperado
Ao selecionar o tema escuro e trocar entre Dashboard, Clientes, Parcelas, Agenda e Relatórios, todas as telas devem abrir diretamente no tema escuro, sem exigir atualização manual.
