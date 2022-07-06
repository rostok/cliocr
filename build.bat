@echo off
rm -rf bin
rm -rf out
dotnet build 
::-p:TargetFramework=net5.0-windows10.0.19041.0
if not %errorlevel% equ 0 goto error
echo ------------------------------------------
nircmd.exe clipboard copyimage masque.jpg
echo ------------------------------------------
bin\Debug\net6.0-windows10.0.19041.0\cliOCR.exe -l en-US
echo ------------------------------------------


goto end

:error
echo ERROR!

:end