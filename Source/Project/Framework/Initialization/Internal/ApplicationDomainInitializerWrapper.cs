using EPiServer.Framework.Initialization;
using EPiServer.Framework.Initialization.Internal;
using RegionOrebroLan.Abstractions;

namespace RegionOrebroLan.EPiServer.Framework.Initialization.Internal
{
	public class ApplicationDomainInitializerWrapper : Wrapper<ApplicationDomainInitializer>, IApplicationDomainInitializer
	{
		#region Constructors

		public ApplicationDomainInitializerWrapper(ApplicationDomainInitializer applicationDomainInitializer) : base(applicationDomainInitializer, nameof(applicationDomainInitializer)) { }

		#endregion

		#region Methods

		public virtual void SetupApplicationDomain(HostType hostType)
		{
			this.WrappedInstance.SetupAppDomain(hostType);
		}

		#endregion
	}
}