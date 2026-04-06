# SmartGCC Windows telepítő

$installDir = "$env:LOCALAPPDATA\Programs\SmartGCC"
$exeSource = Join-Path (Get-Location) "SmartGCC.exe"
$exeDest = "$installDir\smartgcc.exe"

# SmartGCC.exe megléte ellenőrzése
if (-not (Test-Path $exeSource)) {
    Write-Host "❌ Hiba: SmartGCC.exe nem található itt: $exeSource"
    Write-Host "Bizonyosodj meg hogy az install.ps1 és SmartGCC.exe ugyanabban a mappában van!"
    exit 1
}

# Mappa létrehozása ha nem létezik
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Path $installDir | Out-Null
    Write-Host "✅ Mappa létrehozva: $installDir"
}

# Exe másolása
Copy-Item $exeSource $exeDest -Force
Write-Host "✅ SmartGCC.exe másolva ide: $exeDest"

# PATH-ba rakás (felhasználói szintű)
$currentPath = [Environment]::GetEnvironmentVariable("PATH", "User")
if ($currentPath -notlike "*$installDir*") {
    [Environment]::SetEnvironmentVariable("PATH", "$currentPath;$installDir", "User")
    Write-Host "✅ PATH frissítve: $installDir hozzáadva"
} else {
    Write-Host "ℹ️ PATH már tartalmazza: $installDir"
}

Write-Host ""
Write-Host "✅ SmartGCC sikeresen telepítve!"
Write-Host "Indítsd újra a terminált, majd próbáld ki:"
Write-Host "   smartgcc main.c -o myapp"