using AuthAPI.Middlewares;
using AuthAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// database (DbContext with SQL Server)
builder.Services.AddDatabase(builder.Configuration);

// identity (users, roles, lockout settings)
builder.Services.AddIdentityServices();

// validation (FluentValidation + disable default ModelState validation)
builder.Services.AddValidation();

// auth services (AuthService, TokenGenerator, etc.)
builder.Services.AddAuthServices();

// controllers (API endpoints)
builder.Services.AddControllers();

// swagger (OpenAPI documentation)
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// global error handling middleware
app.UseErrorHandleMiddleware();

// enable swagger only in development
app.UseSwaggerIfDev();

// middleware pipeline
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();