#include "Parser.h"

Parser::Parser(const unsigned char* inBuffer, culong inLength) : buffer_(inBuffer), bufferLength_(inLength){}

culong Parser::ParseNumber(culong index, int length)
{
	culong result = 0;
	for (int i = 0; i < length; i++) {
		result |= ((culong)buffer_[i + index]) << (i * 8);
	}
	return result;
}