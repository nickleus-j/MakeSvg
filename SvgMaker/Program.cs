using SvgMaker.Lib;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Serves the generated JSON document
    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    }); // Serves the interactive UI
}
app.UseHttpsRedirection();
var svgGen = SvgGenerator.Instance;
app.MapGet("/", () => Results.Content(svgGen.GenerateSvgText("Hello There! try /random")
    ,"image/svg+xml; charset=utf-8"));
app.MapGet("/random", () => 
    Results.Content(svgGen.GenerateRandomSvg(330,250)
        ,"image/svg+xml; charset=utf-8"));
app.MapGet("/random/{width:int}/{height:int}", (int width, int height) => 
    Results.Content(svgGen.GenerateRandomSvg(width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/rectangle/{width:int}/{height:int}", (int width, int height) => 
    Results.Content(svgGen.GenerateRandomRectanglesSvg(10,width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/circles/{width:int}/{height:int}", (int width, int height) => 
    Results.Content(svgGen.GenerateCirclesSvg(10,width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/circle/{width:int}/{height:int}", (int width, int height) => 
    Results.Content(svgGen.GenerateCircleSvg(width,height),"image/svg+xml; charset=utf-8"));
app.MapGet("/circle/{radius:int}", (int radius) => 
    Results.Content(svgGen.GenerateCircleSvg(radius),"image/svg+xml; charset=utf-8"));
app.MapGet("/write/{text}", (string text) => 
    Results.Content(svgGen.GenerateSvgText(text),"image/svg+xml; charset=utf-8"));
app.MapGet("/qr/", (string q) => 
    Results.Content(svgGen.GetQrSvgOfUrl(q),"image/svg+xml; charset=utf-8"));
app.MapGet("/grid", () => 
    Results.Content(svgGen.GenerateRectangleSvg(300,300)
        ,"image/svg+xml; charset=utf-8"));
app.MapGet("/grid/{width:int}/{height:int}", (int width, int height) => 
    Results.Content(svgGen.GenerateRectangleSvg(width,height)
        ,"image/svg+xml; charset=utf-8"));
app.MapGet("/onion", () => 
    Results.Content(svgGen.GenerateOnionCirclesSvg(250)
        ,"image/svg+xml; charset=utf-8"));
app.MapGet("/polygon", () => 
    Results.Content(svgGen.GeneratePolygonsSvg(3,7,50)
        ,"image/svg+xml; charset=utf-8"));
app.MapGet("/polygon/{sideCount:int}", (int sideCount) => 
    Results.Content(svgGen.GeneratePolygonSvg(sideCount),"image/svg+xml; charset=utf-8"));
app.MapGet("/polygon/{sideCount:int}/{radius:int}", (int sideCount,int radius) => 
    Results.Content(svgGen.GeneratePolygonSvg(sideCount,radius),"image/svg+xml; charset=utf-8"));
app.MapGet("/polygons/{count:int}", (int count) =>
    Results.Content(svgGen.GeneratePolygonsSvg(count, 6, 50),"image/svg+xml; charset=utf-8"));
app.Run();
