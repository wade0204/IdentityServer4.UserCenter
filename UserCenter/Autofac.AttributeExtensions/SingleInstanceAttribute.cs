namespace Autofac.AttributeExtensions
{
    public class SingleInstanceAttribute : RegistrationAttribute
    {
        protected SingleInstanceAttribute() : base(EnumLifeTimeScope.SingleInstance)
        {
        }
    }
}
