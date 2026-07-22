# Reformulação Premium

Esta versão preserva as regras de negócio e moderniza a experiência visual do sistema.

## Principais alterações

- Design system global com tipografia Inter, superfícies, bordas, sombras, espaçamentos e estados consistentes.
- Sidebar redesenhada para desktop, com melhor hierarquia e navegação.
- Navegação inferior para celular, adequada ao uso como PWA.
- Cabeçalho com status do sistema, avatar e acesso rápido à conta.
- Dashboard com cards executivos, indicadores visuais e skeleton de carregamento.
- Login reformulado com apresentação institucional e formulário premium.
- Tabelas, formulários, botões, modais, alertas, badges e listas modernizados globalmente.
- Melhorias de acessibilidade, foco, áreas de toque e redução de movimento.
- Metadados PWA e cores de instalação atualizados.

## Arquivos principais alterados

- `wwwroot/app.css`
- `wwwroot/manifest.webmanifest`
- `wwwroot/service-worker.js`
- `Components/App.razor`
- `Components/Layout/MainLayout.razor`
- `Components/Layout/MainLayout.razor.css`
- `Components/Layout/NavMenu.razor`
- `Components/Layout/NavMenu.razor.css`
- `Components/Pages/Home.razor`
- `Components/Account/Pages/Login.razor`

## Observação

O projeto deve ser restaurado e compilado em um ambiente com .NET 9 SDK antes da publicação.


## Correções adicionais — modais e máscaras
- Modais agora ocupam corretamente toda a viewport, sem ficarem limitados à área do conteúdo.
- Rolagem interna e altura máxima ajustadas para telas menores.
- Máscara dinâmica para CPF/CNPJ.
- Máscara de CEP no padrão `00000-000`.
- Máscara de processo CNJ no padrão `0000000-00.0000.0.00.0000`.

## Identidade visual personalizável
- Campo para o cliente definir o nome da empresa/sistema.
- Upload de logomarca em PNG, JPG ou WebP (até 2 MB).
- Logomarca armazenada no PostgreSQL e exibida automaticamente no menu lateral.
- Ajuste visual proporcional com `object-fit: contain`, evitando deformação.
- Pré-visualização em tempo real e opção de remover/restaurar a marca padrão.
