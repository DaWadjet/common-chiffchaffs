#pragma once

#include "Defines.h"

#include "ParsedCAFF.h"
#include <utility>
#include <memory>
#include "pch.h"

enum BlockType { Header, Credits, Animation };

struct ProcessBlockStartResult {
	BlockType type;
	int length;
	ulong indexForData;
	ulong nextIndex;
};


class Parser {
public:
	Parser(const char* inBuffer, ulong inLength);

	static ulong GeneratePreviewFromCaff(const char* inBuffer, ulong inLength, char* outBuffer, ulong outLength);

	std::shared_ptr<ParsedCAFF> ParseCAFF();
	std::shared_ptr<ParsedCAFF> ParseForPreview();


private:
	std::pair<ulong, int> GetFirstAnimationBlock();
	ProcessBlockStartResult ProcessBlockStart(ulong fromIndex);
	int ParseHeaderBlock(ulong index, int length);
	std::shared_ptr<Image> ParseAnimationBlock(ulong index, int length);

	std::shared_ptr<Image> ParseCiff(ulong startIndex);
	void writeNumber(unsigned char * imageData, ulong startIndex, unsigned int number);

	int ParseNumber(ulong index, int length);

private:
	const char* buffer_;
	ulong bufferLength_;
};