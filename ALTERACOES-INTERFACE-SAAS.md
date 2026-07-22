# Redesign da interface

## Ajustes aplicados

- Tela de autenticação separada da área interna.
- Login redesenhado com painel institucional discreto e formulário centralizado.
- Remoção de molduras decorativas e bordas indevidas em títulos.
- Marca, nome do sistema e cores continuam sendo carregados das configurações.
- Layout mobile simplificado, exibindo apenas marca e formulário.
- Botões primários sem gradientes excessivos.
- Cards e tabelas com acabamento mais sóbrio.
- Título "Aparência" alterado para "Configurações".

## Teste recomendado

```powershell
dotnet restore
dotnet run
```

Depois, use `Ctrl + F5` no navegador para evitar cache de CSS antigo.
