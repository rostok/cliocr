move Program.cs Program.cs-template
dotnet new console --force --framework "net6.0" 
:: must change <TargetFramework>net6.0</TargetFramework> to <TargetFramework>net6.0-windows10.0.19041.0</TargetFramework> to avoid https://docs.microsoft.com/en-us/dotnet/core/tools/sdk-errors/netsdk1130
powershell -Command "(gc cliocr.csproj ) -replace 'net6.0', 'net6.0-windows10.0.19041.0' | Out-File -encoding ASCII cliocr.csproj "
dotnet add package System.Drawing.Common                 --version 10.0.19041.0
dotnet add package Windows.ApplicationModel.DataTransfer --version 10.0.19041.0
::dotnet add package Microsoft.Windows.SDK.Contracts       --version 10.0.19041.0
move Program.cs-template Program.cs 