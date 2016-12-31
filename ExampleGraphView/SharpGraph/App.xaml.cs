﻿using System.Windows;

namespace ExampleGraphView {
    public partial class App {
        private void Application_Startup(object sender, StartupEventArgs e) {
            string initialFile = null;
            if (e.Args.Length > 0) {
                initialFile = e.Args[0];
            }
            var mainWindow = new MainWindow(initialFile);
            MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}