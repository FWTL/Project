using System;
using AutoMapper;
using FWTL.Core.Services;

namespace FWTL.Common.Services
{
    public class RequestToQueryMapper : IRequestToQueryMapper
    {
        private readonly IMapper _mapper;

        public RequestToQueryMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        TQuery IRequestToQueryMapper.Map<TRequest, TQuery>(TRequest request)
        {
            return _mapper.Map<TRequest, TQuery>(request);
        }

        TQuery IRequestToQueryMapper.Map<TRequest, TQuery>(TRequest request, Action<TQuery> afterMap)
        {
            var query = _mapper.Map<TRequest, TQuery>(request);
            afterMap(query);
            return query;
        }
    }
}