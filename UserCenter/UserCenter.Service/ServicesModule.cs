using Autofac;
using Autofac.AttributeExtensions;

namespace UserCenter.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class ServicesModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAttributedClasses(ThisAssembly);
        }
    }
}
