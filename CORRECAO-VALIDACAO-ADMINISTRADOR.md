# Correção da validação de administrador

## Problema corrigido
A validação administrativa utilizava `UserManager.GetUserAsync(ClaimsPrincipal)` dentro de componentes Blazor Server. Dependendo do estado do circuito e dos claims disponíveis, um usuário autenticado e marcado como administrador podia ser interpretado como não autorizado.

## Alteração
- A identificação do usuário passou a ser feita diretamente no PostgreSQL pelo claim `NameIdentifier`.
- Foi incluído fallback por nome de usuário/e-mail para compatibilidade com sessões antigas.
- As propriedades `Ativo` e `Administrador` continuam sendo conferidas diretamente no banco em cada operação protegida.
- A geração de backup agora mostra mensagens de autorização e erros técnicos de forma controlada.

## Segurança preservada
Somente usuários que estejam simultaneamente autenticados, ativos e marcados como administradores podem gerar ou restaurar backups.
