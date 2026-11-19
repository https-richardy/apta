global using System.Globalization;
global using System.Linq.Expressions;
global using System.Diagnostics.CodeAnalysis;

global using Microsoft.Extensions.Logging;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;

global using Apta.Domain.Aggregates;
global using Apta.Domain.Repositories;
global using Apta.Domain.Services;
global using Apta.Domain.Filtering;

global using Apta.Application.Transport.Customers;

global using Apta.Infrastructure.Persistence.Extensions;
global using Apta.Infrastructure.Persistence.Repositories.Pipelines;

global using HttpsRichardy.Internal.Essentials.Filtering;
global using HttpsRichardy.Internal.Essentials.Aggregates;
global using HttpsRichardy.Internal.Essentials.Utilities;
global using HttpsRichardy.Internal.Essentials.Patterns;
global using HttpsRichardy.Internal.Essentials.Extensions;

global using HttpsRichardy.Internal.Essentials.Contracts;
global using HttpsRichardy.Internal.Essentials.Contracts.Behaviors;

global using Dispattchr.Contracts;

global using CsvHelper;
global using CsvHelper.Configuration;
