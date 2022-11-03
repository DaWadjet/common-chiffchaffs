#include "Parser.h"

#include <exception>
#include <stdexcept>
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

Parser::Parser(const char* inBuffer, culong inLength) {
	buffer_ = inBuffer;
	bufferLength_ = inLength;
}

std::shared_ptr<ParsedCAFF> Parser::ParseCAFF() {
	throw std::logic_error("Not implemented");
}

std::shared_ptr<ParsedCAFF> Parser::ParseForPreview() {
	auto [index, length] = GetFirstAnimationBlock();
	auto image = ParseAnimationBlock(index, length);
	auto parsed = std::make_shared<ParsedCAFF>();
	parsed->SetPreviewImage(image);
	return parsed;
}

std::pair<culong, int> Parser::GetFirstAnimationBlock() {
	auto startResult = ProcessBlockStart(0);
	if (startResult.type != Header)
		throw std::logic_error("The first block should be a header");

	int animNum = ParseHeaderBlock(startResult.indexForData, startResult.length);
	if(animNum < 1)
		throw std::logic_error("The file does not contain animations");

	while (startResult.type != Animation) {
		startResult = ProcessBlockStart(startResult.nextIndex);
	}

	return { startResult.indexForData, startResult.length };
}

ProcessBlockStartResult Parser::ProcessBlockStart(culong fromIndex) {
	ProcessBlockStartResult result;
	result.indexForData = fromIndex + ID_FIELD_SIZE_IN_BYTES + LENGTH_FIELD_SIZE_IN_BYTES;
	if (result.indexForData > bufferLength_) {
		throw std::logic_error("Not enough space for reading a block's start");
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
		throw std::logic_error("Invalid Block type");
		break;
	}

	result.length = ParseNumber(fromIndex + ID_FIELD_SIZE_IN_BYTES, LENGTH_FIELD_SIZE_IN_BYTES);
	result.nextIndex = result.indexForData + result.length;

	return result;
}

int Parser::ParseHeaderBlock(culong index, int /*length*/) {
	if (index + MAGIC_FIELD_SIZE_IN_BYTES + HEADER_SIZE_FIELD_SIZE_IN_BYTES + NUM_ANIM_FIELD_SIZE_IN_BYTES > bufferLength_)
		throw std::logic_error("Not enough space for reading the header part of a block");

	std::string magicField(buffer_ + index, buffer_ + index + MAGIC_FIELD_SIZE_IN_BYTES);
	if (magicField != "CAFF")
		throw std::logic_error("The magic field has invalid data");

	// header_size is not intresting may be there could be a check

	return ParseNumber(index + MAGIC_FIELD_SIZE_IN_BYTES + HEADER_SIZE_FIELD_SIZE_IN_BYTES, NUM_ANIM_FIELD_SIZE_IN_BYTES);
}

std::shared_ptr<Image> Parser::ParseAnimationBlock(culong index, int /*length*/) {
	if (index + DURATION_FIELD_ANIM_SIZE_IN_BYTES > bufferLength_)
		throw std::logic_error("Not enough space for reading an anim block");

	return ParseCiff(index + DURATION_FIELD_ANIM_SIZE_IN_BYTES);
}

std::shared_ptr<Image> Parser::ParseCiff(culong startIndex) {
	if (startIndex + MAGIC_FIELD_SIZE_IN_BYTES + HEADER_SIZE_FIELD_SIZE_IN_BYTES + CONTENT_SIZE_FIELD_SIZE_IN_BYTES > bufferLength_)
		throw std::logic_error("Not enough space for reading CIFF block start");

	culong bufferIndex = startIndex + MAGIC_FIELD_SIZE_IN_BYTES;
	std::string magicField(buffer_ + startIndex, buffer_ + bufferIndex);
	if (magicField != "CIFF")
		throw std::logic_error("The magic field has invalid data");


	auto headerSize = ParseNumber(bufferIndex, HEADER_SIZE_FIELD_SIZE_IN_BYTES);
	bufferIndex += HEADER_SIZE_FIELD_SIZE_IN_BYTES;
	auto contentSize = ParseNumber(bufferIndex, CONTENT_SIZE_FIELD_SIZE_IN_BYTES);
	bufferIndex += CONTENT_SIZE_FIELD_SIZE_IN_BYTES;

	if (startIndex + headerSize + contentSize > bufferLength_)
		throw std::logic_error("Not enough space for reading the CIFF block");

	auto width = ParseNumber(bufferIndex, WIDTH_FIELD_SIZE_IN_BYTES);
	bufferIndex += WIDTH_FIELD_SIZE_IN_BYTES;
	auto height = ParseNumber(bufferIndex, HEIGHT_FIELD_SIZE_IN_BYTES);
	bufferIndex += HEIGHT_FIELD_SIZE_IN_BYTES;

	std::string captionAndTags(buffer_ + bufferIndex, buffer_ + startIndex + headerSize);
	if(std::count(captionAndTags.begin(), captionAndTags.end(), '\n') != 1)
		throw std::logic_error("There is not exactly one separator in caption and tag");

	if (*(captionAndTags.end()--) != '\0')
		throw std::logic_error("The tag is not ending with a \\0 character");

	int rowPadding = width * PIXEL_PLACE_IN_BYTES % BMP_ROW_MULTIPIER;
	culong size = IMAGE_HEADER_SIZE_IN_BYTES + width * height * PIXEL_PLACE_IN_BYTES + height * rowPadding;
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

	culong imageIndex = IMAGE_HEADER_SIZE_IN_BYTES;
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

	return std::make_shared<Image>(imageData, imageIndex);
}

void Parser::writeNumber(unsigned char* imageData, culong startIndex, unsigned int number) {
	for (int i = 0; i < 4; i++) {
		imageData[startIndex + i] = (unsigned char)(number >> i * 8) & 255;
	}
}

int Parser::ParseNumber(culong index, int length)
{
	int result = 0;
	for (int i = 0; i < length; i++) {
		result |= ((culong)buffer_[i + index]) << (i * 8);
	}
	return result;
}

culong Parser::GeneratePreviewFromCaff(const char* inBuffer, culong inLength, char* outBuffer, culong outLength) {
	Parser parser = Parser(inBuffer, inLength);
	auto parsed = parser.ParseForPreview();
	auto image = parsed->GetPreviewImage();
	if (image->length > outLength)
		throw std::logic_error("The Image was too large for the outBuffer");

	for (int i = 0; i < image->length; i++) {
		outBuffer[i] = image->data[i];
	}
	return image->length;
}