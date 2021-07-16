namespace Autofac.AttributeExtensions
{
    public class InstancePerLifetimeScopeAttribute : RegistrationAttribute
    {
        public InstancePerLifetimeScopeAttribute() : base(EnumLifeTimeScope.InstancePerLifetimeScope)
        {
        }

        public InstancePerLifetimeScopeAttribute(params object[] lifetimeScopeTags) : base(EnumLifeTimeScope.InstancePerLifetimeScope)
        {
            LifetimeScopeTags = lifetimeScopeTags;
        }
    }
}
