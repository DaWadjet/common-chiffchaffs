using System.Runtime.InteropServices;

namespace Application.Services;

public class ParserService : IParserService
{
    [DllImport(@"../../../Parser/x64/Debug/Parser.dll")]
    private static extern int TimesTwo(int a);

    public string GetResult() 
    {
        return TimesTwo(4).ToString();
    }
}
