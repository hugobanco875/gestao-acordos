@echo off
REM ============================================================
REM  Instala o Gestao de Acordos como SERVICO do Windows
REM  (inicia sozinho junto com o Windows).
REM  >>> Clique com o botao DIREITO e "Executar como administrador"
REM ============================================================

REM ================= EDITE AQUI =================
set "CONEXAO=Host=localhost;Port=5432;Database=gestaoacordos;Username=gestao;Password=SuaSenha"
set "PORTA=8080"
set "APP=C:\GestaoAcordos\GestaoAcordos.exe"
REM =============================================

set "SERVICO=GestaoAcordos"

net session >nul 2>&1
if errorlevel 1 (
  echo Este script precisa ser executado como ADMINISTRADOR.
  pause
  exit /b 1
)

if not exist "%APP%" (
  echo Nao encontrei o arquivo: "%APP%"
  echo Copie a pasta "publicado" para C:\GestaoAcordos, ou ajuste a variavel APP acima.
  pause
  exit /b 1
)

echo [1/4] Removendo servico antigo (se houver)...
sc stop %SERVICO% >nul 2>&1
sc delete %SERVICO% >nul 2>&1

echo [2/4] Criando o servico...
sc create %SERVICO% binPath= "%APP%" start= auto DisplayName= "Gestao de Acordos"
if errorlevel 1 goto :erro
sc description %SERVICO% "Sistema Gestao de Acordos (ASP.NET Core)"

echo [3/4] Definindo conexao com o banco e porta...
reg add "HKLM\SYSTEM\CurrentControlSet\Services\%SERVICO%" /v Environment /t REG_MULTI_SZ /d "ASPNETCORE_ENVIRONMENT=Production\0ASPNETCORE_URLS=http://+:%PORTA%\0ConnectionStrings__DefaultConnection=%CONEXAO%" /f >nul
if errorlevel 1 goto :erro

echo [4/4] Liberando a porta %PORTA% no firewall e iniciando...
netsh advfirewall firewall add rule name="GestaoAcordos %PORTA%" dir=in action=allow protocol=TCP localport=%PORTA% >nul 2>&1
sc start %SERVICO%
if errorlevel 1 goto :erro

echo.
echo ============================================================
echo  Instalado e rodando como servico!
echo  Nesta maquina:   http://localhost:%PORTA%
echo  De outro aparelho: http://IP-DO-SERVIDOR:%PORTA%
echo.
echo  Para ver/parar: services.msc  (procure "Gestao de Acordos")
echo ============================================================
pause
exit /b 0

:erro
echo.
echo *** Ocorreu um erro. Veja as mensagens acima. ***
echo  - Rodou como Administrador?
echo  - O runtime ASP.NET Core 9 esta instalado?
echo  - O caminho em APP esta correto?
pause
exit /b 1
