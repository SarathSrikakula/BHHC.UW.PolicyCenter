using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.API.V1.Mappings;
using FluentValidation;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection; // Ensure this is included
using Microsoft.OpenApi.Models; // Add this for Swagger
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen; // Add this for AddSwaggerGen extension method

using Microsoft.Data.SqlClient; // Add this for SQL Initialization

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => // Add configuration for Swagger

{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PolicyCenter API", Version = "v1" });
});

builder.Services.AddScoped<System.Data.IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new SqlConnection(connectionString);
});
// Configure Dapper Repository
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<IValidator<GetPolicyStatesQuery>, HC.UW.PolicyCenter.API.V1.Validations.GetPolicyStatesQueryValidator>();
builder.Services.AddScoped<IValidator<UpsertPolicyStateCommandRequest>, HC.UW.PolicyCenter.API.V1.Validations.UpsertPolicyStateCommandRequestValidator>();
builder.Services.AddScoped<IValidator<GetAvailableStatesQuery>, HC.UW.PolicyCenter.API.V1.Validations.GetAvailableStatesQueryValidator>();

// Configure MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
