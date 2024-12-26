using Backend.Application.Validators;
using Backend.Core.Interfaces.Repositories;
using Backend.Core.Interfaces.Services;
using Backend.Infrastructure.Data.Requests;
using Backend.Infrastructure.Repositories;
using Backend.Infrastructure.UnitOfWork;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPartner, PartnerRepository>();
builder.Services.AddScoped<IPolicy, PolicyRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<PartnerRequest>, PartnerRequestValidator>();
builder.Services.AddScoped<IValidator<InsurancePolicyRequest>, InsurancePolicyRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
