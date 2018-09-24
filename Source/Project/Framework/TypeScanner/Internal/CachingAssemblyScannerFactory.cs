using System;
using System.Web.Hosting;
using EPiServer.Framework;
using EPiServer.Framework.TypeScanner.Internal;

namespace RegionOrebroLan.EPiServer.Framework.TypeScanner.Internal
{
	public class CachingAssemblyScannerFactory : IAssemblyScannerFactory
	{
		#region Methods

		public virtual IAssemblyScanner Create()
		{
			// ReSharper disable All

			var cachingAssemblyScannerType = Type.GetType(typeof(FrameworkAspNetInitialization).AssemblyQualifiedName.Replace("FrameworkAspNetInitialization", "TypeScanner.Internal.CachingAssemblyScanner"), true);

			var assemblyScanner = (IAssemblyScanner) Activator.CreateInstance(cachingAssemblyScannerType, new ReflectionAssemblyScanner());

			cachingAssemblyScannerType.GetProperty("EnableBuildManagerCache").SetValue(assemblyScanner, HostingEnvironment.IsHosted);

			// ReSharper restore All

			return assemblyScanner;
		}

		#endregion
	}
}