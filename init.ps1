dotnet build Jackfruit.Runtime 
dotnet pack Jackfruit.Runtime 
foreach ($pkg in Get-ChildItem -Path "Jackfruit.Runtime/bin/Debug/Jackfruit.Runtime.*.nupkg") {
    dotnet nuget push $pkg --source JackFruit
}

dotnet build .\Jackfruit.IncrementalGenerator
dotnet pack .\Jackfruit.IncrementalGenerator
foreach ($pkg in Get-ChildItem -Path "Jackfruit.IncrementalGenerator/bin/Debug/Jackfruit.IncrementalGenerator.*.nupkg") {
    dotnet nuget push $pkg --source JackFruit
}
