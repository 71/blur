using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Blur
{
    partial class BlurExtensions
    {
        /// <summary>
        /// Returns whether or not the given <see cref="IMetadataTokenProvider"/>
        /// was created during runtime, and has not yet been compiled.
        /// </summary>
        public static bool IsNew(this IMetadataTokenProvider tokenProvider)
            => tokenProvider.MetadataToken.RID == 0;

#region Mono.Cecil => System.Reflection
        /// <summary>
        /// Load the <see cref="Type"/> associated with the given
        /// <see cref="TypeDefinition"/>.
        /// </summary>
        public static Type AsType(this TypeReference type)
        {
            //if (type.IsNew())
            //    throw new ArgumentException("The given type cannot be dynamically created.", nameof(type));

            // Check if type is nested.
            if (type.DeclaringType != null)
                return type.AsTypeInfo().AsType();

            // Simple try
            Type result = Type.GetType(type.FullName);

            if (result != null)
                return result;

            // Try using its module
            foreach (Assembly assembly in Processor.GetAssemblies())
            {
                result = assembly.GetType(type.FullName);

                if (result != null)
                    return result;
            }

            throw new Exception($"Cannot resolve type {type.FullName}.");
        }

        /// <summary>
        /// Load the <see cref="TypeInfo"/> associated with the given
        /// <see cref="TypeDefinition"/>.
        /// </summary>
        public static TypeInfo AsTypeInfo(this TypeReference type)
        {
            if (type.DeclaringType != null)
                return type.DeclaringType.Resolve().AsTypeInfo().GetDeclaredNestedType(type.Name);

            return type.AsType()?.GetTypeInfo();
        }

        /// <summary>
        /// Load the <see cref="MethodInfo"/> associated with the given
        /// <see cref="MethodDefinition"/>.
        /// </summary>
        public static MethodBase AsInfo(this MethodDefinition method)
        {
            var parameters = method.Parameters;

            if (method.IsConstructor)
            {
                bool isStatic = method.IsStatic;

                return (from mi in method.DeclaringType.AsTypeInfo().DeclaredConstructors
                        where mi.IsStatic == isStatic
                        let miParameters = mi.GetParameters()
                        where miParameters.Length == parameters.Count
                           && miParameters.Select(x => x.ParameterType.FullName)
                                          .SequenceEqual(parameters.Select(x => x.ParameterType.FullName))
                        select mi).FirstOrDefault();
            }

            return (from mi in method.DeclaringType.AsTypeInfo().DeclaredMethods
                    where mi.Name == method.Name
                    let   miParameters = mi.GetParameters()
                    where miParameters.Length == parameters.Count
                       && miParameters.Select(x => x.ParameterType.Name)
                                      .SequenceEqual(parameters.Select(x => x.ParameterType.Name))
                    select mi).FirstOrDefault();
        }

        /// <summary>
        /// Load the <see cref="PropertyInfo"/> associated with the given
        /// <see cref="PropertyDefinition"/>.
        /// </summary>
        public static PropertyInfo AsInfo(this PropertyDefinition property)
        {
            return property.DeclaringType.AsTypeInfo().DeclaredProperties
                .FirstOrDefault(x => x.Name == property.Name);
        }

        /// <summary>
        /// Load the <see cref="FieldInfo"/> associated with the given
        /// <see cref="FieldDefinition"/>.
        /// </summary>
        public static FieldInfo AsInfo(this FieldDefinition field)
        {
            return field.DeclaringType.AsTypeInfo().DeclaredFields
                .FirstOrDefault(x => x.Name == field.Name);
        }

        /// <summary>
        /// Load the <see cref="EventInfo"/> associated with the given
        /// <see cref="EventDefinition"/>.
        /// </summary>
        public static EventInfo AsInfo(this EventDefinition ev)
        {
            return ev.DeclaringType.AsTypeInfo().DeclaredEvents
                .FirstOrDefault(x => x.Name == ev.Name);
        }

        /// <summary>
        /// Load the <see cref="ParameterInfo"/> associated with the given
        /// <see cref="ParameterDefinition"/>.
        /// </summary>
        public static ParameterInfo AsInfo(this ParameterDefinition parameter)
        {
            if (parameter.Method is MethodDefinition)
                return ((MethodDefinition)parameter.Method).AsInfo().GetParameters()
                    .FirstOrDefault(x => x.Name == parameter.Name);

            throw new NotSupportedException();
        }
#endregion

#region System.Reflection => Mono.Cecil
        /// <summary>
        /// Load the <see cref="TypeDefinition"/> associated with the given
        /// <see cref="TypeInfo"/>.
        /// </summary>
        public static TypeDefinition GetDefinition(this TypeInfo type)
        {
            if (type.DeclaringType != null)
                return type.DeclaringType.GetDefinition().NestedTypes.First(x => x.Name == type.Name);

            TypeReference reference;
            if (Processor.TargetModuleDefinition.TryGetTypeReference(type.FullName, out reference))
                return Processor.TargetModuleDefinition.ImportReference(reference).Resolve();
            if ((reference = Processor.TargetModuleDefinition.GetType(type.FullName)) != null)
                return (TypeDefinition)reference;

            throw new Exception($"Cannot get definition for {type.FullName}");
        }

        /// <summary>
        /// Load the <see cref="TypeDefinition"/> associated with the given
        /// <see cref="Type"/>.
        /// </summary>
        public static TypeDefinition GetDefinition(this Type type)
        {
            if (type.DeclaringType != null)
                return type.DeclaringType.GetDefinition().NestedTypes.First(x => x.Name == type.Name);

            TypeReference reference;
            if (Processor.TargetModuleDefinition.TryGetTypeReference(type.FullName, out reference))
                return Processor.TargetModuleDefinition.ImportReference(reference).Resolve();
            if ((reference = Processor.TargetModuleDefinition.GetType(type.FullName)) != null)
                return (TypeDefinition)reference;

            throw new Exception($"Cannot get definition for {type.FullName}");
        }

        /// <summary>
        /// Load the <see cref="MethodDefinition"/> associated with the given
        /// <see cref="MethodBase"/>.
        /// </summary>
        public static MethodDefinition GetDefinition(this MethodBase method)
        {
            ParameterInfo[] parameters = method.GetParameters();

            return method.DeclaringType.GetDefinition().Methods
                .FirstOrDefault(x => x.Name == method.Name && x.Parameters.Count == parameters.Length
                                  && x.Parameters.Select(y => y.ParameterType.Name)
                                      .SequenceEqual(parameters.Select(y => y.ParameterType.Name)));
        }

        /// <summary>
        /// Load the <see cref="PropertyDefinition"/> associated with the given
        /// <see cref="PropertyInfo"/>.
        /// </summary>
        public static PropertyDefinition GetDefinition(this PropertyInfo property)
        {
            return property.DeclaringType.GetDefinition().Properties
                .FirstOrDefault(x => x.Name == property.Name);
        }

        /// <summary>
        /// Load the <see cref="FieldDefinition"/> associated with the given
        /// <see cref="FieldInfo"/>.
        /// </summary>
        public static FieldDefinition GetDefinition(this FieldInfo field)
        {
            return field.DeclaringType.GetDefinition().Fields
                .FirstOrDefault(x => x.Name == field.Name);
        }

        /// <summary>
        /// Load the <see cref="EventDefinition"/> associated with the given
        /// <see cref="EventInfo"/>.
        /// </summary>
        public static EventDefinition GetDefinition(this EventInfo ev)
        {
            return ev.DeclaringType.GetDefinition().Events
                .FirstOrDefault(x => x.Name == ev.Name);
        }

        /// <summary>
        /// Load the <see cref="ParameterDefinition"/> associated with the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        public static ParameterDefinition GetDefinition(this ParameterInfo parameter)
        {
            if (parameter.Member is PropertyInfo)
                return ((PropertyInfo)parameter.Member).GetReference().Parameters
                    .FirstOrDefault(x => x.Name == parameter.Name);

            return ((MethodBase)parameter.Member).GetReference().Parameters
                    .FirstOrDefault(x => x.Name == parameter.Name);
        }

        /// <summary>
        /// Load the <see cref="TypeReference"/> associated with the given
        /// <see cref="TypeInfo"/>.
        /// </summary>
        public static TypeReference GetReference(this TypeInfo type)
        {
            TypeReference reference;
            if (Processor.TargetModuleDefinition.TryGetTypeReference(type.FullName, out reference))
                return reference;

            AssemblyName an = type.Assembly.GetName();
            reference = new TypeReference(type.Namespace, type.Name, Processor.TargetModuleDefinition,
                                     new AssemblyNameReference(an.FullName, an.Version));
            if ((reference = reference.Resolve()) != null)
                return reference;

            throw new Exception($"Cannot get Reference for {type.FullName}");
        }

        /// <summary>
        /// Load the <see cref="TypeReference"/> associated with the given
        /// <see cref="Type"/>.
        /// </summary>
        public static TypeReference GetReference(this Type type)
        {
            return type.GetTypeInfo().GetReference();
        }

        /// <summary>
        /// Load the <see cref="MethodReference"/> associated with the given
        /// <see cref="MethodBase"/>.
        /// </summary>
        public static MethodReference GetReference(this MethodBase method)
        {
            ParameterInfo[] parameters = method.GetParameters();

            return Processor.TargetModuleDefinition.ImportReference(
                method.DeclaringType.GetDefinition().Methods.First(x => x.Name == method.Name && x.Parameters.Count == parameters.Length
                                  && x.Parameters.Select(y => y.ParameterType.FullName)
                                      .SequenceEqual(parameters.Select(y => y.ParameterType.FullName))));
        }

        /// <summary>
        /// Load the <see cref="PropertyReference"/> associated with the given
        /// <see cref="PropertyInfo"/>.
        /// </summary>
        public static PropertyReference GetReference(this PropertyInfo property)
        {
            return property.DeclaringType.GetDefinition().Properties
                .FirstOrDefault(x => x.Name == property.Name);
        }

        /// <summary>
        /// Load the <see cref="FieldReference"/> associated with the given
        /// <see cref="FieldInfo"/>.
        /// </summary>
        public static FieldReference GetReference(this FieldInfo field)
        {
            return Processor.TargetModuleDefinition.ImportReference(field.DeclaringType.GetDefinition().Fields
                .FirstOrDefault(x => x.Name == field.Name));
        }

        /// <summary>
        /// Load the <see cref="EventReference"/> associated with the given
        /// <see cref="EventInfo"/>.
        /// </summary>
        public static EventReference GetReference(this EventInfo ev)
        {
            return ev.DeclaringType.GetDefinition().Events
                .FirstOrDefault(x => x.Name == ev.Name);
        }
        #endregion
    }
}
