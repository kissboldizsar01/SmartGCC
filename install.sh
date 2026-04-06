#!/bin/bash
# SmartGCC Linux/Mac telepítő

INSTALL_DIR="/usr/local/bin"
BINARY_SOURCE="./publish/linux/smartgcc"

echo "SmartGCC telepítése..."

# Futtatási jog
chmod +x "$BINARY_SOURCE"

# Másolás globális helyre
sudo cp "$BINARY_SOURCE" "$INSTALL_DIR/smartgcc"

echo "✅ SmartGCC telepítve! Próbáld ki:"
echo "   smartgcc main.c -o myapp"