# cliocr
a simple command line tool for Optical Character Recognition (OCR) directly from clipboard using Windows.Media.Ocr

# Build
To initialize the project execute initialize-project.bat. This will add 3 packages:

* System.Drawing.Common 
* Windows.ApplicationModel.DataTransfer
* Microsoft.Windows.SDK.Contracts

In case you want to do this manually save Program.cs and then overwrite generated template.

Next build it.

```
dotnet build
```

Single exe app cliocr.exe can be generated in out folder via

```
single-exe-build.bat 
```