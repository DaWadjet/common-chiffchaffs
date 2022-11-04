#pragma once

#include "Image.h"
#include <memory>

class ParsedCAFF {
public:
	ParsedCAFF();

	void SetPreviewImage(std::shared_ptr<Image> image);
	std::shared_ptr<Image> GetPreviewImage();

private:
	std::shared_ptr<Image> previewImage;
};