#pragma once

#include "Defines.h"

#include "ParsedCAFF.h"

class Parser {
	// some private stuff incoming

public:
	Parser();

	ParsedCAFF ParseCAFF(const char* inBuffer, ulong inLen);

	static ulong GeneratePreviewFromCaff(const char* inBuffer, ulong inLen, char* outBuffer, ulong outLen);
};