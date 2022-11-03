CXX = g++
CXXFLAGS = -std=c++17 #-O0 #-nostartfiles
#VALGRINDFLAG = valgrind --leak-check=yes
AFL_CXX = afl-g++

compilation:
	${VALGRINDFLAG} ${CXX} ${CXXFLAGS} Parser/*.cpp parserTest/*cpp -o parserTest/parser

clean:
	rm Debug/parser
	rm Debug/aflParser