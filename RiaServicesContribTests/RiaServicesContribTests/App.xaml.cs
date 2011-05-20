using System;
using System.Windows;
using Microsoft.Silverlight.Testing;
using System.Reflection;
using System.Windows.Resources;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace RiaServicesContribTests
{
    public partial class App : Application
    {

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        public static IEnumerable<Assembly> CurrentAssemblies
        {
            get
            {
                var assemblies = new List<Assembly>();

                // While this may seem like somewhat of a hack, walking the AssemblyParts in the active 
                // deployment object is the only way to get the list of assemblies loaded by the initial XAP. 
                foreach(AssemblyPart ap in Deployment.Current.Parts)
                {
                    StreamResourceInfo sri = Application.GetResourceStream(new Uri(ap.Source, UriKind.Relative));
                    if(sri != null)
                    {
                        // Keep in mind that calling Load on an assembly that is already loaded will
                        // be a no-op and simply return the already loaded assembly object.
                        Assembly assembly = ap.Load(sri.Stream);
                        assemblies.Add(assembly);
                    }
                }

                return assemblies;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var settings = UnitTestSystem.CreateDefaultSettings();
            settings.TestAssemblies.Clear();

            Func<Type, bool> IsTestClass = type => type.IsDefined(typeof(TestClassAttribute), true);
            var testAssemblies = CurrentAssemblies.Where(ass => ass.GetTypes().Any(IsTestClass));

            // Add all assemblies containing test classes to the unit test system.
            foreach(var assembly in testAssemblies)
            {
                settings.TestAssemblies.Add(assembly);
            }
            RootVisual = UnitTestSystem.CreateTestPage(settings);
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if(!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch(Exception)
            {
            }
        }
    }
}