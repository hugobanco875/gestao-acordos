@echo off
REM ============================================================
REM  Publica o Gestao de Acordos numa pasta "publicado".
REM  Rode numa maquina que tenha o .NET 9 SDK instalado.
REM ============================================================
set "PROJETO=%~dp0..\.."

echo Publicando a aplicacao...
dotnet publish "%PROJETO%\GestaoAcordos.csproj" -c Release -o "%~dp0publicado"
if errorlevel 1 goto :erro

echo.
echo ============================================================
echo  Pronto! Foi criada a pasta "publicado" aqui do lado.
echo  Copie ela para o servidor (ex.: C:\GestaoAcordos) e la
echo  rode o "instalar-servico.bat" como Administrador.
echo ============================================================
pause
exit /b 0

:erro
echo.
echo *** Erro ao publicar. O .NET 9 SDK esta instalado? ***
pause
exit /b 1
