rm StatApp.exe

csc -define:DEBUG -optimize -out:StatApp.exe *.cs Controls/*.cs Statistics/*.cs Forms/*.cs

mono StatApp.exe