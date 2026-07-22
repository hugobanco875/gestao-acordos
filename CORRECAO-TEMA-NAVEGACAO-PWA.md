# Correção do tema automático durante a navegação

O gerenciador de tema foi ajustado para reaplicar a preferência salva após cada navegação aprimorada do Blazor.

Também foram adicionados tratamentos para:

- alteração do tema do dispositivo com o aplicativo aberto;
- navegação pelos botões voltar e avançar;
- retorno da PWA do segundo plano;
- retomada da janela;
- sincronização entre abas;
- restauração automática dos atributos de tema caso sejam removidos durante uma atualização da página.

Nenhuma migration ou alteração no banco de dados é necessária.
