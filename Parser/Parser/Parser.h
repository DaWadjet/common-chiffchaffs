#pragma once

#include "Defines.h"

#include "ParsedCAFF.h"
#include <utility>
#include <memory>

enum BlockType { Header = 1, Credits = 2, Animation = 3 };

struct ProcessBlockStartResult {
	BlockType type;
	culong length;
	culong indexForData;
	culong nextIndex;
};


class Parser {
public:
	Parser(const char* inBuffer, culong inLength);

	static culong GeneratePreviewFromCaff(const char* inBuffer, culong inLength, char* outBuffer, culong outLength);

	std::shared_ptr<ParsedCAFF> ParseCAFF();
	std::shared_ptr<ParsedCAFF> ParseForPreview();


private:
	std::pair<culong, int> GetFirstAnimationBlock();
	ProcessBlockStartResult ProcessBlockStart(culong fromIndex);
	culong ParseHeaderBlock(culong index, int length);
	std::shared_ptr<Image> ParseAnimationBlock(culong index, int length);

	std::shared_ptr<Image> ParseCiff(culong startIndex);
	void writeNumber(unsigned char * imageData, culong startIndex, unsigned int number);

	culong ParseNumber(culong index, int length);

private:
	const unsigned char* buffer_;
	culong bufferLength_;
};