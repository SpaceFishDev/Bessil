#include<stdio.h>
#include<stdlib.h>
#include<parser.h>
void print_tree(node_constant* root, int level);

int main(void){
	print_tree(parse(init_lexer_info(STR("4 + 1 + 2 + 5;\n"))), 0);
}



void print_tree(node_constant* root, int level){
	if(root == (void*)0){
		return;
	}
	int i = 0;
	switch(root->base->class){
		case ADD:{
			node_bin_expr* add = (node_bin_expr*)root;
			print(STR("ADD:\n"));
			print_tree(add->left, level);
			print_tree(add->right, level);
		} break;
		case CONSTANT:{
			print(root->value);
			print(STR("\n"));
			print_tree(root->child, level);
			return;
		} break;
		case SUB:{
		} break;
	}	
}