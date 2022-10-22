#include<lexer.h>

typedef struct node_base{
	enum {GLOBAL,CONSTANT, ADD, SUB, BASE} class;
} node_base;


typedef struct node_constant{
	node_base* base;
	string_view* value;
	struct node_constant* child;
} node_constant; 

typedef struct node_binary_expression{
	node_base* base;
	node_base* left;
	node_base* right;
} node_bin_expr;

node_base* init_node(int type);
node_constant* init_node_constant(string_view* value, node_constant* child);
node_bin_expr* init_node_bin_expr(int plus, node_constant* left, node_constant* right);
node_constant* parse(lexer_info* lexer);