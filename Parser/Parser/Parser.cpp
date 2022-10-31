#include "Parser.h"

#include <exception>

Parser::Parser() {

}

ParsedCAFF Parser::ParseCAFF(const char* inBuffer, ulong inLen) {

}

ulong Parser::GeneratePreviewFromCaff(const char* inBuffer, ulong inLen, char* outBuffer, ulong outLen) {
	Parser parser;
	auto parsed = parser.ParseCAFF(inBuffer, inLen);
	auto image = parsed.GetPreviewImage();
	if (image.length > outLen) {
		throw std::exception("The Image was too large for the outBuffer");
	}
	// Obviously it will not work!! just temporary
	outBuffer = image.data;

	return image.length;
}