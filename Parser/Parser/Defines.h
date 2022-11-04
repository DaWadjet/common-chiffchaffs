#pragma once

#define culong unsigned long long //ulong had an error in Linux so I appended a 'c'

// used by both CIFF and CAFF constants!!!
constexpr int IMAGE_HEADER_SIZE_IN_BYTES = 54;
constexpr int MAGIC_FIELD_SIZE_IN_BYTES = 4;
constexpr int HEADER_SIZE_FIELD_SIZE_IN_BYTES = 8;