global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging.Abstractions;

global using Apta.Application.Handlers.Administrators;
global using Apta.Application.Mapping;
global using Apta.Application.Transport;
global using Apta.Application.Transport.Administrators;
global using Apta.Application.Validators.Administrators;

global using Apta.Domain.Aggregates;
global using Apta.Domain.Errors;
global using Apta.Domain.Filtering;
global using Apta.Domain.Repositories;

global using Apta.Infrastructure.Persistence;
global using Apta.Infrastructure.Persistence.Repositories;

global using Apta.Tests.Collections;
global using Apta.Tests.Fixtures;

global using HttpsRichardy.Internal.Essentials.Contracts;
global using HttpsRichardy.Internal.Essentials.Contracts.Behaviors;
global using HttpsRichardy.Internal.Essentials.Filtering;

global using AutoFixture;
global using HttpsRichardy.Mapping;
global using Moq;
global using Testcontainers.PostgreSql;
