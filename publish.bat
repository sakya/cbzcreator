rd /s /q .\dist\win-x64
rd /s /q .\CbzCreator\bin
rd /s /q .\CbzCreator\obj
rd /s /q .\CbzCreator.Lib\bin
rd /s /q .\CbzCreator.Lib\obj
rd /s /q .\CbzCreatorGui\bin
rd /s /q .\CbzCreatorGui\obj

dotnet clean CbzCreator.sln -c Release
dotnet publish CbzCreator.sln -c Release --runtime win-x64 -p:PublishReadyToRun=true --self-contained --output .\dist\win-x64