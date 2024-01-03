using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Flavorique_Web_App;
using Flavorique_Web_App.Data;
using Flavorique_Web_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TXTextControl;

var CORSPolicy = "_CORSPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add URLS to CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CORSPolicy,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7122");
                      });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
});

// Connect Database and DbContext.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Sets Default Identity Model.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add Razor View Services.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Add Mailer Services.
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add Razor View to String Renderer.
builder.Services.AddTransient<RazorViewToStringRenderer>();

// Add HTML To PDF Converter.
builder.Services.AddTransient<HTMLToPDFConverter>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure CkBox JWT
app.MapGet("/api/CKBox/token", () =>
{

    var environmentId = builder.Configuration.GetValue<string>("CKBoxEnvironmentId");
    var accessKey = builder.Configuration.GetValue<string>("CKBoxAccessKey");
    var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(accessKey));

    var signingCredentials = new SigningCredentials(securityKey, "HS256");
    var header = new JwtHeader(signingCredentials);

    var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);

    var payload = new JwtPayload
    {
        { "aud", environmentId },
        { "iat", dateTimeOffset.ToUnixTimeSeconds() },
        { "sub", "user-123" },
        { "user", new Dictionary<string, string> {
            { "email", "joe.doe@example.com" },
            { "name", "Joe Doe" }
        } },
        { "auth", new Dictionary<string, object> {
            { "ckbox", new Dictionary<string, string> {
                { "role", "admin" }
            } }
        } }
    };

    var securityToken = new JwtSecurityToken(header, payload);
    var handler = new JwtSecurityTokenHandler();

    return handler.WriteToken(securityToken);
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(CORSPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
