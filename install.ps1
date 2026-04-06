# SmartGCC Windows telepítő
$installDir = "$env:USERPROFILE\.local\bin"
$exeSource = "$PSScriptRoot\publish\win\SmartGCC.exe"
$exeDest = "$installDir\smartgcc.exe"

# Mappa létrehozása ha nem létezik
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir | Out-Null
}

# Fájl másolása
Copy-Item $exeSource $exeDest -Force

# PATH-ba rakás ha még nincs benne
$currentPath = [Environment]::GetEnvironmentVariable("PATH", "User")
if ($currentPath -notlike "*$installDir*") {
    [Environment]::SetEnvironmentVariable(
        "PATH", 
        "$currentPath;$installDir", 
        "User"
    )
    Write-Host "✅ PATH frissítve: $installDir hozzáadva"
}

Write-Host "✅ SmartGCC telepítve! Indítsd újra a terminált, majd próbáld:"
Write-Host "   smartgcc main.c -o myapp"