#pragma once

#include <memory>
#include "Defines.h"

#include "Image.h"
#include "Parser.h"

class CiffParser : public Parser {
public:
	CiffParser(const unsigned char* inBuffer, culong inLength);

	std::shared_ptr<Image> Parse(culong startIndex);

private:
	void writeNumber(unsigned char* imageData, culong startIndex, unsigned int number);

private:
	unsigned char* imageData;
};