#include "CaffParser.h"

#include <exception>
#include <stdexcept>
#include <string>

#include "CiffParser.h"

constexpr int ID_FIELD_SIZE_IN_BYTES = 1;
constexpr int LENGTH_FIELD_SIZE_IN_BYTES = 8;
constexpr int NUM_ANIM_FIELD_SIZE_IN_BYTES = 8;
constexpr int DURATION_FIELD_ANIM_SIZE_IN_BYTES = 8;

CaffParser::CaffParser(const unsigned char* inBuffer, culong inLength) : Parser(inBuffer, inLength) {
}

std::shared_ptr<ParsedCAFF> CaffParser::ParseCAFF() {
	throw std::logic_error("Not implemented");
}

std::shared_ptr<ParsedCAFF> CaffParser::ParseForPreview() {
	auto [index, length] = GetFirstAnimationBlock();
	auto image = ParseAnimationBlock(index, length);
	auto parsed = std::make_shared<ParsedCAFF>();
	parsed->SetPreviewImage(image);
	return parsed;
}

std::pair<culong, int> CaffParser::GetFirstAnimationBlock() {
	auto startResult = ProcessBlockStart(0);
	if (startResult.type != Header)
		throw std::logic_error("The first block should be a header");

	int animNum = ParseHeaderBlock(startResult.indexForData, startResult.length);
	if(animNum < 1)
		throw std::logic_error("The file does not contain animations");

	while (startResult.type != Animation) {
		startResult = ProcessBlockStart(startResult.nextIndex);
	}

	return { startResult.indexForData, startResult.length };
}

ProcessBlockStartResult CaffParser::ProcessBlockStart(culong fromIndex) {
	ProcessBlockStartResult result;
	result.indexForData = fromIndex + ID_FIELD_SIZE_IN_BYTES + LENGTH_FIELD_SIZE_IN_BYTES;
	if (result.indexForData > bufferLength_) {
		throw std::logic_error("Not enough space for reading a block's start");
	}

	switch (buffer_[fromIndex])
	{
	case 0x1:
		result.type = Header;
		break;
	case 0x2:
		result.type = Credits;
		break;
	case 0x3:
		result.type = Animation;
		break;
	default:
		throw std::logic_error("Invalid Block type");
		break;
	}

	result.length = ParseNumber(fromIndex + ID_FIELD_SIZE_IN_BYTES, LENGTH_FIELD_SIZE_IN_BYTES);
	result.nextIndex = result.indexForData + result.length;

	return result;
}

culong CaffParser::ParseHeaderBlock(culong index, int /*length*/) {
	if (index + MAGIC_FIELD_SIZE_IN_BYTES + HEADER_SIZE_FIELD_SIZE_IN_BYTES + NUM_ANIM_FIELD_SIZE_IN_BYTES > bufferLength_)
		throw std::logic_error("Not enough space for reading the header part of a block");

	std::string magicField(buffer_ + index, buffer_ + index + MAGIC_FIELD_SIZE_IN_BYTES);
	if (magicField != "CAFF")
		throw std::logic_error("The magic field has invalid data");

	// header_size is not intresting may be there could be a check

	return ParseNumber(index + MAGIC_FIELD_SIZE_IN_BYTES + HEADER_SIZE_FIELD_SIZE_IN_BYTES, NUM_ANIM_FIELD_SIZE_IN_BYTES);
}

std::shared_ptr<Image> CaffParser::ParseAnimationBlock(culong index, int /*length*/) {
	if (index + DURATION_FIELD_ANIM_SIZE_IN_BYTES > bufferLength_)
		throw std::logic_error("Not enough space for reading an anim block");

	CiffParser parser = CiffParser(buffer_, bufferLength_);
	return parser.Parse(index + DURATION_FIELD_ANIM_SIZE_IN_BYTES);
}

culong CaffParser::GeneratePreviewFromCaff(const char* inBuffer, culong inLength, char* outBuffer, culong outLength) {
	CaffParser parser = CaffParser(reinterpret_cast<const unsigned char*>(inBuffer), inLength);
	auto parsed = parser.ParseForPreview();
	auto image = parsed->GetPreviewImage();
	if (image->length > outLength)
		throw std::logic_error("The Image was too large for the outBuffer");

	auto buffer = reinterpret_cast<char*>(image->data);
	for (int i = 0; i < image->length; i++) {
		outBuffer[i] = buffer[i];
	}
	return image->length;
}