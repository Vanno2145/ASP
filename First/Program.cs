using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var rates = new Dictionary<string, decimal>
{
    { "USD_EUR", 0.92m },
    { "EUR_USD", 1.09m },
    { "USD_RUB", 90.5m },
    { "RUB_USD", 0.011m },
    { "EUR_RUB", 98.6m },
    { "RUB_EUR", 0.01m }
};

app.MapGet("/currencies", () =>
{
    var currencies = rates.Keys
        .SelectMany(key => key.Split('_'))
        .Distinct()
        .OrderBy(c => c)
        .ToList();
    return Results.Ok(currencies);
});

app.MapGet("/exchangeRate/{from}/{to}", ([FromRoute] string from, [FromRoute] string to) =>
{
    var key = $"{from.ToUpper()}_{to.ToUpper()}";

    if (rates.TryGetValue(key, out var rate))
    {
        return Results.Ok(new { from, to, rate });
    }
    
    return Results.NotFound(new { error = "Rate not found." });
});

app.MapGet("/convertCurrency/{from}/{to}/{amount}", ([FromRoute] string from, [FromRoute] string to, [FromRoute] decimal amount) =>
{
    var key = $"{from.ToUpper()}_{to.ToUpper()}";

    if (rates.TryGetValue(key, out var rate))
    {
        var result = amount * rate;
        return Results.Ok(new 
        { 
            from, 
            to, 
            amount, 
            convertedAmount = result, 
            rate 
        });
    }
    
    return Results.NotFound(new { error = "Rate not found." });
});

app.Run();