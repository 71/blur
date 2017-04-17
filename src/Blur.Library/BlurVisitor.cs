using System;
using Mono.Cecil;

namespace Blur
{   
    /// <summary>
    /// Represents an <see cref="object"/> that can register to events
    /// to edit <see cref="object"/>s.
    /// </summary>
    /// <remarks>
    /// By default, this object does nothing on each Visit, but does
    /// not throw either. You can thus override any method you like,
    /// and ignore others.
    /// </remarks>
    public abstract class BlurVisitor
    {
        /// <summary>
        /// Gets the priority, the number that defines when a Blur visitor should run,
        /// compared to other visitors.
        /// <para>
        /// The priority must be a number between 0 and 1000.
        /// </para>
        /// <para>
        /// For example, the internal <see cref="BlurVisitor"/>, that visits
        /// all members marked with an <see cref="IWeaver"/> attribute,
        /// has a priority of <c>100</c>.
        /// </para>
        /// </summary>
        public abstract int Priority { get; }

        /// <summary>
        /// Visit the given object.
        /// </summary>
        internal void Visit(object obj)
        {
            try
            {
                if (obj is TypeDefinition typeDef)
                    this.Visit(typeDef);
                else if (obj is PropertyDefinition propDef)
                    this.Visit(propDef);
                else if (obj is FieldDefinition fieldDef)
                    this.Visit(fieldDef);
                else if (obj is EventDefinition eventDef)
                    this.Visit(eventDef);
                else if (obj is MethodDefinition methodDef)
                {
                    foreach (ParameterDefinition parameterDef in methodDef.Parameters)
                        this.Visit(parameterDef, methodDef);

                    this.Visit(methodDef);
                }
                else if (obj is AssemblyDefinition assemblyDef)
                    this.Visit(assemblyDef);
            }
            catch (Exception e)
            {
                throw new Exception($"Error encountered by {this.GetType()} whilst visiting {obj}.", e);
            }
        }

        #region Protected methods Visit()
        /// <summary>
        /// Visit the given <paramref name="assembly"/>.
        /// </summary>
        protected virtual void Visit(AssemblyDefinition assembly)
        {
            // Do nothing.
        }

        /// <summary>
        /// Visit the given <paramref name="type"/>.
        /// </summary>
        protected virtual void Visit(TypeDefinition type)
        {
            // Do nothing.
        }

        /// <summary>
        /// Visit the given <paramref name="field"/>.
        /// </summary>
        protected virtual void Visit(FieldDefinition field)
        {
            // Do nothing.
        }

        /// <summary>
        /// Visit the given <paramref name="property"/>.
        /// </summary>
        protected virtual void Visit(PropertyDefinition property)
        {
            // Do nothing.
        }

        /// <summary>
        /// Visit the given <paramref name="event"/>.
        /// </summary>
        protected virtual void Visit(EventDefinition @event)
        {
            // Do nothing.
        }

        /// <summary>
        /// Visit the given <paramref name="method"/>.
        /// </summary>
        protected virtual void Visit(MethodDefinition method)
        {
            // Do nothing.
        }

        /// <summary>
        /// Visit the given <paramref name="parameter"/>.
        /// </summary>
        protected virtual void Visit(ParameterDefinition parameter, MethodDefinition method)
        {
            // Do nothing.
        }
        #endregion
    }
}
