// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.9.2

using Bot.Builder.Community.Adapters.Crunch;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicData;
using MusicMasterBot.Bots;
using MusicMasterBot.Dialogs;
using MusicMasterBot.TextToSpeech;
using System.Linq;
using UserCommandLogic;
using Microsoft.Extensions.Configuration;
using Tools;

namespace MusicMasterBot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Add Alexa Adapter with error handler
            services.AddSingleton<CrunchAdapter, CrunchAdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // Register LUIS recognizer
            services.AddSingleton<UserCommandRecognizer>();

            // Register the BookingDialog.
            services.AddSingleton<RequestSongDialog>();

            // The MainDialog that will be run by the bot.
            services.AddSingleton<MainDialog>();

            var databaseConnector = new DatabaseConnector();

            services.AddSingleton<ISongChooser>(new SongChooser(databaseConnector));
            services.AddSingleton(databaseConnector);
            services.AddSingleton<IPlayer, MusicPlayer>();

            // Create the Voice Middleware that will be added to the middleware pipeline in the AdapterWithErrorHandler
            services.AddSingleton<VoiceMiddleware>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, MusicAndWelcomeBot<MainDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration, DatabaseConnector databaseConnector, ISongChooser songChooser)
        {
            databaseConnector.Setup(configuration["SongDatabase:Server"], configuration["SongDatabase:Database"],
                configuration["SongDatabase:User"], configuration["SongDatabase:Password"]);
            songChooser.SetDatabaseConnection(databaseConnector);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
