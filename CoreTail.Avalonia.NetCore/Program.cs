﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Avalonia;

namespace CoreTail.Avalonia.NetCore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
            if (args.Contains("--wait-for-attach"))
            {
                Console.WriteLine("Attach debugger and use 'Set next statement'");
                while (true)
                {
                    Thread.Sleep(100);
                    if (Debugger.IsAttached)
                        break;
                }
            }
            if (args.Contains("--fbdev"))
                AppBuilder.Configure<App>()
                    .InitializeWithLinuxFramebuffer(tl =>
            {
                tl.Content = App.InitializeAndGetMainWindow(args);
                ThreadPool.QueueUserWorkItem(_ => ConsoleSilencer());
            });
            else
            {
                BuildAvaloniaApp().SetupWithoutStarting()
                    .Instance.Run(App.InitializeAndGetMainWindow(args));
            }
        }

        /// <summary>
        /// This method is needed for IDE previewer infrastructure
        /// </summary>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>().UsePlatformDetect()
            //.UseReactiveUI() TODO: use?
            ;

        static void ConsoleSilencer()
        {
            Console.CursorVisible = false;
            while (true)
                Console.ReadKey(true);
        }
    }
}
