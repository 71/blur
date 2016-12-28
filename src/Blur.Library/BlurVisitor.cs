using System;
using Mono.Cecil;
using System.Reflection;

namespace Blur
{
    #region ProcessingState enum
    /// <summary>
    /// Enum used by <see cref="BlurVisitor"/> to
    /// behave differently based on the state of
    /// the processing of an assembly.
    /// </summary>
    [Flags]
    public enum ProcessingState
    {
        /// <summary>
        /// Processing hasn't started yet.
        /// </summary>
        Before = 1,

        /// <summary>
        /// Processing has ended.
        /// </summary>
        After = 2,

        /// <summary>
        /// Accept both <see cref="Before"/> and <see cref="After"/>.
        /// </summary>
        Any = Before | After
    }
    #endregion

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
        /// Gets the <see cref="ProcessingState"/> that must be
        /// matched to execute this visitor.
        /// </summary>
        protected ProcessingState RequiredState { get; private set; }

        /// <summary>
        /// Gets the current <see cref="ProcessingState"/>.
        /// </summary>
        protected ProcessingState CurrentState { get; private set; }

        /// <summary>
        /// Create a new <see cref="BlurVisitor"/>.
        /// </summary>
        /// <param name="requiredState">
        /// <see cref="ProcessingState"/> that must be matched to execute
        /// this visitor.
        /// </param>
        protected BlurVisitor(ProcessingState requiredState)
        {
            this.RequiredState = requiredState;
        }

        /// <summary>
        /// Visit the given object.
        /// </summary>
        internal void Visit(object obj, ProcessingState state)
        {
            this.CurrentState = state;

            if ((this.RequiredState & state) != state)
                return;

            try
            {
                if (obj is TypeDefinition)
                    this.Visit((TypeDefinition)obj);
                else if (obj is PropertyDefinition)
                    this.Visit((PropertyDefinition)obj);
                else if (obj is FieldDefinition)
                    this.Visit((FieldDefinition)obj);
                else if (obj is EventDefinition)
                    this.Visit((EventDefinition)obj);
                else if (obj is MethodDefinition)
                {
                    MethodDefinition method = (MethodDefinition)obj;

                    foreach (ParameterDefinition parameter in method.Parameters)
                        this.Visit(parameter, method);

                    this.Visit(method);
                }
                else if (obj is AssemblyDefinition)
                    this.Visit((AssemblyDefinition)obj);
            }
            catch (Exception e)
            {
                throw new Exception($"Error encountered by {this.GetType()} whilst visiting {obj.GetType()}.", e);
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
