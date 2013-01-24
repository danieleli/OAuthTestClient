using System;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Http;
using Microsoft.Practices.Unity;
using System.Collections.Generic;

namespace PPS.API.Common.Helpers
{
	public static class RepositoryResolver
	{
		private static bool UseMockRepositories
		{
			get
			{
				bool setting = Convert.ToBoolean(WebConfigurationManager.AppSettings[Constants.Settings.UseMockRepositories] ?? (object)false);
				return setting;
			}
		}

        private static List<Assembly> RepositoryAssemblies
        {
            get
            {
                return _repositoryAssemblies.Value;
            }
        }
        static Lazy<List<Assembly>> _repositoryAssemblies = new Lazy<List<Assembly>>(InitializeRepositoryAssemblies);

        private static List<Assembly> InitializeRepositoryAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("Repositories")).ToList();
        }

		public static void ResolveRepositories(IUnityContainer container)
		{
            foreach (var assembly in RepositoryAssemblies)
				ResolveRepositories(container, assembly);

			ResolveRepositories(container, Assembly.GetExecutingAssembly());
		}

		private static void ResolveRepositories(IUnityContainer container, Assembly assembly)
		{
            var repositoryTypes = RepositoryAssemblies.SelectMany(s => s.GetTypes()).Where(r => r.FullName.Contains("Repository"));

            foreach (var repositoryType in repositoryTypes)
            {
                if (UseMockRepositories && repositoryType.FullName.Contains("Mock"))
                {
                    ExtractInterfacesAndRegister(container, repositoryType);
                }
                else if (!UseMockRepositories && repositoryType.FullName.Contains("SQL"))
                {
                    ExtractInterfacesAndRegister(container, repositoryType);
                }
            }
		}

        private static void ExtractInterfacesAndRegister(IUnityContainer container, Type repositoryType)
        {
            foreach (var repositoryInterface in repositoryType.GetInterfaces().Where(t => t.FullName.Contains("Repository")))
            {
                container.RegisterType(repositoryInterface, repositoryType);

                //if (container.Registrations.Where(t => t.RegisteredType == repositoryInterface.GetType()).Count() == 0)
                //{
                //    Logger.Debug(typeof(RepositoryResolver), "Registering type {0} - {1}", repositoryInterface.FullName, repositoryType.FullName);

                //    container.RegisterType(repositoryInterface, repositoryType);
                //}
            }
        }
	}
}