
using AutoMapper;
using Domain.ExternalInterfaces;
using Domain.Interfaces.Generics;
using Domain.Interfaces.Interfaces;
using Domain.Interfaces.InterfaceServices;
using Domain.Services;
using Entities.Entities;
using Infrastructure.Configuration;
using Infrastructure.Repository.ExternalRepositories;
using Infrastructure.Repository.Generics;
using Infrastructure.Repository.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Token;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GestaoClientes.WebAPI",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Rediclife Carneiro Caldas",
            Email = "rediclife@gmail.com",
            Url = new Uri("https://linkedin.com/in/rediclife/")
        }
    });

    var xmlFile = "WebAPI.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    s.IncludeXmlComments(xmlPath);
});

// ConfigServices
builder.Services.AddDbContext<ContextBase>(options =>
              options.UseSqlServer(
                  builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ContextBase>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// INTERFACE E REPOSITORIO
builder.Services.AddSingleton(typeof(IGeneric<>), typeof(RepositoyGenerics<>));
builder.Services.AddSingleton<ICliente, RepositoryCliente>();
builder.Services.AddSingleton<IEnderecoCepExt, RepositoryEnderecoCepExt>();
builder.Services.AddSingleton<IEndereco, RepositoryEndereco>();
builder.Services.AddSingleton<IContato, RepositoryContato>();

// SERVIÇO DOMINIO
builder.Services.AddSingleton<IServiceCliente, ServiceCliente>();


// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(option =>
      {
          option.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = false,
              ValidateAudience = false,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,

              ValidIssuer = "Teste.Security.Bearer",
              ValidAudience = "Teste.Security.Bearer",
              IssuerSigningKey = JwtSecurityKey.Create("Secret_Key-Gestao_Clientes-12345678")
          };

          option.Events = new JwtBearerEvents
          {
              OnAuthenticationFailed = context =>
              {
                  Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                  return Task.CompletedTask;
              },
              OnTokenValidated = context =>
              {
                  Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                  return Task.CompletedTask;
              }
          };
      });


var config = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.CreateMap<ClienteDTO, Cliente>();
    cfg.CreateMap<Cliente, ClienteDTO>();
    cfg.CreateMap<EnderecoDTO, Endereco>();
    cfg.CreateMap<Endereco, EnderecoDTO>();
    cfg.CreateMap<ContatoDTO, Contato>();
    cfg.CreateMap<Contato, ContatoDTO>();
});

IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//var devClient = "http://localhost:4200";
var devClient = "http://localhost:3000";
app.UseCors(x => x
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader().WithOrigins(devClient));


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwaggerUI();

app.Run();

