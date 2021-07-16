namespace Autofac.AttributeExtensions
{
    public class InstancePerRequestAttribute : InstancePerLifetimeScopeAttribute
    {
        protected InstancePerRequestAttribute() : base(RequestLifetimeScopeTag)
        {
        }
    }
}
