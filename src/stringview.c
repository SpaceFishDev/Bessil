#include <stddef.h>
#include<stdio.h>
#include<stdlib.h>
#include<string.h>
#include "include/stringview.h"


string_view* sv_from_cstring(char* data)
{
	int len = strlen(data);
	string_view* sv = calloc(1, sizeof(string_view));
	sv->source = data;
	sv->length = len;
	return sv;
}

string_view* substring(string_view* from, int start, int end){
	int i = 0;
	char* string = malloc(end - start);
	while(i + start != end){
		string[i] = from->source[i + start];
		++i;
	}
	string[end - start] = 0;

	return sv_from_cstring(string);
}
void write_sv(FILE* file, string_view* string){
	fwrite(string->source, string->length, 1, file);
}
void read_sv(FILE* file, string_view* string){
	fread(string->source, string->length, 1, file);
}
void print(string_view* sv){
	write_sv(fopen("CON", "w"), sv);
}

void concat_sv(string_view* from, string_view* to){
	char* new = malloc(from->length + to->length);
	int i = 0;
	while(i != from->length){
		new[i] = from->source[i];
		++i;
	}
	while(i != from->length + to->length){
		new[i] = to->source[i - from->length];
		++i;
	}
	from->length = from->length + to->length;
	from->source = new;
}
// just looks a bit better than sv->source[i]
char at(string_view* sv, int pos){
	return sv->source[pos];
}
char* sv_as_cstring(string_view* sv){
	char* m = malloc(sv->length);
	memcpy(m,  sv->source, sv->length);
	m[sv->length] = 0;
	return m;
}
int cmp(string_view* sv, string_view* to){
	return strcmp(sv_as_cstring(sv), sv_as_cstring(to));
}
