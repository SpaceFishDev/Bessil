#pragma once
#include "stringview.h"
#include "token.h"
void assert(int condition, string_view* msg);

typedef struct LINFO{
	string_view* source;
	int position;
	char current;
} lexer_info;

lexer_info* init_lexer_info(string_view* source);
void lexer_next(lexer_info* linfo);
token_info* lex(lexer_info* lexer);