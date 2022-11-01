#include "Parser.h"

#include <exception>
#include <string>

constexpr int ID_FIELD_SIZE_IN_BYTES = 1;
constexpr int LENGTH_FIELD_SIZE_IN_BYTES = 8;
constexpr int MAGIC_FIELD_SIZE_IN_BYTES = 4;
constexpr int HEADER_SIZE_FIELD_SIZE_IN_BYTES = 8;	// used by CIFF and CAFF TOO!!!
constexpr int NUM_ANIM_FIELD_SIZE_IN_BYTES = 8;
constexpr int DURATION_FIELD_ANIM_SIZE_IN_BYTES = 8;
constexpr int CONTENT_SIZE_FIELD_SIZE_IN_BYTES = 8;
constexpr int WIDTH_FIELD_SIZE_IN_BYTES = 8;
constexpr int HEIGHT_FIELD_SIZE_IN_BYTES = 8;

Parser::Parser(const char* inBuffer, ulong inLen) {
	buffer_ = inBuffer;
	bufferLength_ = inLen;
}

ParsedCAFF Parser::ParseCAFF() {
	throw std::exception("Not implemented");
}

ParsedCAFF Parser::ParseForPreview() {
	auto [index, length] = GetFirstAnimationBlock();
	auto image = ParseAnimationBlock(index, length);
}

std::pair<ulong, int> Parser::GetFirstAnimationBlock() {
	auto startResult = ProcessBlockStart(0);
	if (startResult.type != Header)
		throw std::exception("The first block should be a header");

	int animNum = ParseHeaderBlock(startResult.indexForData, startResult.length);
	if(animNum < 1)
		throw std::exception("The file does not contain animations");

	while (startResult.type != Animation) {
		startResult = ProcessBlockStart(startResult.nextIndex);
	}

	return { startResult.indexForData, startResult.length };
}

ProcessBlockStartResult Parser::ProcessBlockStart(ulong fromIndex) {
	ProcessBlockStartResult result;
	result.indexForData = fromIndex + ID_FIELD_SIZE_IN_BYTES + LENGTH_FIELD_SIZE_IN_BYTES;
	if (result.indexForData > fromIndex) {
		throw std::exception("Not enough space for reading a block");
	}

	switch (buffer_[fromIndex])
	{
	case 0x1:
		result.type = Header;
		break;
	case 0x2:
		result.type = Credits;
		break;
	case 0x3:
		result.type = Animation;
		break;
	default:
		throw std::exception("Invalid Block type");
		break;
	}

	result.length = ParseNumber(fromIndex + ID_FIELD_SIZE_IN_BYTES, LENGTH_FIELD_SIZE_IN_BYTES);
	result.nextIndex = result.indexForData + result.length;

	return result;
}

int Parser::ParseHeaderBlock(ulong index, int /*length*/) {
	if (index + MAGIC_FIELD_SIZE_IN_BYTES + HEADER_SIZE_FIELD_SIZE_IN_BYTES + NUM_ANIM_FIELD_SIZE_IN_BYTES > bufferLength_)
		throw std::exception("Not enough space for reading a block");

	std::string magicField(buffer_ + index, buffer_ + index + MAGIC_FIELD_SIZE_IN_BYTES);
	if (magicField != "CAFF")
		throw std::exception("The magic field has invalid data");

	// header_size is not intresting may be there could be a check

	return ParseNumber(index + MAGIC_FIELD_SIZE_IN_BYTES + HEADER_SIZE_FIELD_SIZE_IN_BYTES, NUM_ANIM_FIELD_SIZE_IN_BYTES);
}

Image Parser::ParseAnimationBlock(ulong index, int /*length*/) {
	if (index + DURATION_FIELD_ANIM_SIZE_IN_BYTES > bufferLength_)
		throw std::exception("Not enough space for reading a block");

	return ParseCiff(index + DURATION_FIELD_ANIM_SIZE_IN_BYTES);
}

Image Parser::ParseCiff(ulong index) {

}


int Parser::ParseNumber(ulong index, int length)
{
	int result = 0;
	for (int i = 0; i < length; i++) {
		result |= ((ulong)buffer_[i + index]) << (i * 8);
	}
	return result;
}

ulong Parser::GeneratePreviewFromCaff(const char* inBuffer, ulong inLen, char* outBuffer, ulong outLen) {
	Parser parser = Parser(inBuffer, inLen);
	auto parsed = parser.ParseForPreview();
	auto image = parsed.GetPreviewImage();
	if (image.length > outLen) {
		throw std::exception("The Image was too large for the outBuffer");
	}
	// Obviously it will not work!! just temporary
	outBuffer = image.data;

	return image.length;
}