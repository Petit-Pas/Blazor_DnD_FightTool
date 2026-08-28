using DnDFightTool.Components.DndUi.Web.Hosting;
using DnDFightTool.Components.DndUi.Web.IoC;

var builder = WebApplication.CreateBuilder(args);

var dataFolder = Path.Combine(Path.GetTempPath(), "DnDFightTool.Web");
builder.Services.RegisterWebAppServices(dataFolder);

var app = builder.Build();

app.ConfigureWebAppPipeline();

app.Run();
