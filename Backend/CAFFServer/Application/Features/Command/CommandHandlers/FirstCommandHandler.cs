using Application.Eventing.Command.Commands;
using Application.Services;
using MediatR;

namespace Application.Eventing.Command.CommandHandlers;

public class FirstCommandHandler : IRequestHandler<FirstCommand, string>
{
    private readonly IFileService parserService;

    public FirstCommandHandler(IFileService parserService)
    {
        this.parserService = parserService;
    }
    public Task<string> Handle(FirstCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(parserService.GetResult());
    }
}
