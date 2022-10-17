#pragma once
#include "stringview.h"

enum TOKEN_TYPES{
	TOKEN_ID,
	TOKEN_EXPR,
	TOKEN_PLUS,
	TOKEN_MINUS,
	TOKEN_STAR,
	TOKEN_SLASH,
	TOKEN_STRING,
	TOKEN_ENDOFFILE,
	TOKEN_OPAREN,
	TOKEN_CPAREN,
	TOKEN_OBRACKET,
	TOKEN_CBRACKET,
	TOKEN_SEMI,
	TOKEN_EQ,
	TOKEN_FUNC,
	TOKEN_COMMA,
	BAD_TOKEN,
};

typedef struct TINFO{
	int type;
	string_view* data;
} token_info;
token_info* init_token_info( int type, string_view* string);
