using System;
using System.Reflection;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        /// <summary>
        /// Insert the given <see cref="MethodBody"/> to this writer.
        /// </summary>
        private ILWriter Body(MethodBody body)
        {
            foreach (VariableDefinition vd in body.Variables)
                this.variables.Add(vd);
            foreach (var ins in body.Instructions)
                this.instructions.Insert(position++, ins);

            return this;
        }

        /// <summary>
        /// Insert the IL body of a method to this writer.
        /// </summary>
        /// <param name="del">The method that shall be copied.</param>
        public ILWriter Body(Delegate del) => this.Body(del.GetMethodInfo());

        /// <summary>
        /// Insert the IL body of a <see langword="delegate"/> of
        /// type <typeparamref name="TDelegate"/> to this writer.
        /// </summary>
        public ILWriter Body<TDelegate>(TDelegate del)
        {
            Delegate deleg = del as Delegate;
            if (deleg != null)
                return this.Body(deleg);
            throw new ArgumentException("Given object is not a delegate.", nameof(del));
        }
    }
}
