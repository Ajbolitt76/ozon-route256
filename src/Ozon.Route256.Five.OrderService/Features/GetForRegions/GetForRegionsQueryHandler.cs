using Ozon.Route256.Five.OrderService.Contracts.GetForRegions;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Exceptions;
using Ozon.Route256.Five.OrderService.Services.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.GetForRegions;

public class GetForRegionsQueryHandler : IQueryHandler<GetForRegionsQuery, GetForRegionsResponse>
{
    private readonly IOrderRepository _repository;
    private readonly IRegionRepository _regionRepository;

    public GetForRegionsQueryHandler(IOrderRepository repository, IRegionRepository regionRepository)
    {
        _repository = repository;
        _regionRepository = regionRepository;
    }

    public async Task<HandlerResult<GetForRegionsResponse>> Handle(GetForRegionsQuery request, CancellationToken token)
    {
        var unknownRegions = request.Regions
            .Except(await _regionRepository.GetAllRegions(token))
            .ToList();
        
        if(unknownRegions.Any())
            return HandlerResult<GetForRegionsResponse>.FromError(
                new DomainException($"Unknown regions {string.Join(',', unknownRegions)}"));
        
        return new GetForRegionsResponse(
            await _repository.GetForRegions(request.Regions, request.StartFrom, token));
    }
}