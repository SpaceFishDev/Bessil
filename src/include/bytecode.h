#pragma once
#include<lexer.h>

enum opcodes{
	LDEC,
	LEND,
	CALL,
	ASCII,
	INT,
	LONG,
};

typedef struct instruction{
	int type;
	string_view* data;
	struct instruction** arguments;
} instruction;

instruction* init_instruction(int type, string_view* data);

instruction** compile_bytecode(string_view* source);
