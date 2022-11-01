#pragma once

#include "Defines.h"

#include "ParsedCAFF.h"
#include <utility>

enum BlockType { Header, Credits, Animation };

struct ProcessBlockStartResult {
	BlockType type;
	int length;
	ulong indexForData;
	ulong nextIndex;
};


class Parser {
public:
	Parser(const char* inBuffer, ulong inLen);

	static ulong GeneratePreviewFromCaff(const char* inBuffer, ulong inLen, char* outBuffer, ulong outLen);

	ParsedCAFF ParseCAFF();
	ParsedCAFF ParseForPreview();


private:
	std::pair<ulong, int> GetFirstAnimationBlock();
	ProcessBlockStartResult ProcessBlockStart(ulong fromIndex);
	int ParseHeaderBlock(ulong index, int length);
	Image ParseAnimationBlock(ulong index, int length);

	Image ParseCiff(ulong startIndex);
	void writeNumber(unsigned char * imageData, ulong startIndex, unsigned int number);

	int ParseNumber(ulong index, int length);

private:
	const char* buffer_;
	ulong bufferLength_;
};