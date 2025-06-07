using Corretora.Bussiness.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// O certo seria colocar em um vault key, mas como isso é só uma atividade, vamos deixar assim.
var sqlServerConnectionString = "data source=.\\SQLEXPRESS;initial catalog=corretora;trusted_connection=true;TrustServerCertificate=True";

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CorretoraContext>(options => options.UseSqlServer(sqlServerConnectionString), ServiceLifetime.Scoped);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
