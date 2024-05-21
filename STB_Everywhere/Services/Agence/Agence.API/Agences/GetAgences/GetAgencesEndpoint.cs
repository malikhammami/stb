using Agence.API.Persistance;
using Carter;
using Microsoft.EntityFrameworkCore;
using System;

namespace Agence.API.Agences.GetAgences
{
    public class GetAgencesEndpoint : ICarterModule
    {
        private readonly AgenceDbContext dbContext;

        public GetAgencesEndpoint(AgenceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (HttpRequest request) =>
            {
                var products = await dbContext.Agences.ToListAsync();

                var response = new GetAgencesResult(products);

                return Results.Ok(response);
            })
            .Produces<GetAgencesResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
        }
    }
}
