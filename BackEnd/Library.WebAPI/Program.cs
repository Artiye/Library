using Api.Middlewares;
using Library.Application.DTOs.ResponseDTO;
using Library.Application.Encryption;
using Library.Application.Options;
using Library.Application.RepositoryInterfaces;
using Library.Application.Services;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using Library.Infrastructure.Data;
using Library.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("DefaultConnection not found")));

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>().AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoleManager<RoleManager<IdentityRole>>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.AddTransient<IEmailSenderService, EmailSenderService>();
builder.Services.AddTransient<IEmailSender, EmailSenderService>();


builder.Services.Configure<EmailOptions>(
builder.Configuration.GetSection(EmailOptions.EmailSender));

builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection("EncryptionSettings"));


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IBookClubRepository, BookClubRepository>();
builder.Services.AddScoped<IBookClubService, BookClubService>();

builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddScoped<IMemberService, MemberService>();    

builder.Services.AddScoped<IEncryptionService, EncryptionService>();

builder.Services.AddScoped<ResponseDTO>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapIdentityApi<ApplicationUser>();
app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<PreventDuplicateLoginMiddleware>();
app.UseMiddleware<RequestContextLoggingMiddleware>();
app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager,
    [FromBody] object empty) =>
{
    if (empty != null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.Unauthorized();
})
.WithOpenApi()
.RequireAuthorization();
app.MapControllers();


app.Run();
