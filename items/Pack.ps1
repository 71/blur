param (
	[string]$Config  = "Release",
	[switch]$Force   = $false,
	[switch]$Push    = $false,
	[string]$MSBuild = "C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe"
)

# Go to root, if needed
If ($(Get-Location).Path.EndsWith("items"))
{
	CD ..
}

# Compile files
Cmd /Q /C """$MSBuild"" src\Blur\Blur.csproj /property:Configuration=$Config"
Cmd /Q /C """$MSBuild"" src\Blur.Library\Blur.Library.csproj /property:Configuration=$Config"

if ($Force)
{
    DotNet build -o build\Core\netstandard1.3 -f netstandard1.3 -c $Config src\Blur.Library\project.json
    DotNet build -o build\Core\netstandard1.5 -f netstandard1.5 -c $Config src\Blur.Library\project.json
}
else
{
    DotNet build --no-incremental -o build\Core\netstandard1.3 -f netstandard1.3 -c $Config src\Blur.Library\project.json
    DotNet build --no-incremental -o build\Core\netstandard1.5 -f netstandard1.5 -c $Config src\Blur.Library\project.json
}

# Start nuget pack
# It crashes if invoked directly by PowerShell
Cmd /Q /C "NuGet pack -Verbosity quiet items\Blur.nuspec"

# Optionally start nuget push
If ($Push)
{
	Cmd /Q /C "NuGet push $((Get-ChildItem *.nupkg | Select -Last 1).Name)"
}