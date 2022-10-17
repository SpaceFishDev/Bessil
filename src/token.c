#include "include/stringview.h"
#include "include/token.h"
token_info* init_token_info( int type, string_view* string){
	token_info* tinfo = calloc(1, sizeof(token_info));
	tinfo->data = string;
	tinfo->type = type;
	return tinfo;
}