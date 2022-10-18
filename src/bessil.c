#include<stdio.h>
#include<stdlib.h>
#include<lexer.h>

int main(void){
	lexer_info* lexer = init_lexer_info(STR(
		"func main()\n"
		"{\n"
		"print(\"Hello, world!\");\n"
		"}"
		)
	);
}