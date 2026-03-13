param(
    [int]$Port = 5057
)

$projectPath = Join-Path $PSScriptRoot "..\AutocleanManager.Api\AutocleanManager.Api.csproj"
$projectPath = (Resolve-Path $projectPath).Path

$dotnetCommand = Get-Command dotnet -ErrorAction SilentlyContinue
if ($dotnetCommand) {
    $dotnetExe = $dotnetCommand.Source
} else {
    $fallback = "C:\Program Files\dotnet\dotnet.exe"
    if (Test-Path $fallback) {
        $dotnetExe = $fallback
    } else {
        Write-Error "Nao foi encontrado o dotnet. Instale o SDK .NET 10."
        exit 1
    }
}

$url = "http://localhost:$Port"

Write-Host "Usando dotnet em: $dotnetExe"
Write-Host "Projeto: $projectPath"
Write-Host "Iniciando API em: $url"

& $dotnetExe restore $projectPath
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

& $dotnetExe run --project $projectPath --urls $url
exit $LASTEXITCODE
