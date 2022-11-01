#include "pch.h"
#include "ParsedCAFF.h"

ParsedCAFF::ParsedCAFF() {

}

void ParsedCAFF::SetPreviewImage(Image image) {
	previewImage = image;
}

Image ParsedCAFF::GetPreviewImage() {
	return previewImage;
}