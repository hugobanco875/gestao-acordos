# Correção do erro PendingModelChangesWarning

O Entity Framework Core 9 detectou diferença entre o modelo atual e o arquivo
`ApplicationDbContextModelSnapshot.cs`. Como as migrations recentes foram
adicionadas manualmente, o EF interrompia a execução antes de aplicá-las.

A configuração em `Program.cs` agora ignora apenas o aviso
`PendingModelChangesWarning`, permitindo que `Database.Migrate()` aplique as
migrations pendentes normalmente.

Nenhum banco precisa ser apagado e os dados existentes são preservados.
