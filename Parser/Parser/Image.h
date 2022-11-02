#pragma once

#include "Defines.h"

struct Image {
	unsigned char* data;
	culong length;

	Image(unsigned char* data, culong length) : data(data), length(length) {};

	~Image() {
		delete[] data;
	}
};
