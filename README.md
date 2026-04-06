# SmartGCC 🔍

GCC fordítási hibák emberi nyelven – C kezdőknek és haladóknak.

## Mit csinál?

A `smartgcc` ugyanúgy használható mint a `gcc`, de a hibaüzeneteket
érthetően, magyarul magyarázza el.

**Hagyományos gcc kimenet:**
main.c:5:6: error: expected ',' or ';' before 'return'

**SmartGCC kimenet:**
❌ HIBA: Hiányzó pontosvessző
📍 Helyszín: main.c (5. sor, 6. oszlop)
💻 Érintett kód:
4 |  int x = 0

5 |  return(0);
💡 Magyarázat: A fordító pontosvesszőt várt...
🔧 Javaslat: Ellenőrizd az előző sor végét...


## Követelmények

- GCC telepítve és PATH-ban elérhető
- Windows: [WinLibs](https://winlibs.com) vagy MSYS2
- Linux: `sudo apt install gcc`

## Telepítés

### Windows
```powershell
# PowerShell adminként:
.\install.ps1
```

### Linux/Mac
```bash
chmod +x install.sh
./install.sh
```

## Használat
```bash
# Ugyanúgy mint a gcc:
smartgcc main.c -o myapp

# Warningokkal:
smartgcc -Wall main.c -o myapp

# Nyers GCC kimenet (SmartGCC formázás nélkül):
smartgcc --raw main.c -o myapp
```

## Fordítás forrásból

Ha magad szeretnéd buildelni:
```bash
cd src
dotnet publish -r win-x64 --self-contained true \
  -p:PublishSingleFile=true -o ../publish/win
```