global using System.Diagnostics.CodeAnalysis;
global using System.Security.Claims;
global using System.Text.Json;

global using Microsoft.OpenApi;
global using Microsoft.IdentityModel.Tokens;

global using Microsoft.AspNetCore.OpenApi;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ApiExplorer;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;

global using Apta.Domain.Errors;
global using Apta.Application.Transport.Administrators;

global using Apta.WebApi.Authentication;
global using Apta.WebApi.Extensions;
global using Apta.WebApi.Transformers;
global using Apta.WebApi.Metadata;

global using Apta.Infrastructure.IoC.Configurations;
global using Apta.Infrastructure.IoC.Extensions;

global using Markdig;
global using Markdig.Syntax;

global using FluentValidation.AspNetCore;
global using Flurl;

global using Scalar.AspNetCore;
global using Dispattchr.Contracts;
global using HttpsRichardy.Internal.Essentials.Patterns;
