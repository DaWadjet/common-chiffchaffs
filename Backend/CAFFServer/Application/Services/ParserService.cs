using System.Runtime.InteropServices;

namespace Application.Services;

public class ParserService : IParserService
{
    [DllImport(@"../parser_core.dll")]
    private static extern int ParseAnimation();

    public string GetResult() 
    {
        return ParseAnimation().ToString();
    }
}
