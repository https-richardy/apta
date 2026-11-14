global using System.Net;
global using System.Net.Http.Json;
global using System.Text.Json;
global using System.Text.Encodings.Web;
global using System.Security.Claims;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;

global using Apta.Application.Handlers.Administrators;
global using Apta.Application.Handlers.Customers;
global using Apta.Application.Mapping;
global using Apta.Application.Transport;
global using Apta.Application.Transport.Administrators;
global using Apta.Application.Transport.Customers;
global using Apta.Application.Validators.Administrators;
global using Apta.Application.Validators.Customers;

global using Apta.Domain.Aggregates;
global using Apta.Domain.Errors;
global using Apta.Domain.Filtering;
global using Apta.Domain.Repositories;

global using Apta.Infrastructure.Persistence;
global using Apta.Infrastructure.Persistence.Repositories;

global using Apta.WebApi;
global using Apta.WebApi.Controllers;
global using Apta.Tests.Collections;
global using Apta.Tests.Fixtures;

global using HttpsRichardy.Internal.Essentials.Contracts;
global using HttpsRichardy.Internal.Essentials.Contracts.Behaviors;
global using HttpsRichardy.Internal.Essentials.Filtering;
global using HttpsRichardy.Internal.Essentials.Patterns;

global using AutoFixture;
global using HttpsRichardy.Mapping;
global using Moq;
global using Testcontainers.PostgreSql;
global using Dispattchr.Contracts;
