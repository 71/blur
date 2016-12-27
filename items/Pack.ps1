# Go to root, if needed
If ($(Get-Location).Path.EndsWith("items"))
{
    CD ..
}

# Compile files
MSBuild src\Blur\Blur.csproj /property:Configuration=Release>nul
MSBuild src\Blur.Library\Blur.Library.csproj /property:Configuration=Release>nul

DotNet build -o build\Core\netstandard1.3 -f netstandard1.3 -c Release src\Blur.Library\project.json
DotNet build -o build\Core\netstandard1.5 -f netstandard1.5 -c Release src\Blur.Library\project.json

# Start nuget pack
# It crashes if invoked directly by PowerShell
Cmd /Q /C "NuGet pack -Verbosity quiet items\Blur.nuspec"

# Optionally start nuget push
Write-Host "Do you want to push the file?"
$Resp = Read-Host

If ($Resp -like "y*")
{
    Cmd /Q /C "NuGet push $((Get-ChildItem *.nupkg | Select -Last 1).Name)"
}