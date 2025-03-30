using MoonGridBuilder.Core.Interfaces;
using MudBlazor.Services;
using MoonMud.DemoApp.Components;
using MoonMud.DemoApp.Models;
using MoonMud.DemoApp.Service;
using FluentValidation;
using FluentValidation.AspNetCore;
using MoonMud.DemoApp.Validators;

namespace MoonMud.DemoApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddScoped<AssetDataGridService>();

        builder.Services.AddScoped<IDataGridService<AssetReadDto>, AssetDataGridService>();

        builder.Services.AddValidatorsFromAssemblyContaining<AssetCreateDtoValidator>();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();

        // Add MudBlazor services
        builder.Services.AddMudServices();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}