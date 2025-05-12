using Neinei.Services;

var builder = WebApplication.CreateBuilder();
builder.Services.AddControllers();
builder.Services.AddScoped<IVisitService, VisitService>();
var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();