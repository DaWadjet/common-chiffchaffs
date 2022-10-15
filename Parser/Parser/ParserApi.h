#pragma once

class Parser {
public:
	Parser();

	static int TimesTwo(int a);
};

#ifdef WIN32
extern "C" __declspec(dllexport) int TimesTwo(int a) {
	return Parser::TimesTwo(a);
}
#else
int TimesTwo(int a) {
	return Parser::TimesTwo(a);
}
#endif