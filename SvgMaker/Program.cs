using SvgMaker.Lib;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello There!");
app.MapGet("/random", () => 
    Results.Content(SvgGenerator.GenerateRandomSvg(330,250)
        ,"image/svg+xml; charset=utf-8"));
app.MapGet("/random/{width}/{height}", (int width, int height) => 
    Results.Content(SvgGenerator.GenerateRandomSvg(width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/rectangle/{width}/{height}", (int width, int height) => 
    Results.Content(SvgGenerator.GenerateRandomRectanglesSvg(10,width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/circles/{width}/{height}", (int width, int height) => 
    Results.Content(SvgGenerator.GenerateCirclesSvg(10,width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/circle/{width}/{height}", (int width, int height) => 
    Results.Content(SvgGenerator.GenerateCircleSvg(width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/circle/{radius}", (int radius) => 
    Results.Content(SvgGenerator.GenerateCircleSvg(radius),"image/svg+xml; charset=utf-8"));
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}