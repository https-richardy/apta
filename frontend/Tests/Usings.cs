global using System.Security.Claims;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.FluentUI.AspNetCore.Components;

global using Apta.WebUI.Transport;
global using Apta.WebUI.Transport.Administrators;
global using Apta.WebUI.Transport.Authentication;
global using Apta.WebUI.Pages.Shared;
global using Apta.WebUI.Pages.Authentication.Components;
global using Apta.WebUI.Pages.Administrators.Components.Content;
global using Apta.WebUI.Pages.Administrators.Components.Forms;
global using Apta.WebUI.Pages.Administrators.Components.Tables;
global using Apta.WebUI.Clients.Interfaces;
global using Apta.WebUI.Services;
global using Apta.WebUI.Authentication;

global using Bunit;
global using Bunit.TestDoubles;
global using AutoFixture;
global using Moq;
global using HttpsRichardy.Internal.Essentials.Patterns;

global using Blazored.LocalStorage;
global using DotNetQuery.Core;
global using DotNetQuery.Extensions.DependencyInjection;

global using WireMock.Server;
global using WireMock.RequestBuilders;
global using WireMock.ResponseBuilders;
