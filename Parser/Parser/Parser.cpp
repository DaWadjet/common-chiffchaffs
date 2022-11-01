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
constexpr int PIXEL_PLACE_IN_BYTES = 3;
constexpr int IMAGE_HEADER_SIZE_IN_BYTES = 54;
constexpr int BMP_ROW_MULTIPIER = 4;

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

	ulong bufferIndex = startIndex + MAGIC_FIELD_SIZE_IN_BYTES;
	std::string magicField(buffer_ + startIndex, buffer_ + bufferIndex);
	if (magicField != "CIFF")
		throw std::exception("The magic field has invalid data");


	auto headerSize = ParseNumber(bufferIndex, HEADER_SIZE_FIELD_SIZE_IN_BYTES);
	bufferIndex += HEADER_SIZE_FIELD_SIZE_IN_BYTES;
	auto contentSize = ParseNumber(bufferIndex, CONTENT_SIZE_FIELD_SIZE_IN_BYTES);
	bufferIndex += CONTENT_SIZE_FIELD_SIZE_IN_BYTES;

	if (startIndex + headerSize + contentSize > bufferLength_)
		throw std::exception("Not enough space for reading a block");

	auto width = ParseNumber(bufferIndex, WIDTH_FIELD_SIZE_IN_BYTES);
	bufferIndex += WIDTH_FIELD_SIZE_IN_BYTES;
	auto height = ParseNumber(bufferIndex, HEIGHT_FIELD_SIZE_IN_BYTES);
	bufferIndex += HEIGHT_FIELD_SIZE_IN_BYTES;

	std::string captionAndTags(buffer_ + bufferIndex, buffer_ + startIndex + headerSize);
	if(std::count(captionAndTags.begin(), captionAndTags.end(), '\n') != 1)
		throw std::exception("There is not exactly one separator in caption and tag");

	if (!captionAndTags.ends_with('\0'))
		throw std::exception("The tag is not ending with a \\0 character");

	int rowPadding = width * PIXEL_PLACE_IN_BYTES % BMP_ROW_MULTIPIER;
	ulong size = IMAGE_HEADER_SIZE_IN_BYTES + width * height * PIXEL_PLACE_IN_BYTES + height * rowPadding;
	unsigned char* imageData = new unsigned char[size];

	for (int i = 0; i < IMAGE_HEADER_SIZE_IN_BYTES; i++) {
		imageData[i] = 0;
	}
	imageData[0] = 'B';
	imageData[1] = 'M';
	writeNumber(imageData, 2, size);
	imageData[13] = 54;
	imageData[17] = 40;
	writeNumber(imageData, 18, width);
	writeNumber(imageData, 22, height);
	imageData[26] = 1;
	imageData[28] = 24;
	writeNumber(imageData, 38, 3780);
	writeNumber(imageData, 42, 3780);

	ulong imageIndex = IMAGE_HEADER_SIZE_IN_BYTES;
	for (int i = 0; i < height; i++) {
		for (int c = 0; c < width; c++) {
			imageData[imageIndex++] = buffer_[bufferIndex++]; //maybe we have to swap this and the last because r,g,b
			imageData[imageIndex++] = buffer_[bufferIndex++];
			imageData[imageIndex++] = buffer_[bufferIndex++];
		}
		for (int c = 0; c < rowPadding; c++) {
			imageData[imageIndex++] = 0;
		}
	}
}

void Parser::writeNumber(unsigned char* imageData, ulong startIndex, unsigned int number) {
	for (int i = 0; i < 4; i++) {
		imageData[startIndex + i] = (unsigned char)(number >> i * 8) & 255;
	}
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