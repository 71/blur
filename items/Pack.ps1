param (
	[string]$Config = "Release",
	[switch]$Force  = $false,
	[switch]$Push   = $false,
	[string]$Args   = ""
)

if ($Force)
{
	$Args = $Args + " --no-incremental"
}


# Go to root, if needed
If ($(Get-Location).Path.EndsWith("items"))
{
	CD ..
}

# Compile files
MSBuild src\Blur\Blur.csproj /property:Configuration=$Config
MSBuild src\Blur.Library\Blur.Library.csproj /property:Configuration=$Config

DotNet build -o build\Core\netstandard1.3 -f netstandard1.3 -c $Config src\Blur.Library\project.json
DotNet build -o build\Core\netstandard1.5 -f netstandard1.5 -c $Config src\Blur.Library\project.json

# Start nuget pack
# It crashes if invoked directly by PowerShell
Cmd /Q /C "NuGet pack -Verbosity quiet items\Blur.nuspec"

# Optionally start nuget push
If ($Push)
{
	Cmd /Q /C "NuGet push $((Get-ChildItem *.nupkg | Select -Last 1).Name)"
}