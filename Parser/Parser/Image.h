#pragma once

#include "Defines.h"

struct Image {
	unsigned char* data;
	ulong length;

	Image(unsigned char* data, ulong length) : data(data), length(length) {};

	~Image() {
		delete[] data;
	}
};
