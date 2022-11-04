#include "ParsedCAFF.h"

ParsedCAFF::ParsedCAFF() {

}

void ParsedCAFF::SetPreviewImage(std::shared_ptr<Image> image) {
	previewImage = image;
}

std::shared_ptr<Image> ParsedCAFF::GetPreviewImage() {
	return previewImage;
}