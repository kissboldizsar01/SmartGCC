// Tesztfájl: warning típusú hibák
// Lefedi: unused variable, unused but set, control reaches end,
//         return type defaults to int, format mismatch, division by zero

#include <stdio.h>

// Return type defaults to int (hianyzo visszateresi tipus)
osszead(int a, int b) {
    return a + b;
}

// Control reaches end of non-void function
int osztaly(int pont) {
    if (pont >= 50) {
        return 1;
    }
    // hianyzo return az else agbol!
}

int main(void) {

    // 1. Unused variable
    int felhasznalatlan = 99;

    // 2. Unused but set variable
    int beallitott = 0;
    beallitott = 42;

    // 3. Format mismatch (%d helyett float jön)
    float pi = 3.14;
    printf("Pi erteke: %d\n", pi);

    // 4. Division by 
    int eredmeny = 10 / 0;

    printf("eredmeny: %d\n", eredmeny);

    return 0;
}