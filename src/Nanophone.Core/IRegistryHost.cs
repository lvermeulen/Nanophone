namespace Nanophone.Core
{
    public interface IRegistryHost : IManageServiceInstances, IResolveServicesCatalog, IResolveServiceInstances, IHaveKeyValues
    { }
}
