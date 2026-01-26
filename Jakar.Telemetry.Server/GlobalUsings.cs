// Jakar.Extensions :: Jakar.Extensions.Telemetry.Server
// 06/25/2024  11:06

global using System.Collections.Frozen;
global using System.Collections.Immutable;
global using System.Data;
global using System.Data.Common;
global using System.Diagnostics;
global using System.Net.Http;
global using System.Net.Http.Json;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Text.Json.Serialization.Metadata;
global using Dapper.Contrib.Extensions;
global using Jakar.Database;
global using Jakar.Telemetry.DbMigrations;
global using Jakar.Extensions.Blazor;
global using Jakar.Telemetry.Server;
global using Jakar.Telemetry.Server.Components;
global using Jakar.Telemetry.Server.Components.Pages;
global using Jakar.Telemetry.Server.Components.Layout;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.Routing;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Components.Web.Virtualization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.Extensions.Configuration;
global using Microsoft.JSInterop;
global using Npgsql;
global using Syncfusion.Blazor;
global using static Microsoft.AspNetCore.Components.Web.RenderMode;
   