#include "CiffParser.h"

#include <stdexcept>
#include <string>
#include <algorithm>

constexpr int CONTENT_SIZE_FIELD_SIZE_IN_BYTES = 8;
constexpr int WIDTH_FIELD_SIZE_IN_BYTES = 8;
constexpr int HEIGHT_FIELD_SIZE_IN_BYTES = 8;
constexpr int PIXEL_PLACE_IN_BYTES = 3;
constexpr int BMP_ROW_MULTIPIER = 4;

CiffParser::CiffParser(const unsigned char* inBuffer, culong inLength) : Parser(inBuffer, inLength) {}

std::shared_ptr<Image> CiffParser::Parse(culong startIndex) {
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
	if (std::count(captionAndTags.begin(), captionAndTags.end(), '\n') != 1)
		throw std::logic_error("There is not exactly one separator in caption and tag");

	if (*(--captionAndTags.end()) != '\0')
		throw std::logic_error("The tag is not ending with a \\0 character");

	bufferIndex = startIndex + headerSize;
	int rowPadding = (width * PIXEL_PLACE_IN_BYTES) % BMP_ROW_MULTIPIER;
	culong size = IMAGE_HEADER_SIZE_IN_BYTES + width * height * PIXEL_PLACE_IN_BYTES + height * rowPadding;

	if (width * height * PIXEL_PLACE_IN_BYTES != contentSize)
		throw std::logic_error("Content-size do not match with width and height");

	imageData = new unsigned char[size];

	for (int i = 0; i < IMAGE_HEADER_SIZE_IN_BYTES; i++) {
		imageData[i] = 0;
	}
	imageData[0] = 'B';
	imageData[1] = 'M';
	writeNumber(imageData, 2, size);
	imageData[10] = 54;
	imageData[14] = 40;
	writeNumber(imageData, 18, width);
	writeNumber(imageData, 22, height);
	imageData[26] = 1;
	imageData[28] = 24;
	writeNumber(imageData, 38, 3780);
	writeNumber(imageData, 42, 3780);

	culong imageIndex = IMAGE_HEADER_SIZE_IN_BYTES;
	for (int i = 0; i < height; i++) {
		for (int c = 0; c < width; c++) {
			imageData[imageIndex++] = buffer_[bufferIndex + 2]; //maybe we have to swap this
			imageData[imageIndex++] = buffer_[bufferIndex + 1];
			imageData[imageIndex++] = buffer_[bufferIndex];
			bufferIndex += 3;
		}
		for (int c = 0; c < rowPadding; c++) {
			imageData[imageIndex++] = 0;
		}
	}

	return std::make_shared<Image>(imageData, imageIndex);
}


void CiffParser::writeNumber(unsigned char* imageData, culong startIndex, unsigned int number) {
	for (int i = 0; i < 4; i++) {
		imageData[startIndex + i] = (unsigned char)(number >> i * 8) & 255;
	}
}