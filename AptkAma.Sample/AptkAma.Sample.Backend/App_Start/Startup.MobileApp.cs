using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;
using AptkAma.Sample.Backend.Models;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Owin;

namespace AptkAma.Sample.Backend
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("CustomLogin", ".auth/login/CustomLogin", new { controller = "CustomLogin" });

            //For more information on Web API tracing, see http://go.microsoft.com/fwlink/?LinkId=620686 
            config.EnableSystemDiagnosticsTracing();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            var migrator = new DbMigrator(new Migrations.Configuration());
            string errorMessage = null;
            try
            {
                migrator.Update();
            }
            catch (DbEntityValidationException ex)
            {
                errorMessage = ex.EntityValidationErrors.SelectMany(entityValidationError => entityValidationError.ValidationErrors).Aggregate(errorMessage, (current, dbValidationError) => current + (dbValidationError.PropertyName + ": " + dbValidationError.ErrorMessage + " "));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            if (errorMessage != null) throw new Exception(errorMessage);

            var settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                // This middleware is intended to be used locally for debugging. By default, HostName will
                // only have a value when running in an App Service application.
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }
            app.UseWebApi(config);
        }
    }

    public class AptkAmaInitializer : CreateDatabaseIfNotExists<AptkAmaContext>
    {
        protected override void Seed(AptkAmaContext context)
        {
            //List<TodoItem> todoItems = new List<TodoItem>
            //{
            //    new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
            //    new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
            //};

            //foreach (TodoItem todoItem in todoItems)
            //{
            //    context.Set<TodoItem>().Add(todoItem);
            //}

            //base.Seed(context);
        }
    }
}

