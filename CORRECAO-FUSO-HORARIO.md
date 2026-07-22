# Correção de fuso horário

O sistema passou a utilizar uma fonte única de data e hora no fuso `America/Maceio` (UTC-3), adequado a Sergipe.

## Alterações

- Criado `Services/RelogioSistema.cs`.
- Cadastro de usuários e último acesso usam o relógio centralizado.
- Criação de empresas, clientes, acordos, anexos e eventos usa o mesmo relógio.
- Ações administrativas e geração de backup usam o mesmo horário.
- O contêiner Docker e o deploy no Render recebem `TZ=America/Maceio`.
- O horário deixa de depender da região do servidor de hospedagem.

## Observação sobre dados anteriores

A correção garante o horário correto para novos registros. Datas antigas permanecem com o valor que já estava salvo, pois alterar automaticamente registros históricos poderia modificar horários que foram gravados corretamente por outros ambientes.
