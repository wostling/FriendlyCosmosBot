﻿using System;
using System.Threading.Tasks;

namespace AstroBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Not every host console supports setting window width
            try
            {
                // Set console width
                Console.WindowWidth = 190;
            }
            catch { }

            // Register a global exception handler
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            // Async wrapper of Main method
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Async main function serves as entry point of the app
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {
            NLog.LogManager.LoadConfiguration("nlog.config");

            NLog.LogManager.GetLogger(this.GetType().FullName).Info("---------------------------------------------------------");
            NLog.LogManager.GetLogger(this.GetType().FullName).Info("----------------- Launching Astro bot -------------------");
            NLog.LogManager.GetLogger(this.GetType().FullName).Info("---------------------------------------------------------");
            NLog.LogManager.GetLogger(this.GetType().FullName).Info("");
            NLog.LogManager.GetLogger(this.GetType().FullName).Info("NLog logging system loaded");

            // Initialize globals
            Globals.InitGlobals();

            new DiscordAstroBot().Initialize();

            // Wait indefinitely so the bot can run in the background
            await Task.Delay(-1);
        }

        /// <summary>
        /// Global exception handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            NLog.LogManager.GetLogger("AstroBot").Fatal($"Unhandled exception caught by global handler: {e.ExceptionObject.ToString()}");
        }
    }
}
