#include<parser.h>

node_constant* parse_r(node_constant* root, node_constant* parent, int index, int i , token_info** tokens){
	while(index < i){
		token_info* token = tokens[index];
		int parent_is_root;
		if(parent->base->class != CONSTANT){
			parent_is_root = 0;
		}else{
			 parent_is_root = (cmp(STR("GLBL"), parent->value) == 0);
		}
		switch(token->type){
			case TOKEN_EXPR:{
				if(tokens[index + 1]->type == TOKEN_PLUS  && tokens[index + 1]->type != TOKEN_SEMI){
					node_bin_expr* node = init_node_bin_expr(1, init_node_constant(tokens[index]->data, (void*)0), init_node_constant(tokens[index + 2]->data, (void*)0)); 
					root->child = node;
					return parse_r(root, node, index + 3, i, tokens );
				}
				else{
					printf("Provided: EXPR, EXPECTED: BINEXPR");
					exit(-1);
				}
			}
			case TOKEN_PLUS:{
				if(parent->base->class != ADD){
					printf("CANNOT ADD TO TYPE %d", parent->base->class);
					exit(-1); 
				}
				if(tokens[index + 1]->type == TOKEN_EXPR){
					node_bin_expr* node = init_node_bin_expr(1,  init_node_constant(tokens[index + 1]->data, (void*)0), ((node_bin_expr*)((node_bin_expr*)parent)->right));
					((node_bin_expr*)parent)->right = node;	
					return parse_r(root, node, index + 2, i, tokens);
				}
			} break;
			case TOKEN_SEMI:{
				return parse_r(root, root, index + 1, i, tokens);
			} break;
		}
		return root;
	}
}

node_constant* parse(lexer_info* lexer)
{
	token_info** tokens = malloc(1);
	int i = 0;
	while(1){
		token_info* t = lex(lexer);
		realloc(tokens, i + 1);
		tokens[i] = t;
		++i;
		if(t->type == TOKEN_ENDOFFILE){
			break;
		}
	}
	int index = 0;
	parse_r(init_node_constant(STR("GLBL"), (void*)0), init_node_constant(STR("GLBL"), (void*)0), index, i + 1, tokens);
}

node_base* init_node(int type){
	node_base* result = malloc(sizeof(node_base));
	result->class = type;
	return result;
}
node_constant* init_node_constant( string_view* value, node_constant* child){
	node_constant* result = malloc(sizeof(node_constant));
	result->base = init_node(CONSTANT);
	result->value = value;
	result->child = child;
	int nchild = 0;
	return result;
}
node_bin_expr* init_node_bin_expr(int plus, node_constant* left, node_constant* right){
	node_bin_expr* result = malloc(sizeof(node_bin_expr));
	result->base = (plus == 1) ? init_node(ADD) : init_node(SUB);
	result->left = left;
	result->right = right;
	return result;
}