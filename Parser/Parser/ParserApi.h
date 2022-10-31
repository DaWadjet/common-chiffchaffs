#pragma once

#include "Defines.h"

#include "Parser.h"

#ifdef WIN32
extern "C" __declspec(dllexport) ulong GeneratePreviewFromCaff(const char* inBuffer, ulong inLen, char* outBuffer, ulong outLen) {
	return Parser::GeneratePreviewFromCaff(inBuffer, inLen, outBuffer, outLen);
}
#else
ulong TimesTwo(const char* inBuffer, ulong inLen, char* outBuffer, ulong outLen) {
	return Parser::GeneratePreviewFromCaff(inBuffer, inLen, outBuffer, outLen);
}
#endif