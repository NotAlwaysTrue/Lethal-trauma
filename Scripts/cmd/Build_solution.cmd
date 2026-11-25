cd ../../
dotnet build PluginToolbox\PluginToolbox.csproj -c Release /clp:ErrorsOnly /p:Platform="x64" -p WarningLevel=0 -maxcpucount:1
pause