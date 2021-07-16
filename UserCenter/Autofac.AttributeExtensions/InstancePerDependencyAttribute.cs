namespace Autofac.AttributeExtensions
{
    public class InstancePerDependencyAttribute : RegistrationAttribute
    {
        protected InstancePerDependencyAttribute() : base(EnumLifeTimeScope.InstancePerDependency)
        {
        }
    }
}
