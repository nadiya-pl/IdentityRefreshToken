using ProtectedAPI.Extensions;
using ProtectedAPI.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// add services to the container
builder.Services.AddControllers();

// jwt authentication (custom extension)
builder.Services.AddJwtAuthentication(builder.Configuration);

// authorization
builder.Services.AddAuthorization();

// swagger
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

// global error handling middleware
app.UseErrorHandleMiddleware();

// enable swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// middleware pipeline
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();