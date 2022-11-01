#pragma once

#include "Image.h"

class ParsedCAFF {
public:
	ParsedCAFF();

	void SetPreviewImage(Image image);
	Image GetPreviewImage();

private:
	Image previewImage;
};