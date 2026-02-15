using Application.Interfaces;
using MediatR;

namespace Application.Features.ProductFeatures.Commands;

public record DeleteProductByIdCommand(int Id) : IRequest<int>;

public class DeleteProductByIdCommandHandler : IRequestHandler<DeleteProductByIdCommand, int>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductByIdCommandHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<int> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException($"Product {request.Id} not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
