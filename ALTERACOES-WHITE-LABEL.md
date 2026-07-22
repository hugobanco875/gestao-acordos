# Evolução white-label

## Implementado

- Logomarca ampliada no menu lateral, login, mobile e pré-visualização.
- Imagens passam a preencher integralmente o espaço com `object-fit: cover`.
- Nome da empresa pode ocupar até duas linhas, sem cortar prematuramente.
- Novo campo de descrição da empresa, com padrão `Advocacia especializada`.
- Temas Claro, Escuro e Automático (segue a preferência do aparelho).
- Tema aplicado tanto na área autenticada quanto nas telas de login.
- Nova migration `20260721213000_SubtituloETema`.

## Aplicação

Ao executar `dotnet run`, a migration será aplicada automaticamente pelo projeto.
Depois, acesse **Configurações > Identidade visual** e salve as preferências.
