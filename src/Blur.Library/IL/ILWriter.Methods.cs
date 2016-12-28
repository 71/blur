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
            foreach (var ins in body.Instructions)
                this.instructions.Insert(position++, ins);

            return this;
        }

        /// <summary>
        /// Insert the IL body of an <see cref="Action"/> to this writer.
        /// </summary>
        /// <param name="action">The action whose body will be copied.</param>
        /// <param name="removeRet">If <see langword="true"/>, all instructions with the <see cref="OpCodes.Ret"/> opcode will be removed.</param>
        public ILWriter Body(Action action, bool removeRet = true)
        {
            MethodBody body = action.GetMethodInfo().GetDefinition().Body;

            foreach (Instruction ins in body.Instructions)
                if (!removeRet || ins.OpCode != OpCodes.Ret)
                    this.instructions.Insert(position++, ins);

            return this;
        }

        /// <summary>
        /// Insert the IL body of a <paramref name="method"/> to this writer.
        /// </summary>
        /// <param name="method">The method whose body will be copied.</param>
        public ILWriter Body(MethodInfo method) => this.Body(method.GetDefinition().Body);

        /// <summary>
        /// Insert the IL body of a <see langword="delegate"/> to this writer.
        /// </summary>
        /// <param name="del">The <see langword="delegate"/> whose body will be copied.</param>
        public ILWriter Body(Delegate del) => this.Body(del.GetMethodInfo());

        /// <summary>
        /// Insert the IL body of a <see langword="delegate"/> of
        /// type <typeparamref name="TDelegate"/> to this writer.
        /// </summary>
        /// <param name="del">The <see langword="delegate"/> whose body will be copied.</param>
        public ILWriter Body<TDelegate>(TDelegate del)
        {
            Delegate deleg = del as Delegate;
            if (deleg != null)
                return this.Body(deleg);
            throw new ArgumentException("Given object is not a delegate.", nameof(del));
        }
    }
}
