using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ReflectionOpCode = System.Reflection.Emit.OpCode;
using ReflectionOpCodes = System.Reflection.Emit.OpCodes;

namespace Blur
{
    partial class ILWriter
    {
        #region Proxies
        private class CodeReaderProxy : BinaryReader
        {
            private readonly object codeReader;
            private readonly MethodInfo readVariables;
            private readonly MethodInfo getCallSite;
            private readonly MethodInfo getParameter;
            private readonly MethodInfo getString;

            public int Position => (int)BaseStream.Position;

            public CodeReaderProxy(Stream ms, MetadataReaderProxy metadata) : base(ms)
            {
                Type proxy = typeof(Instruction).GetTypeInfo().Assembly.GetType("CodeReader");
                this.codeReader = Activator.CreateInstance(proxy, metadata);

                this.readVariables = proxy.GetRuntimeMethod(nameof(ReadVariables), new[] { typeof(MetadataToken) });
                this.getCallSite = proxy.GetRuntimeMethod(nameof(GetCallSite), new[] { typeof(MetadataToken) });
                this.getString = proxy.GetRuntimeMethod(nameof(GetString), new[] { typeof(MetadataToken) });
                this.getParameter = proxy.GetRuntimeMethod(nameof(GetParameter), new[] { typeof(int) });
            }

            public MetadataToken ReadToken()
                => new MetadataToken(ReadUInt32());

            public Collection<VariableDefinition> ReadVariables(MetadataToken token)
                => (Collection<VariableDefinition>)readVariables.Invoke(codeReader, new object[] { token });

            public CallSite GetCallSite(MetadataToken token)
                => (CallSite)getCallSite.Invoke(codeReader, new object[] { token });

            public ParameterDefinition GetParameter(int index)
                => (ParameterDefinition)getParameter.Invoke(codeReader, new object[] { index });

            public string GetString(MetadataToken token)
                => (string)getString.Invoke(codeReader, new object[] { token });
        }

        private class MetadataReaderProxy
        {
            private readonly object metadataReader;
            private readonly MethodInfo lookupToken;

            public MetadataReaderProxy(ModuleDefinition module)
            {
                Type proxy = metadataReader.GetType();
                this.metadataReader = typeof(ModuleDefinition).GetRuntimeField("reader").GetValue(module);

                this.lookupToken = proxy.GetRuntimeMethod(nameof(LookupToken), new[] { typeof(MetadataToken) });
            }

            public IMetadataTokenProvider LookupToken(MetadataToken token)
                => (IMetadataTokenProvider)lookupToken.Invoke(metadataReader, new object[] { token });
        }
        #endregion

        #region Utils
        private static void Advance(BinaryReader reader, int count)
        {
            reader.BaseStream.Seek(count, SeekOrigin.Current);
        }

        private class ParseContext
        {
            public CodeReaderProxy Code { get; set; }
            public MetadataReaderProxy Metadata { get; set; }
            public Collection<VariableDefinition> Variables { get; set; }
            public IILVisitor Visitor { get; set; }
        }

        static void ParseFatMethod(ParseContext context)
        {
            var code = context.Code;

            Advance(code, 4);
            var code_size = code.ReadInt32();
            var local_var_token = code.ReadToken();

            if (local_var_token != MetadataToken.Zero)
                context.Variables = code.ReadVariables(local_var_token);

            ParseCode(code_size, context);
        }

        static void ParseCode(int code_size, ParseContext context)
        {
            var code = context.Code;
            var metadata = context.Metadata;
            var visitor = context.Visitor;

            var start = code.Position;
            var end = start + code_size;

            while (code.Position < end)
            {
                var il_opcode = code.ReadByte();
                var opcode = il_opcode != 0xfe
                    ? (new OpCode[0xe0 + 1])[il_opcode]
                    : (new OpCode[0xe0 + 1])[code.ReadByte()];

                switch (opcode.OperandType)
                {
                    case OperandType.InlineNone:
                        visitor.OnInlineNone(opcode);
                        break;
                    case OperandType.InlineSwitch:
                        var length = code.ReadInt32();
                        var branches = new int[length];
                        for (int i = 0; i < length; i++)
                            branches[i] = code.ReadInt32();
                        visitor.OnInlineSwitch(opcode, branches);
                        break;
                    case OperandType.ShortInlineBrTarget:
                        visitor.OnInlineBranch(opcode, code.ReadSByte());
                        break;
                    case OperandType.InlineBrTarget:
                        visitor.OnInlineBranch(opcode, code.ReadInt32());
                        break;
                    case OperandType.ShortInlineI:
                        if (opcode == OpCodes.Ldc_I4_S)
                            visitor.OnInlineSByte(opcode, code.ReadSByte());
                        else
                            visitor.OnInlineByte(opcode, code.ReadByte());
                        break;
                    case OperandType.InlineI:
                        visitor.OnInlineInt32(opcode, code.ReadInt32());
                        break;
                    case OperandType.InlineI8:
                        visitor.OnInlineInt64(opcode, code.ReadInt64());
                        break;
                    case OperandType.ShortInlineR:
                        visitor.OnInlineSingle(opcode, code.ReadSingle());
                        break;
                    case OperandType.InlineR:
                        visitor.OnInlineDouble(opcode, code.ReadDouble());
                        break;
                    case OperandType.InlineSig:
                        visitor.OnInlineSignature(opcode, code.GetCallSite(code.ReadToken()));
                        break;
                    case OperandType.InlineString:
                        visitor.OnInlineString(opcode, code.GetString(code.ReadToken()));
                        break;
                    case OperandType.ShortInlineArg:
                        visitor.OnInlineArgument(opcode, code.GetParameter(code.ReadByte()));
                        break;
                    case OperandType.InlineArg:
                        visitor.OnInlineArgument(opcode, code.GetParameter(code.ReadInt16()));
                        break;
                    case OperandType.ShortInlineVar:
                        visitor.OnInlineVariable(opcode, GetVariable(context, code.ReadByte()));
                        break;
                    case OperandType.InlineVar:
                        visitor.OnInlineVariable(opcode, GetVariable(context, code.ReadInt16()));
                        break;
                    case OperandType.InlineTok:
                    case OperandType.InlineField:
                    case OperandType.InlineMethod:
                    case OperandType.InlineType:
                        var member = metadata.LookupToken(code.ReadToken());
                        switch (member.MetadataToken.TokenType)
                        {
                            case TokenType.TypeDef:
                            case TokenType.TypeRef:
                            case TokenType.TypeSpec:
                                visitor.OnInlineType(opcode, (TypeReference)member);
                                break;
                            case TokenType.Method:
                            case TokenType.MethodSpec:
                                visitor.OnInlineMethod(opcode, (MethodReference)member);
                                break;
                            case TokenType.Field:
                                visitor.OnInlineField(opcode, (FieldReference)member);
                                break;
                            case TokenType.MemberRef:
                                var field_ref = member as FieldReference;
                                if (field_ref != null)
                                {
                                    visitor.OnInlineField(opcode, field_ref);
                                    break;
                                }

                                var method_ref = member as MethodReference;
                                if (method_ref != null)
                                {
                                    visitor.OnInlineMethod(opcode, method_ref);
                                    break;
                                }

                                throw new InvalidOperationException();
                        }
                        break;
                }
            }
        }

        static VariableDefinition GetVariable(ParseContext context, int index)
        {
            return context.Variables[index];
        }

        static void Parse(ModuleDefinition module, byte[] bytes, IILVisitor visitor)
        {
            MetadataReaderProxy metadata = new MetadataReaderProxy(module);

            using (MemoryStream ms = new MemoryStream(bytes))
            using (CodeReaderProxy reader = new CodeReaderProxy(ms, metadata))
            {
                ParseContext ctx = new ParseContext
                {
                    Code = new CodeReaderProxy(ms, metadata),
                    Metadata = metadata,
                    Visitor = visitor
                };

                byte flags = reader.ReadByte();

                switch (flags & 0x3)
                {
                    case 0x2:
                        int code_size = flags >> 2;
                        ParseCode(code_size, ctx);
                        break;

                    case 0x3:
                        Advance(reader, -1);
                        ParseFatMethod(ctx);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
        #endregion

        #region Visitors
        interface IILVisitor
        {
            void OnInlineNone(OpCode opcode);
            void OnInlineSByte(OpCode opcode, sbyte value);
            void OnInlineByte(OpCode opcode, byte value);
            void OnInlineInt32(OpCode opcode, int value);
            void OnInlineInt64(OpCode opcode, long value);
            void OnInlineSingle(OpCode opcode, float value);
            void OnInlineDouble(OpCode opcode, double value);
            void OnInlineString(OpCode opcode, string value);
            void OnInlineBranch(OpCode opcode, int offset);
            void OnInlineSwitch(OpCode opcode, int[] offsets);
            void OnInlineVariable(OpCode opcode, VariableDefinition variable);
            void OnInlineArgument(OpCode opcode, ParameterDefinition parameter);
            void OnInlineSignature(OpCode opcode, CallSite callSite);
            void OnInlineType(OpCode opcode, TypeReference type);
            void OnInlineField(OpCode opcode, FieldReference field);
            void OnInlineMethod(OpCode opcode, MethodReference method);
        }

        class ILVisitor : IILVisitor, IEnumerable<Instruction>
        {
            private readonly IList<Instruction> Instructions;
            private int Position;

            public ILVisitor(IList<Instruction> ins, int position)
            {
                this.Instructions = ins;
                this.Position = position;
            }

            public IEnumerator<Instruction> GetEnumerator() => Instructions.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => Instructions.GetEnumerator();

            private static ReflectionOpCode R(OpCode opcode)
            {
                return (ReflectionOpCode)typeof(ReflectionOpCodes).GetTypeInfo()
                    .GetDeclaredField(opcode.Name)
                    .GetValue(null);
            }

            public void OnInlineNone(OpCode opcode)
                => Instructions.Add(Instruction.Create(opcode));

            public void OnInlineSByte(OpCode opcode, sbyte value)
                => Instructions.Add(Instruction.Create(opcode, value));

            public void OnInlineByte(OpCode opcode, byte value)
                => Instructions.Add(Instruction.Create(opcode, value));

            public void OnInlineInt32(OpCode opcode, int value)
                => Instructions.Add(Instruction.Create(opcode, value));

            public void OnInlineInt64(OpCode opcode, long value)
                => Instructions.Add(Instruction.Create(opcode, value));

            public void OnInlineSingle(OpCode opcode, float value)
                => Instructions.Add(Instruction.Create(opcode, value));

            public void OnInlineDouble(OpCode opcode, double value)
                => Instructions.Add(Instruction.Create(opcode, value));

            public void OnInlineString(OpCode opcode, string value)
                => Instructions.Add(Instruction.Create(opcode, value));

            public void OnInlineBranch(OpCode opcode, int offset)
            {
                Instruction target = null;
                for (int i = 0; i < Instructions.Count; i++)
                {
                    if (Instructions[i].Offset == offset)
                    {
                        target = Instructions[i];
                        break;
                    }
                }

                Instructions.Add(Instruction.Create(opcode, target));
            }

            public void OnInlineSwitch(OpCode opcode, int[] offsets)
            {
                Instruction[] targets = new Instruction[offsets.Length];
                for (int o = 0; o < targets.Length; o++)
                {
                    for (int i = 0; i < Instructions.Count; i++)
                    {
                        if (Instructions[i].Offset == offsets[o])
                        {
                            targets[o] = Instructions[i];
                            goto NextTarget;
                        }
                    }

                    NextTarget: ;
                }

                Instructions.Add(Instruction.Create(opcode, targets));
            }

            public void OnInlineVariable(OpCode opcode, VariableDefinition variable)
                => Instructions.Add(Instruction.Create(opcode, variable));

            public void OnInlineArgument(OpCode opcode, ParameterDefinition parameter)
                => Instructions.Add(Instruction.Create(opcode, parameter));

            public void OnInlineSignature(OpCode opcode, CallSite callSite)
                => Instructions.Add(Instruction.Create(opcode, callSite));

            public void OnInlineType(OpCode opcode, TypeReference type)
                => Instructions.Add(Instruction.Create(opcode, type));

            public void OnInlineField(OpCode opcode, FieldReference field)
                => Instructions.Add(Instruction.Create(opcode, field));

            public void OnInlineMethod(OpCode opcode, MethodReference method)
                => Instructions.Add(Instruction.Create(opcode, method));
        }
#endregion

        /// <summary>
        /// Parse a <see cref="byte"/> array to <see cref="Instruction"/>s.
        /// </summary>
        public static IEnumerable<Instruction> FromByteArray(byte[] bytes)
        {
            List<Instruction> instructions = new List<Instruction>();
            ILVisitor visitor = new ILVisitor(instructions, 0);
            Parse(Processor.TargetModuleDefinition, bytes, visitor);
            return instructions.AsEnumerable();
        }

        /// <summary>
        /// Parse a <see cref="byte"/> array to <see cref="Instruction"/>s,
        /// and add them to the instructions to print.
        /// </summary>
        public ILWriter Parse(byte[] bytes)
        {
            ILVisitor visitor = new ILVisitor(this.instructions, position);
            Parse(Processor.TargetModuleDefinition, bytes, visitor);
            return this;
        }
    }
}
