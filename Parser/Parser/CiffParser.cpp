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
	readCiffHeader(startIndex);

	int rowPadding = headerData.width % BMP_ROW_MULTIPIER;
	imageLength = IMAGE_HEADER_SIZE_IN_BYTES + headerData.width * headerData.height * PIXEL_PLACE_IN_BYTES + headerData.height * rowPadding;
	imageData = new unsigned char[imageLength];

	writeImageHeader(imageLength, headerData.width, headerData.height);

	writePicture(startIndex);

	return std::make_shared<Image>(imageData, imageLength);
}

void CiffParser::readCiffHeader(culong startIndex) {
	
	if (startIndex + MAGIC_FIELD_SIZE_IN_BYTES + HEADER_SIZE_FIELD_SIZE_IN_BYTES + CONTENT_SIZE_FIELD_SIZE_IN_BYTES > bufferLength_)
		throw std::logic_error("Not enough space for reading CIFF block start");

	culong bufferIndex = startIndex + MAGIC_FIELD_SIZE_IN_BYTES;
	std::string magicField(buffer_ + startIndex, buffer_ + bufferIndex);
	headerData.magic = magicField;

	headerData.headerSize = ParseNumber(bufferIndex, HEADER_SIZE_FIELD_SIZE_IN_BYTES);
	bufferIndex += HEADER_SIZE_FIELD_SIZE_IN_BYTES;
	headerData.contentSize = ParseNumber(bufferIndex, CONTENT_SIZE_FIELD_SIZE_IN_BYTES);
	bufferIndex += CONTENT_SIZE_FIELD_SIZE_IN_BYTES;

	if (startIndex + headerData.headerSize + headerData.contentSize > bufferLength_)
		throw std::logic_error("Not enough space for reading the CIFF block");

	headerData.width = ParseNumber(bufferIndex, WIDTH_FIELD_SIZE_IN_BYTES);
	bufferIndex += WIDTH_FIELD_SIZE_IN_BYTES;
	headerData.height = ParseNumber(bufferIndex, HEIGHT_FIELD_SIZE_IN_BYTES);
	bufferIndex += HEIGHT_FIELD_SIZE_IN_BYTES;

	std::string captionAndTags(buffer_ + bufferIndex, buffer_ + startIndex + headerData.headerSize);
	headerData.captionAndTags = captionAndTags;

	checkCiffHeader();
}

void CiffParser::checkCiffHeader() {
	if (headerData.magic != "CIFF")
		throw std::logic_error("The magic field has invalid data");

	if (std::count(headerData.captionAndTags.begin(), headerData.captionAndTags.end(), '\n') != 1)
		throw std::logic_error("There is not exactly one separator in caption and tag");

	if (*(--headerData.captionAndTags.end()) != '\0')
		throw std::logic_error("The tag is not ending with a \\0 character");

	if (headerData.width * headerData.height * PIXEL_PLACE_IN_BYTES != headerData.contentSize)
		throw std::logic_error("Content-size do not match with width and height");
}


void CiffParser::writeImageHeader(culong size, culong width, culong height) {

	for (int i = 0; i < IMAGE_HEADER_SIZE_IN_BYTES; i++) {
		imageData[i] = 0;
	}
	// Needed header informations for BMP files
	imageData[0] = 'B';
	imageData[1] = 'M';
	writeNumber(2, size);
	imageData[10] = 54;
	imageData[14] = 40;
	writeNumber(18, width);
	writeNumber(22, height);
	imageData[26] = 1;
	imageData[28] = 24;
	writeNumber(38, 3780);
	writeNumber(42, 3780);
}


void CiffParser::writeNumber(culong startIndex, unsigned int number) {
	for (int i = 0; i < 4; i++) {
		imageData[startIndex + i] = (unsigned char)(number >> i * 8) & 255;
	}
}

void CiffParser::writePicture(culong startIndex) {
	auto rowPadding = headerData.width % BMP_ROW_MULTIPIER;
	culong bufferIndex = startIndex + headerData.headerSize;
	culong startImageIndex = IMAGE_HEADER_SIZE_IN_BYTES;
	for (int i = headerData.height - 1; i >= 0; i--) {
		culong imageIndex = startImageIndex + i * (headerData.width * PIXEL_PLACE_IN_BYTES + rowPadding);
		for (int c = 0; c < headerData.width; c++) {
			imageData[imageIndex++] = buffer_[bufferIndex+2];
			imageData[imageIndex++] = buffer_[bufferIndex+1];
			imageData[imageIndex++] = buffer_[bufferIndex];
			bufferIndex += 3;
		}
		for (int c = 0; c < rowPadding; c++) {
			imageData[imageIndex++] = 0;
		}
	}
}