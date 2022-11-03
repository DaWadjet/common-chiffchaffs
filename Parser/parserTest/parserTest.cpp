#include "../Parser/ParserApi.h"

#include <iostream>
#include <fstream>
#include <string>

#define culong unsigned long long //ulong had an error in Linux so I appended a 'c'

int main()
{
    std::string input[] = {"input/1.caff", "input/2.caff" , "input/3.caff"};
    std::string output[] = {"output/1.bmp", "output/2.bmp", "output/3.bmp"};

    for (int i = 0; i < 3; i++) {

        char* inBuffer;
        culong inLength;
        char* outBuffer;
        culong outLength;

        std::ifstream inputFile(input[i], std::ios::out | std::ios::binary);
        if (inputFile.is_open()) {
            inputFile.seekg(0, std::ios::end);
            inLength = inputFile.tellg();
            inputFile.seekg(0, std::ios::beg);
            inBuffer = new char[inLength];
            inputFile.read(inBuffer, inLength);
            inputFile.close();
 
            // It should be enough
            outLength = inLength;
            outBuffer = new char[outLength];
        }
        else {
            std::cout << "Could not open input file." << std::endl;
            return -1;
        }

        try {
            outLength = GeneratePreviewFromCaff(inBuffer, inLength, outBuffer, outLength);
        }
        catch (std::logic_error e) {
            std::cout << e.what() << std::endl;
            return -1;
        }

        std::ofstream outputFile(output[i], std::ios::out | std::ios::binary);
        if (outputFile.is_open()) {
            outputFile.write(outBuffer, outLength);
            outputFile.close();
        }
        else {
            std::cout << "Could not open output file," << std::endl;
            return -1;
        }

        delete[] inBuffer;
        delete[] outBuffer;
        std::cout << input[i] << " is done." << std::endl;

    }
    return 0;
}