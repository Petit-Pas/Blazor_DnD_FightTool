using DnDFightTool.Components.DndUi.Web.Hosting;
using DnDFightTool.Components.DndUi.Web.IoC;

var builder = WebApplication.CreateBuilder(args);

// Path.GetTempPath() is wiped on reboot on Linux (tmpfs/systemd-tmpfiles); use a persistent, OS-correct folder instead.
var dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DnDFightTool.Web");
builder.Services.RegisterWebAppServices(dataFolder);

var app = builder.Build();

app.ConfigureWebAppPipeline();

app.Run();
