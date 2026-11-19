global using System.Diagnostics.CodeAnalysis;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Microsoft.EntityFrameworkCore;

global using Apta.Domain.Repositories;
global using Apta.Domain.Services;
global using Apta.Application.Mapping;

global using Apta.Application.Handlers.Administrators;
global using Apta.Application.Transport.Administrators;
global using Apta.Application.Transport.Customers;
global using Apta.Application.Validators.Administrators;
global using Apta.Application.Validators.Customers;

global using Apta.Infrastructure.Services;
global using Apta.Infrastructure.Workers;
global using Apta.Infrastructure.Persistence;
global using Apta.Infrastructure.Persistence.Repositories;
global using Apta.Infrastructure.IoC.Configurations;

global using HttpsRichardy.Internal.Essentials.Contracts;
global using HttpsRichardy.Mapping;

global using FluentValidation;
global using Dispattchr.Extensions;
