# Correção do tema automático na PWA

- O tema é aplicado antes do carregamento do CSS, evitando a tela clara inicial.
- O modo Automático acompanha alterações do tema do dispositivo em tempo real.
- A preferência é mantida no navegador/PWA por `localStorage`.
- O tema é reaplicado ao entrar no layout principal ou na tela de login.
- Bootstrap, formulários, menu lateral e barras do navegador recebem o mesmo tema.
- A cor da barra superior da PWA também muda entre claro e escuro.
