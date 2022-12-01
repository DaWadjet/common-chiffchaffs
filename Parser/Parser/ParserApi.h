#pragma once

#include "Defines.h"

#include "CaffParser.h"

static culong GeneratePreview(const char* inBuffer, culong inLength, char* outBuffer, culong outLength);

extern "C" __declspec(dllexport) culong GeneratePreviewFromCaff(const char* inBuffer, culong inLength, char* outBuffer, culong outLength) {
	return GeneratePreview(inBuffer, inLength, outBuffer, outLength);
}