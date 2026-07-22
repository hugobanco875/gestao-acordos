# Alterações de segurança e administração de usuários

## Implementado

1. Serviço reutilizável `AdministradorService` para validar se o usuário autenticado é administrador ativo.
2. Cadastro público fechado após a criação do primeiro usuário.
3. Bootstrap preservado: quando o banco ainda não possui usuários, a rota de cadastro cria o primeiro administrador e inicia sua sessão.
4. Criação de novos usuários restrita a administradores ativos, sem substituir a sessão do administrador criador.
5. Backup, Configurações e Administração de Usuários protegidos pela validação centralizada.
6. Links públicos de criação de conta removidos do menu e do login após o bootstrap.
7. Botão **Novo usuário** adicionado à tela de Administração.
8. Bloqueio por força bruta ativado: 5 tentativas inválidas bloqueiam a conta por 15 minutos.
9. Operações administrativas críticas revalidam a autorização antes de executar.
