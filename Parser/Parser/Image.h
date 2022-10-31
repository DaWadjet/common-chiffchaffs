#pragma once

#include "Defines.h"

struct Image {
	char* data;
	ulong length;

	Image(char* data, ulong length) : data(data), length(length) {};

	~Image() {
		delete[] data;
	}
};
