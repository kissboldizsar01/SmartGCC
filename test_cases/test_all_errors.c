// Tesztfájl: error típusú hibák
// Lefedi: missing semicolon, undeclared, implicit function, conflicting types,
//         too few/many arguments, incompatible types, dereferencing, subscript

#include <stdio.h>

// Conflicting types: előbb int-ként deklaráljuk...
int szamol(int a, int b);

// ...majd máshogy definiáljuk
float szamol(int a, int b) {
    return a + b;
}

// Too few / too many arguments teszthez
int osszead(int a, int b) {
    return a + b;
}

// Incompatible types teszthez
struct Pont {
    int x;
    int y;
};

int main() {

    // 1. Hiányzó pontosvessző (missing semicolon)
    int szam = 5
        printf("szam: %d\n", szam);

    // 2. Nem deklarált változó (undeclared)
    eredmeny = 10;

    // 3. Implicit függvénydeklaráció (implicit function declaration)
    // (nincs #include <stdlib.h>, mégis hívjuk)
    int mem = malloc(10);

    // 4. Too few arguments
    int a = osszead(5);

    // 5. Too many arguments
    int b = osszead(1, 2, 3);

    // 6. Incompatible types
    int szam2 = 3.14;
    struct Pont p;
    int* ptr = p;

    return 0;
}