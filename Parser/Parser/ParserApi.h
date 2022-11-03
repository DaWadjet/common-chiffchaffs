#pragma once

#include "Defines.h"

#include "Parser.h"

#ifdef WIN32
extern "C" __declspec(dllexport) ulong GeneratePreviewFromCaff(const char* inBuffer, ulong inLength, char* outBuffer, ulong outLength) {
	return Parser::GeneratePreviewFromCaff(inBuffer, inLength, outBuffer, outLength);
}
#else
culong GeneratePreviewFromCaff(const char* inBuffer, culong inLength, char* outBuffer, culong outLength) {
	return Parser::GeneratePreviewFromCaff(inBuffer, inLength, outBuffer, outLength);
}
#endif