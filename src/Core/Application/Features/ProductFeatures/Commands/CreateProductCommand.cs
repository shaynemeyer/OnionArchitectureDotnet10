using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.ProductFeatures.Commands;

public record CreateProductCommand(
    string Name,
    string Barcode,
    string Description,
    decimal Rate) : IRequest<int>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Barcode = request.Barcode,
            Description = request.Description,
            Rate = request.Rate
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}