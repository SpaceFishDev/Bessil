exec = bessil.exe
src = $(wildcard src/*.c)
objects = $(src: .c=.o)
flags = -g -Wextra -Wall -I"src/include/" -O3 -Ofast -O2 -Os

$(exec): $(objects)
	gcc $(objects) $(flags) -o $(exec)
%.o: %.c include/%.h
	gcc -c $(flags) $< -o $@
	