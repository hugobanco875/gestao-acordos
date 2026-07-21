@echo off
REM ============================================================
REM  Remove o servico do Windows do Gestao de Acordos.
REM  >>> Execute como Administrador.
REM ============================================================
set "SERVICO=GestaoAcordos"
set "PORTA=8080"

net session >nul 2>&1
if errorlevel 1 (
  echo Execute como ADMINISTRADOR.
  pause
  exit /b 1
)

echo Parando e removendo o servico...
sc stop %SERVICO% >nul 2>&1
sc delete %SERVICO%
netsh advfirewall firewall delete rule name="GestaoAcordos %PORTA%" >nul 2>&1

echo.
echo Servico removido. (Os dados no banco NAO foram apagados.)
pause
