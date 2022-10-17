#include<stdio.h>
#include<stdlib.h>
#include<bytecode.h>

int main(void){
	compile_bytecode(STR(
		"func main()\n"
		"{\n"
		"print(\"Hello, world!\");\n"
		"}"
		)
	);
}