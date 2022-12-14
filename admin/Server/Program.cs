using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Application.Server.Data;
using Microsoft.AspNetCore.Identity;
using Application.Model.Org;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Application.Model;

var builder = WebApplication.CreateBuilder(args);

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// get allowed origins from appsettings.json

var cors = new Cors();
var section = builder.Configuration.GetSection("Cors");
cors = section.Get<Cors>();


// Add services to the container.
var applicationConnectionString = builder.Configuration.GetConnectionString("ApplicationConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(applicationConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>
    (options => {
        options.IdentityResources["openid"].UserClaims.Add("role");
        options.ApiResources.Single().UserClaims.Add("role");
        // options.IdentityResources["profile"].UserClaims.Add("firstname");
        // options.IdentityResources["profile"].UserClaims.Add("lastname");
    });
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");


builder.Services.Configure<IdentityOptions>(options => 
    options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);



builder.Services.AddAuthentication()
    .AddIdentityServerJwt();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins(cors.Origins);
                                // .AllowAnyHeader()
                                // .AllowAnyMethod();
                      });
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
