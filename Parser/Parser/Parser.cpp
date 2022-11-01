#include "Parser.h"

#include <exception>
#include <string>
#include <algorithm>

constexpr int ID_FIELD_SIZE_IN_BYTES = 1;
constexpr int LENGTH_FIELD_SIZE_IN_BYTES = 8;
constexpr int MAGIC_FIELD_SIZE_IN_BYTES = 4;		// used by both CIFF and CAFF!!!
constexpr int HEADER_SIZE_FIELD_SIZE_IN_BYTES = 8;	// used by both CIFF and CAFF!!!
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

Image Parser::ParseCiff(ulong startIndex) {
	if (startIndex + MAGIC_FIELD_SIZE_IN_BYTES + HEADER_SIZE_FIELD_SIZE_IN_BYTES + CONTENT_SIZE_FIELD_SIZE_IN_BYTES > bufferLength_)
		throw std::exception("Not enough space for reading a block");

	ulong index = startIndex + MAGIC_FIELD_SIZE_IN_BYTES;
	std::string magicField(buffer_ + startIndex, buffer_ + index);
	if (magicField != "CIFF")
		throw std::exception("The magic field has invalid data");


	auto headerSize = ParseNumber(index, HEADER_SIZE_FIELD_SIZE_IN_BYTES);
	index += HEADER_SIZE_FIELD_SIZE_IN_BYTES;
	auto contentSize = ParseNumber(index, CONTENT_SIZE_FIELD_SIZE_IN_BYTES);
	index += CONTENT_SIZE_FIELD_SIZE_IN_BYTES;

	if (startIndex + headerSize + contentSize > bufferLength_)
		throw std::exception("Not enough space for reading a block");

	auto width = ParseNumber(index, WIDTH_FIELD_SIZE_IN_BYTES);
	index += WIDTH_FIELD_SIZE_IN_BYTES;
	auto height = ParseNumber(index, HEIGHT_FIELD_SIZE_IN_BYTES);
	index += HEIGHT_FIELD_SIZE_IN_BYTES;

	std::string captionAndTags(buffer_ + index, buffer_ + startIndex + headerSize);
	if(std::count(captionAndTags.begin(), captionAndTags.end(), '\n') != 1)
		throw std::exception("There is not exactly one separator in caption and tag");

	if (!captionAndTags.ends_with('\0'))
		throw std::exception("The tag is not ending with a \\0 character");


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