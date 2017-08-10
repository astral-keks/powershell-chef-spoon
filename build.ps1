$src = "$PSScriptRoot\src"
$artifact = "$PSScriptRoot\artifact\AstralKeks.ChefSpoon"

# Build binaries
if (Test-Path $artifact) { Remove-Item $artifact -Recurse }
dotnet restore $src\Command\ChefSpoon.Command.csproj
dotnet build $src\Command\ChefSpoon.Command.csproj --configuration Release -o  $artifact