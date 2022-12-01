using Application.Features.CommandAggreagte.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IMediator mediator;

        public CommentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Policy = "User")]
        [HttpPost]
        public async Task SaveComment([FromBody] SaveCommentCommand command)
        {
            await mediator.Send(command);
        }

        [Authorize(Policy = "User")]
        [HttpPut]
        public async Task UpdateComment([FromBody] UpdateCommentCommand command)
        {
            await mediator.Send(command);
        }
        
        [Authorize(Policy = "User")]
        [HttpDelete]
        public async Task DeleteComment([FromBody] DeleteCommentCommand command)
        {
            await mediator.Send(command);
        }
    }
}
