# Correção do travamento após a atualização do tema

A versão anterior utilizava um MutationObserver nos atributos de tema do elemento HTML. Como a própria reaplicação do tema modificava esses atributos, alguns navegadores podiam entrar em um ciclo contínuo de atualização, deixando o Blazor apenas carregando.

Nesta versão:
- o MutationObserver foi removido;
- o tema continua sendo reaplicado após `enhancedload`, retorno à aba e mudança do tema do dispositivo;
- atributos só são atualizados quando o valor realmente mudou;
- o JavaScript recebeu versionamento na URL para evitar o carregamento do arquivo antigo pelo cache.
