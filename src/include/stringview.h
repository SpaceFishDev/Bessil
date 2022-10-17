#pragma once
#include <stddef.h>
#include<stdio.h>
#include<stdlib.h>


#define STR sv_from_cstring

typedef struct STRVIEW{
	char* source;
	int length;
} string_view;

void print(string_view* sv);
string_view* sv_from_cstring(char* data);
string_view* substring(string_view* from, int start, int end);
void write_sv(FILE* file, string_view* string);
void read_sv(FILE* file, string_view* string);
void concat_sv(string_view* from, string_view* to);
char at(string_view* sv, int pos);
char* sv_as_cstring(string_view* sv);
int cmp(string_view* sv, string_view* to);