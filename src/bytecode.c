#include<bytecode.h>

instruction* init_instruction(int type, string_view* data){
	instruction* result = malloc(sizeof(instruction));
	result->type = type;
	result->data = data;
	return result;
}


#define INSTRUCTIONS_MAX 1024 * 16

instruction** compile_bytecode(string_view* source){
	lexer_info* temp_lexer = init_lexer_info(source);

	int size = 0;
	while(1){
		token_info* t = lex(temp_lexer);
		++size;
		if(t->type == TOKEN_ENDOFFILE){
			break;
		}
	}
	token_info** tokens = calloc(size, size * sizeof(token_info*));
	lexer_info* lexer = init_lexer_info(source);
	int index = 0;
	while(1){
		token_info* token = lex(lexer);
		tokens[index] = token;
		++index;
		if(token->type == TOKEN_ENDOFFILE){
			break;
		}
	}
	instruction** instructions = calloc(INSTRUCTIONS_MAX, INSTRUCTIONS_MAX * sizeof(instruction));
	if(!instructions){
		printf("Could not allocate enough memory for compilation");
		exit(-1);
	}
	int i = 0;
	int instruction_index = 0;
	while(1){
		token_info* token = tokens[i];
		if(token->type == TOKEN_ENDOFFILE){
			return instructions;
		}
		switch(token->type){
			case TOKEN_FUNC:{
				if(tokens[i + 1]->type != TOKEN_ID){
					printf("Token of type: %d given when %d was expected", tokens[i + 1]->type, TOKEN_ID);
					exit(-1);
				}
				instructions[instruction_index] = init_instruction(LDEC,  tokens[i + 1]->data);
				i += 2;
				++instruction_index;
			} break;
			case TOKEN_ID:{
				if(tokens[i + 1]->type != TOKEN_OPAREN && tokens[i + 1]->type != TOKEN_EQ){
					printf("Token of type: %d given when %d was expected", tokens[i + 1]->type, TOKEN_ID);
					exit(-1);
				}
				if(tokens[i + 1]->type == TOKEN_EQ){
					//variable
				}
				else
				{
					printf("Got here");
					int size = 0;
					int x = 0;
					while(tokens[i + x]->type != TOKEN_CPAREN){
						++x;
						++size;
					}
					token_info** t_tokens = malloc(size);
					x = 0;
					while(t_tokens[i + x]->type != TOKEN_CPAREN){
						t_tokens[x] = t_tokens[i];
						++x;
						++i;
					}
					//function call
				}
			} break;
			default:{
				printf("Not handled %s\n", sv_as_cstring(token->data));
				++i;
			}
		}
	}

	return instructions;

}