using Ozon.Route256.Five.OrderService.Contracts.GetRegions;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Repository.Abstractions;

namespace Ozon.Route256.Five.OrderService.Features.GetRegions;

public class GetRegionsQueryHandler : IQueryHandler<GetRegionsQuery, GetRegionsResponse>
{
    private readonly IRegionRepository _regionRepository;

    public GetRegionsQueryHandler(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }
    
    public async Task<HandlerResult<GetRegionsResponse>> Handle(GetRegionsQuery request, CancellationToken token)
        => new GetRegionsResponse(await _regionRepository.GetAllRegions(token));
}