#pragma once

#include <memory>
#include <string>
#include "Defines.h"

#include "Image.h"
#include "Parser.h"

class CiffParser : public Parser {
public:
	CiffParser(const unsigned char* inBuffer, culong inLength);

	std::shared_ptr<Image> Parse(culong startIndex);

private:
	void readCiffHeader(culong startIndex);
	void checkCiffHeader();

	void writeImageHeader(culong size, culong width, culong height);
	void writeNumber(culong startIndex, unsigned int number);
	void writePicture(culong startIndex);

private:
	struct ImageHeaderData {
		std::string magic;
		culong headerSize;
		culong contentSize;
		culong width;
		culong height;
		std::string captionAndTags;
	};

	unsigned char* imageData = nullptr;
	culong imageLength = 0;
	ImageHeaderData headerData;
};