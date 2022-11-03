#pragma once

#include "Defines.h"

#include "CaffParser.h"

#ifdef WIN32
extern "C" __declspec(dllexport) ulong GeneratePreviewFromCaff(const char* inBuffer, ulong inLength, char* outBuffer, ulong outLength) {
	return CaffParser::GeneratePreviewFromCaff(inBuffer, inLength, outBuffer, outLength);
}
#else
culong GeneratePreviewFromCaff(const char* inBuffer, culong inLength, char* outBuffer, culong outLength) {
	return CaffParser::GeneratePreviewFromCaff(inBuffer, inLength, outBuffer, outLength);
}
#endif