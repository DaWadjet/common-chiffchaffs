#include "ParserApi.h"

culong GeneratePreview(const char* inBuffer, culong inLength, char* outBuffer, culong outLength) {
	return CaffParser::GeneratePreviewFromCaff(inBuffer, inLength, outBuffer, outLength);
}