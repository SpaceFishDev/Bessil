#include "include/lexer.h"
#include<string.h>
void assert(int condition, string_view* msg){
	if(condition == 0){
		print(msg);
		exit(-1);
	}
}
void lexer_next(lexer_info* linfo){
	if(linfo->position >= linfo->source->length){
		linfo->current = '\0';
		return;
	}

	linfo->position += 1;
	linfo->current = at(linfo->source, linfo->position);
}


lexer_info* init_lexer_info(string_view* source){
	lexer_info* lex = calloc(1, sizeof(lexer_info));
	lex->current = source->source[0];
	lex->position = 0;
	lex->source = source;
	return lex;
}

token_info* lex(lexer_info* lexer){
	if((lexer->current >= 'a' && lexer->current <= 'z') || (lexer->current >= 'A' && lexer->current <= 'Z')){
		int start = lexer->position;
		while((lexer->current >= 'a' && lexer->current <= 'z') || (lexer->current >= 'A' && lexer->current <= 'Z')){
			lexer_next(lexer);
		}
		int end = lexer->position;
		if(cmp(substring(lexer->source, start, end), STR("func")) == 0){
			return init_token_info(TOKEN_FUNC, substring(lexer->source, start, end));
		}
		return init_token_info(TOKEN_ID, substring(lexer->source, start, end));
	}
	if((lexer->current >= '0' && lexer->current <= '9') || lexer->current == '.'){
		int start = lexer->position;
		int num_dec = 0;
		while((lexer->current >= '0' && lexer->current <= '9') || lexer->current == '.'){
			if(lexer->current == '.')
				++num_dec;
			assert(num_dec < 2 , STR("Multiple '.' in expression"));
			lexer_next(lexer);
		}
		int end = lexer->position;
		
		return init_token_info(TOKEN_EXPR, substring(lexer->source, start, end));
	}
	switch(lexer->current){
		case ' ':
		case '\t':
		case '\n':{
			lexer_next(lexer);
			return lex(lexer);
		} break;
		case '\"':{
			lexer_next(lexer);
			int start = lexer->position;
			while(lexer->current != '\"'){
				assert(lexer->current != '\n', STR("ERROR: STRING NEVER ENDS"));
				lexer_next(lexer);
			}
			int end = lexer->position;
			lexer_next(lexer);
			return init_token_info(TOKEN_STRING, substring(lexer->source, start, end));
		} break;
		case '(':{
			lexer_next(lexer);
			return init_token_info(TOKEN_OPAREN, STR("("));
		} break;
		case ')':{
			lexer_next(lexer);
			return init_token_info(TOKEN_CPAREN, STR(")"));
		} break;
		case '{':{
			lexer_next(lexer);
			return init_token_info(TOKEN_OBRACKET, STR("{"));
		} break;
		case '}':{
			lexer_next(lexer);
			return init_token_info(TOKEN_CBRACKET, STR("}"));
		} break;
		case ';':{
			lexer_next(lexer);
			return init_token_info(TOKEN_SEMI, STR(";"));
		} break;
		case '=':{
			lexer_next(lexer);
			return init_token_info(TOKEN_EQ, STR("="));
		} break;
		case '+':{
			lexer_next(lexer);
			return init_token_info(TOKEN_PLUS, STR("+"));
		} break;
		case '-':{
			lexer_next(lexer);
			return init_token_info(TOKEN_MINUS, STR("-"));
		} break;
		case '*':{
			lexer_next(lexer);
			return init_token_info(TOKEN_STAR, STR("*"));
		} break;
		case '/':{
			lexer_next(lexer);
			return init_token_info(TOKEN_SLASH, STR("/"));
		} break;
		case '\0':{
			goto end;
		}
		case ',':{
			lexer_next(lexer);
			return lex(lexer);
		}
		default:{
			assert(0, STR("Unknown character in input."));
		} break;
	}	
	end:
	return init_token_info( TOKEN_ENDOFFILE, STR("\0"));
}

