using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.ProductFeatures.Queries;

public record GetProductByIdQuery(int Id) : IRequest<Product>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
        => _context = context;

    public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        => await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new KeyNotFoundException($"Product {request.Id} not found.");
}