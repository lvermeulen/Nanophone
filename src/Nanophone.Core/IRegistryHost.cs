namespace Nanophone.Core
{
    public interface IRegistryHost : IManageServiceInstances, IManageHealthChecks, IResolveServiceInstances, IHaveKeyValues
    { }
}
