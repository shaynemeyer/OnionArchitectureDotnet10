using Application.Features.ProductFeatures.Commands;
using Application.Features.ProductFeatures.Queries;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.v1;

[ApiVersion("1.0")]
public class ProductController : BaseApiController
{
    /// <summary>Creates a new Product.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        => Ok(await Mediator.Send(command));

    /// <summary>Gets all Products.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await Mediator.Send(new GetAllProductsQuery()));

    /// <summary>Gets a Product by Id.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await Mediator.Send(new GetProductByIdQuery(id)));

    /// <summary>Deletes a Product by Id.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => Ok(await Mediator.Send(new DeleteProductByIdCommand(id)));

    /// <summary>Updates a Product.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id do not match.");

        return Ok(await Mediator.Send(command));
    }
}