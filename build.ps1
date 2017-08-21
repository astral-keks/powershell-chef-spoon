$src = "$PSScriptRoot\src"
$package = "$PSScriptRoot\package"
$artifact = "$PSScriptRoot\artifact\AstralKeks.ChefSpoon"

# Build binaries
if (Test-Path $artifact) { Remove-Item $artifact -Recurse }
if (Test-Path $package) { Remove-Item $package -Recurse }
dotnet restore $src\Command\ChefSpoon.Command.csproj
dotnet build $src\Command\ChefSpoon.Command.csproj --configuration Release -o $artifact