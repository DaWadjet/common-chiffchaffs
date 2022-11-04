#pragma once

#include "Defines.h"

class Parser {
public:
	Parser(const unsigned char* inBuffer, culong inLength);
protected:
	culong ParseNumber(culong index, int length);

protected:
	const unsigned char* buffer_ = nullptr;
	culong bufferLength_ = 0;
};
