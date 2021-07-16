using System;

namespace Autofac.AttributeExtensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.GenericParameter, AllowMultiple = true)]
    public class RegistrationAttribute : Attribute
    {
        public EnumLifeTimeScope LifeTimeScope;
        protected RegistrationAttribute(EnumLifeTimeScope lifeTimeScope)
        {
            LifeTimeScope = lifeTimeScope;
        }

        public object[] LifetimeScopeTags { get; protected set; }

        public string Name { get; set; }

        public object Key { get; set; }

        public bool AsImplementedInterfaces { get; set; } = true;

        public Type[] As { get; set; }

        public static readonly object RequestLifetimeScopeTag = new object();
    }
}
