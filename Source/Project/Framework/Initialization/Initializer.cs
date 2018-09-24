using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using EPiServer.Framework.Initialization;
using RegionOrebroLan.EPiServer.Framework.Initialization.Internal;
using RegionOrebroLan.EPiServer.Framework.TypeScanner.Internal;
using RegionOrebroLan.EPiServer.ServiceLocation.AutoDiscovery;

namespace RegionOrebroLan.EPiServer.Framework.Initialization
{
	public class Initializer : IInitializer
	{
		#region Fields

		private static readonly object _lock = new object();

		#endregion

		#region Constructors

		public Initializer(IApplicationDomainInitializer applicationDomainInitializer, HostType hostType, IInitializationEngineFactory initializationEngineFactory)
		{
			this.ApplicationDomainInitializer = applicationDomainInitializer ?? throw new ArgumentNullException(nameof(applicationDomainInitializer));
			this.HostType = hostType;
			this.InitializationEngineFactory = initializationEngineFactory ?? throw new ArgumentNullException(nameof(initializationEngineFactory));
		}

		#endregion

		#region Properties

		protected internal virtual IApplicationDomainInitializer ApplicationDomainInitializer { get; }
		protected internal virtual HostType HostType { get; }
		protected internal virtual IInitializationEngineFactory InitializationEngineFactory { get; }

		#endregion

		#region Methods

		protected internal virtual void DisableOriginalEngine(DisabledInitializationEngine disabledInitializationEngine)
		{
			// ReSharper disable PossibleNullReferenceException
			typeof(InitializationModule).GetField("_engine", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, disabledInitializationEngine);
			// ReSharper restore PossibleNullReferenceException
		}

		public static void Initialize(HostType hostType)
		{
			lock(_lock)
			{
				var applicationDomainInitializer = new ApplicationDomainInitializerWrapper(global::EPiServer.Framework.Initialization.Internal.ApplicationDomainInitializer.Instance);
				var assemblies = InitializationModule.Assemblies.AllowedAssemblies.ToArray();
				var assemblyScanner = new CachingAssemblyScannerFactory().Create();
				var serviceLocatorFactory = new ServiceLocatorFactoryResolver().Resolve(assemblies);

				var initializationEngineFactory = new InitializationEngineFactory(assemblies, assemblyScanner, serviceLocatorFactory);

				var initializer = new Initializer(applicationDomainInitializer, hostType, initializationEngineFactory);

				initializer.Initialize();
			}
		}

		public virtual void Initialize()
		{
			this.ApplicationDomainInitializer.SetupApplicationDomain(this.HostType);

			var initializationEngine = this.InitializationEngineFactory.Create(this.HostType);

			var disabledInitializationEngine = (initializationEngine as IInitializationEngineReplacement)?.OriginalInitializationEngine;

			if(disabledInitializationEngine == null)
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The initialization-engine must be of type \"{0}\".", typeof(IInitializationEngineReplacement)));

			this.DisableOriginalEngine(disabledInitializationEngine);

			initializationEngine.Initialize();
		}

		#endregion
	}
}