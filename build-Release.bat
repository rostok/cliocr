@echo off
dotnet build --configuration Release
if not %errorlevel% equ 0 goto error
echo ------------------------------------------
nircmd.exe clipboard copyimage masque.jpg
echo ------------------------------------------
bin\Release\netcoreapp3.1\cliOCR.exe -l en-US
echo ------------------------------------------

del c:\bin\cliOCR.exe
C:\ProgramData\chocolatey\tools\shimgen.exe -o=c:\bin\cliocr.exe -p=%CD%\bin\Release\netcoreapp3.1\cliocr.exe 

goto end

:error
echo ERROR!

:end