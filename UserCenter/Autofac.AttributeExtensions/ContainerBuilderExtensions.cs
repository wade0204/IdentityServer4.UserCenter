using Autofac.Core.Lifetime;
using Autofac.Features.AttributeFilters;
using Autofac.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Autofac.AttributeExtensions
{
    using Registration = Builder.IRegistrationBuilder<object, Builder.ConcreteReflectionActivatorData, Builder.SingleRegistrationStyle>;
    public static class ContainerBuilderExtensions
    {
        public static void RegisterAttributedClasses(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            var attributedTypes = assemblies.SelectMany(a => a.GetLoadableTypes());

            foreach (var type in attributedTypes)
            {
                var registrationAttributes = type.GetTypeInfo().GetCustomAttributes<RegistrationAttribute>(false);

                foreach (var attribute in registrationAttributes)
                {
                    var registration = builder.RegisterType(type);

                    ConfigureParameters(registration, type);

                    SetLifeTimeScope(registration, attribute);

                    RegisterAs(registration, attribute, type);

                    ConfigureKeyFilter(registration, type);
                }
            }
        }
        private static void ConfigureParameters(Registration registration, Type type)
        {
            var attributedParameters = type.GetTypeInfo().GetConstructors()
                .SelectMany(c => c.GetParameters())
                .Select(p => new { info = p, attribute = p.GetCustomAttribute<ParameterRegistrationAttribute>() })
                .Where(p => p.attribute != null);

            foreach (var parameter in attributedParameters)
            {
                if (parameter.attribute.Named != null)
                    registration.WithParameter((p, c) => p == parameter.info, (p, c) => c.ResolveNamed(parameter.attribute.Named, parameter.info.ParameterType));
            }
        }

        private static void ConfigureKeyFilter(Registration registration, Type type)
        {
            var attributedParameters = type.GetTypeInfo().GetConstructors()
                .SelectMany(c => c.GetParameters())
                .Select(p => new { info = p, attribute = p.GetCustomAttribute<KeyFilterAttribute>() })
                .Where(p => p.attribute != null);
            foreach (var attributedParameter in attributedParameters)
            {
                if (attributedParameter.attribute.Key != null)
                {
                    registration.WithAttributeFiltering();
                }
            }
        }

        private static void SetLifeTimeScope(Registration registration, RegistrationAttribute attribute)
        {
            switch (attribute.LifeTimeScope)
            {
                case EnumLifeTimeScope.InstancePerDependency:
                    registration.InstancePerDependency();
                    break;
                case EnumLifeTimeScope.InstancePerLifetimeScope:
                    if (attribute.LifetimeScopeTags?.Any() ?? false)
                        registration.InstancePerMatchingLifetimeScope(FixedScopeTags(attribute).ToArray());
                    else
                        registration.InstancePerLifetimeScope();
                    break;
                case EnumLifeTimeScope.SingleInstance:
                    registration.SingleInstance();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IEnumerable<object> FixedScopeTags(RegistrationAttribute attribute)
        {
            // Turns RequestLifetimeScopeTag into Autofac's version
            return attribute.LifetimeScopeTags.Select(t => t == RegistrationAttribute.RequestLifetimeScopeTag ? MatchingScopeLifetimeTags.RequestLifetimeScopeTag : t);
        }

        private static void RegisterAs(Registration registration, RegistrationAttribute attribute, Type type)
        {
            if (RegisterOnlyAsNamed(attribute))
            {
                foreach (var asType in RegisterAsTypes(attribute, type))
                    RegisterNamed(registration, attribute, asType);
                if (attribute.As == null)
                    registration.AsSelf();
            }
            else
            {
                foreach (var asType in RegisterAsTypes(attribute, type))
                {
                    RegisterNamed(registration, attribute, asType);
                    RegisterKeyed(registration, attribute, asType);
                    registration.As(asType);
                }
            }
        }
        private static bool RegisterOnlyAsNamed(RegistrationAttribute attribute)
        {
            return attribute.Name != null && attribute.Key == null;
        }

        private static IEnumerable<Type> RegisterAsTypes(RegistrationAttribute attribute, Type type)
        {
            return attribute.As
                   ?? (attribute.AsImplementedInterfaces
                       ? type.GetTypeInfo().GetInterfaces().Concat(new[] { type })
                       : new[] { type });
        }

        private static void RegisterNamed(Registration registration, RegistrationAttribute attribute, Type asType)
        {
            if (attribute.Name != null)
                registration.Named(attribute.Name, asType);
        }

        private static void RegisterKeyed(Registration registration, RegistrationAttribute attribute, Type asType)
        {
            if (attribute.Key != null)
                registration.Keyed(attribute.Key, asType);
        }
    }
}
