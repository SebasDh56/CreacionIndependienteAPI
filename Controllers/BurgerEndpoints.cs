﻿using Microsoft.EntityFrameworkCore;
using AQApi2.Data;
using AQApi2.Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
namespace AQApi2.Controllers;

public static class BurgerEndpoints
{
    public static void MapBurgerEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Burger").WithTags(nameof(Burger));

        group.MapGet("/", async (AmableQuishpeDbContext db) =>
        {
            return await db.Burgers.ToListAsync();
        })
        .WithName("GetAllBurgers")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Burger>, NotFound>> (int burgerid, AmableQuishpeDbContext db) =>
        {
            return await db.Burgers.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Burgerid == burgerid)
                is Burger model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetBurgerById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int burgerid, Burger burger, AmableQuishpeDbContext db) =>
        {
            var affected = await db.Burgers
                .Where(model => model.Burgerid == burgerid)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Burgerid, burger.Burgerid)
                    .SetProperty(m => m.Name, burger.Name)
                    .SetProperty(m => m.Withcheese, burger.Withcheese)
                    .SetProperty(m => m.Price, burger.Price)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateBurger")
        .WithOpenApi();

        group.MapPost("/", async (Burger burger, AmableQuishpeDbContext db) =>
        {
            db.Burgers.Add(burger);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Burger/{burger.Burgerid}",burger);
        })
        .WithName("CreateBurger")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int burgerid, AmableQuishpeDbContext db) =>
        {
            var affected = await db.Burgers
                .Where(model => model.Burgerid == burgerid)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteBurger")
        .WithOpenApi();
    }
}
