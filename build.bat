@echo off
dotnet build
if not %errorlevel% equ 0 goto error
echo ------------------------------------------
nircmd.exe clipboard copyimage masque.jpg
echo ------------------------------------------
bin\Debug\netcoreapp3.1\cliOCR.exe -l en-US
echo ------------------------------------------


goto end

:error
echo ERROR!

:end