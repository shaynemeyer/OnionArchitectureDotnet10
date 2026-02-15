using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProductFeatures.Commands;

public record UpdateProductCommand(
    int Id,
    string Name,
    string Barcode,
    string Description,
    decimal Rate) : IRequest<int>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<int> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException($"Product {request.Id} not found.");

        product.Name = request.Name;
        product.Barcode = request.Barcode;
        product.Description = request.Description;
        product.Rate = request.Rate;

        await _context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}