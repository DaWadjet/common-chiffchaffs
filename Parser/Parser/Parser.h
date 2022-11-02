#pragma once

#include "Defines.h"

#include "ParsedCAFF.h"
#include <utility>
#include <memory>

enum BlockType { Header, Credits, Animation };

struct ProcessBlockStartResult {
	BlockType type;
	int length;
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
	int ParseHeaderBlock(culong index, int length);
	std::shared_ptr<Image> ParseAnimationBlock(culong index, int length);

	std::shared_ptr<Image> ParseCiff(culong startIndex);
	void writeNumber(unsigned char * imageData, culong startIndex, unsigned int number);

	int ParseNumber(culong index, int length);

private:
	const char* buffer_;
	culong bufferLength_;
};