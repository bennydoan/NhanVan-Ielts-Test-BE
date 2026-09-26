using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NhanVanAPi.Data;
using NhanVanAPi.Models;
using NhanVanAPi.Services;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//register token service 
builder.Services.AddScoped<TokenService>();

//register the send email service
builder.Services.AddScoped<EmailService>(); // each https 


//register the database 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//register Identity — provides UserManager, RoleManager, and password/token handling
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Add cors ( cross origin resources sharing)

builder.Services.AddCors(options =>
{
    options.AddPolicy("CallFrontEnd", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


//JWT Configuration 
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // checks the token was actually issued by your API (matches Jwt:Issuer from config).
        ValidateAudience = true,//checks the token was actually issued by your API (matches Jwt:Issuer from config).
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,//rejects expired tokens.
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization(); //it enables the [Authorize] attribute you'll put on controllers/endpoints later




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("CallFrontEnd");   // order matter , must be after UseHttpsRedirection and before authentication 


app.UseAuthentication(); // this one comes first 
app.UseAuthorization();

app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}


app.Run();
