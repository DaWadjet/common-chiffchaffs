#pragma once

#define ulong unsigned long long

class Parser {
public:
	Parser();

	static ulong GeneratePreviewFromCaff(const char* inBuffer, ulong inLen, char* outBuffer, ulong outLen);
};

#ifdef WIN32
extern "C" __declspec(dllexport) ulong GeneratePreviewFromCaff(const char* inBuffer, ulong inLen, char* outBuffer, ulong outLen) {
	return Parser::GeneratePreviewFromCaff(inBuffer, inLen, outBuffer, outLen);
}
#else
ulong TimesTwo(const char* inBuffer, ulong inLen, char* outBuffer, ulong outLen) {
	return Parser::GeneratePreviewFromCaff(inBuffer, inLen, outBuffer, outLen);
}
#endif