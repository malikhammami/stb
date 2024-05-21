using Agence.API.Models;
using Agence.API.Persistance;
using CQRS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;

namespace Agence.API.Agences.GetAgences
{

    public record GetAgencesQuery() : IQuery<GetAgencesResult>;
    public record GetAgencesResult(IEnumerable<Models.Agence> Agences);

    internal class GetAgencesQueryHandler : IQueryHandler<GetAgencesQuery, GetAgencesResult>
    {
        private readonly AgenceDbContext dbContext;
        private readonly ILogger<GetAgencesQueryHandler> logger;

        public GetAgencesQueryHandler(AgenceDbContext dbContext, ILogger<GetAgencesQueryHandler> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<GetAgencesResult> Handle(GetAgencesQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("GetProductsQueryHandler.Handle called with {@Query}", query);

            var agences = await dbContext.Agences.ToListAsync(cancellationToken);

            return new GetAgencesResult(agences);
        }
    }
}
