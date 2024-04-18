using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Capstone_Group2.Entities;
using Capstone_Group2.DataAccess;
using Capstone_Group2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<EmailService>();


string connectionString = builder.Configuration.GetConnectionString("Capstone_LocalDb");

builder.Services.AddDbContext<CapstoneDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<CapstoneDbContext>().AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    await CapstoneDbContext.CreateAdminUser(scope.ServiceProvider);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

app.Run();
