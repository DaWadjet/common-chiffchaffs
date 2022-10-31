#include "pch.h"
#include "ParsedCAFF.h"

ParsedCAFF::ParsedCAFF() {

}

Image ParsedCAFF::GetPreviewImage() {
	ulong length = 50;
	char* temporary = new char[length];
	return Image(temporary, length);
}