// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.MongoDB.Entities;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace IdentityServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            const string connectionString = @"mongodb://localhost:27017";
            
            var builder = services.AddIdentityServer()
                .AddTestUsers(TestUsers.Users)
                .AddConfigurationStore(options =>
                {
                    options.Connect("identityServer", connectionString);
                })
                .AddOperationalStore(options =>
                {
                    options.Connect("identityServer", connectionString);
                });

            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "<insert here>";
                    options.ClientSecret = "<insert here>";
                })
                .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "interactive.confidential";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // this will do the initial DB population
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var clientsCollection = serviceScope.ServiceProvider.GetRequiredService<IMongoCollection<ClientEntity>>();
            if (!clientsCollection.AsQueryable().Any())
            {
                clientsCollection.InsertMany(Config.Clients
                    .Select(client => client.ToEntity()));
            }

            var identityResourceCollection = serviceScope.ServiceProvider.GetRequiredService<IMongoCollection<IdentityResourceEntity>>();
            if (!identityResourceCollection.AsQueryable().Any())
            {
                identityResourceCollection.InsertMany(Config.IdentityResources
                    .Select(resource => resource.ToEntity()));
            }

            var apiScopeCollection = serviceScope.ServiceProvider.GetRequiredService<IMongoCollection<ApiScopeEntity>>();
            if (!apiScopeCollection.AsQueryable().Any())
            {
                apiScopeCollection.InsertMany(Config.ApiScopes
                    .Select(resource => resource.ToEntity()));
            }
        }
    }
}
