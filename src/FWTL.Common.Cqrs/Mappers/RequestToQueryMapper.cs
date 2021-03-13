using System;
using AutoMapper;

namespace FWTL.Common.Cqrs.Mappers
{
    public class RequestToQueryMapper
    {
        private readonly IMapper _mapper;

        public RequestToQueryMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public TQuery Map<TRequest, TQuery>(TRequest request)
        {
            return _mapper.Map<TRequest, TQuery>(request);
        }

        public TQuery Map<TRequest, TQuery>(TRequest request, Action<TQuery> afterMap)
        {
            var query = _mapper.Map<TRequest, TQuery>(request);
            afterMap(query);
            return query;
        }
    }
}