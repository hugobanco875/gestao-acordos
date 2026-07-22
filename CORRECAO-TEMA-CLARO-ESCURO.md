# Correção definitiva dos temas claro e escuro

## Alterações

- Removida a opção de tema automático.
- Mantidas apenas as opções **Claro** e **Escuro**.
- A preferência escolhida é salva no `localStorage` do dispositivo.
- A navegação entre páginas não sobrescreve mais o tema salvo com um valor vindo do servidor.
- O tema é reaplicado após navegação aprimorada, voltar/avançar, retorno da PWA ao primeiro plano e sincronização entre abas.
- O tema é aplicado também no elemento `body`, além do `html`, para maior compatibilidade com navegadores móveis.
- Registros antigos com o valor `automatico` passam a ser tratados como tema claro até o usuário escolher e salvar uma das duas opções.

## Banco de dados

Não há migration nova. A coluna existente continua sendo usada.
