using EPiServer.Framework.Initialization;

namespace RegionOrebroLan.EPiServer.Framework.Initialization.Internal
{
	public interface IApplicationDomainInitializer
	{
		#region Methods

		void SetupApplicationDomain(HostType hostType);

		#endregion
	}
}