
using MediatR;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Domain;
using Microsoft.Extensions.Logging;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Application.Activities
{
    public class List
    {
        public class Query:IRequest<Result<List<ActivityDto>>>{
            
        }

        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        {
            private readonly DataContext context;
           
            private readonly IMapper mapper;

            public Handler(DataContext context,IMapper mapper)
            {
               this.mapper = mapper;
        
                this.context = context;
            }
            public async Task<Result<List<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                       var activities = await context.Activities
                      .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken);

                 
                   
                return  Result<List<ActivityDto>>.Success(activities);
            }
        }
    }
}