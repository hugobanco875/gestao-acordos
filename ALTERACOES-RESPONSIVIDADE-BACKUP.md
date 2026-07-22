# Ajustes de responsividade e segurança do backup

## Criação de usuário
- Novo layout responsivo para desktop e celular.
- E-mail ocupa a largura total no desktop.
- Senha e confirmação ficam lado a lado no desktop e empilhadas no celular.
- Ações ficam alinhadas no desktop e em largura total no celular.
- Permissão administrativa apresentada em bloco destacado e acessível.

## Backup e restauração
- A página continua disponível somente para administrador ativo.
- Exportação e restauração agora também validam a permissão dentro do `BackupService`.
- O serviço exige o usuário autenticado e rejeita chamadas de usuários comuns, inativos ou não autenticados.
