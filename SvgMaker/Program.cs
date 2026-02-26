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
var svgGen = SvgGenerator.Instance;
app.MapGet("/", () => "Hello There!");
app.MapGet("/random", () => 
    Results.Content(svgGen.GenerateRandomSvg(330,250)
        ,"image/svg+xml; charset=utf-8"));
app.MapGet("/random/{width}/{height}", (int width, int height) => 
    Results.Content(svgGen.GenerateRandomSvg(width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/rectangle/{width}/{height}", (int width, int height) => 
    Results.Content(svgGen.GenerateRandomRectanglesSvg(10,width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/circles/{width}/{height}", (int width, int height) => 
    Results.Content(svgGen.GenerateCirclesSvg(10,width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/circle/{width}/{height}", (int width, int height) => 
    Results.Content(svgGen.GenerateCircleSvg(width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/circle/{radius}", (int radius) => 
    Results.Content(svgGen.GenerateCircleSvg(radius),"image/svg+xml; charset=utf-8"));
app.MapGet("/read/{text}", (string text) => 
    Results.Content(svgGen.GenerateSvgText(text),"image/svg+xml; charset=utf-8"));
app.Run();
