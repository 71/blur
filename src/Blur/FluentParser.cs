/*
 ______ _     _   _ _____ _   _ ___________  ___  ______  _____ ___________ 
 |  ___| |   | | | |  ___| \ | |_   _| ___ \/ _ \ | ___ \/  ___|  ___| ___ \
 | |_  | |   | | | | |__ |  \| | | | | |_/ / /_\ \| |_/ /\ `--.| |__ | |_/ /
 |  _| | |   | | | |  __|| . ` | | | |  __/|  _  ||    /  `--. \  __||    / 
 | |   | |___| |_| | |___| |\  | | | | |   | | | || |\ \ /\__/ / |___| |\ \ 
 \_|   \_____/\___/\____/\_| \_/ \_/ \_|   \_| |_/\_| \_|\____/\____/\_| \_|


  A single file to parse arguments easily.
  Can parse string, int, bool, string[], int[]

  UNLICENSED: <http://unlicense.org/UNLICENSE>
  PART OF THE MINI PROJECT <https://github.com/6A/mini>

*/

namespace Blur.Processing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    #region Utils
    public static class FluentParser
    {
        /// <summary>
        /// Split a string into multiple arguments,
        /// taking care of escaping single and double quotes.
        /// </summary>
        public static string[] Split(string str)
        {
            List<string> args = new List<string>();

            int quote = 0; // 0 (no quote), 1 (single), 2 (double)
            bool escaped = false; // escaped by a \\
            string arg = ""; // arg currently being processed

            foreach (char c in str)
            {
                if (escaped) // char is escaped, move on
                {
                    escaped = false;
                    arg += c;
                }

                switch (c)
                {
                    case '\\': // next char is escaped
                        escaped = true;
                        break;

                    case ' ':
                        if (quote == 0)
                        {
                            if (!string.IsNullOrWhiteSpace(arg))
                                args.Add(arg);
                            arg = "";
                        }
                        else
                        {
                            arg += c;
                        }
                        break;

                    case '\'':
                        switch (quote)
                        {
                            case 0:
                                // enter quote
                                quote = 2;
                                break;
                            case 1:
                                // leave quote
                                if (!string.IsNullOrWhiteSpace(arg))
                                    args.Add(arg);
                                arg = "";
                                break;
                            default:
                                // in another quote, add char
                                arg += c;
                                break;
                        }
                        break;

                    case '"':
                        switch (quote)
                        {
                            case 0:
                                // enter quote
                                quote = 2;
                                break;
                            case 2:
                                // leave quote
                                if (!string.IsNullOrWhiteSpace(arg))
                                    args.Add(arg);
                                arg = "";
                                break;
                            default:
                                // in another quote, add char
                                arg += c;
                                break;
                        }
                        break;

                    default:
                        arg += c;
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(arg)) // end of string, process last arg
                args.Add(arg);

            // only take non-null args
            return args.ToArray();
        }
    }
    #endregion

    /// <summary>
    /// Helper class that parses arguments to a class fluently.
    /// </summary>
    /// <typeparam name="T">The type of the arguments you wish to parse.</typeparam>
    /// <example>
    /// ParsedArgs = new FluentParser{Arguments}()
    ///     .Define(x => x.Address, arg => arg.As("address").As('a').Required())
    ///     .Define(x => x.Ports, arg => arg.As("ports", "port").As('p').Required())
    ///     .Parse(args);
    /// </example>
    /// <remarks>
    /// Since this file is to be included in any project,
    /// the "internal" keyword cannot be used. Some methods
    /// will thus be public, but aren't meant to be used.
    /// </remarks>
    public sealed class FluentParser<T> where T : new()
    {
        #region Fields, Properties, Constructors
        private readonly List<IFluentParserBuilderInternal> parsers;

        /// <summary>
        /// Initialize a new fluent parser..
        /// </summary>
        public FluentParser()
        {
            parsers = new List<IFluentParserBuilderInternal>();
        }
        #endregion

        /// <summary>
        /// Define a new argument, using its name in the class and a <see cref="IFluentParserBuilder{TReturn}"/>.
        /// </summary>
        /// <typeparam name="TReturn">The type of the argument; can be <see cref="string"/>, <see cref="bool"/>, <see cref="int"/>,
        /// <see cref="string"/>[] and <see cref="int"/>[].</typeparam>
        /// <returns>This instance of <see cref="FluentParser{T}"/></returns>
        public FluentParser<T> Define<TReturn>(Expression<Func<T, TReturn>> expr, Func<IFluentParserBuilder<TReturn>, IFluentParserBuilder<TReturn>> arg)
        {
            parsers.Add((IFluentParserBuilderInternal)arg(new FluentParserBuilder<TReturn>(expr.Body as MemberExpression)));
            return this;
        }

        #region Parse
        /// <summary>
        /// Check whether or not the given argument is a value or a key (starts by "--" or "-").
        /// </summary>
        private static bool IsPropertyKey(string arg)
        {
            return arg.StartsWith("--") && arg.Length > 2 || arg.StartsWith("-") && arg.Length == 2;
        }

        /// <summary>
        /// Set the value of the given member to <paramref name="value"/>.
        /// </summary>
        private static void Set(MemberExpression ex, T item, object value)
        {
            if (ex.Member is FieldInfo field)
                field.SetValue(item, value);
            else if (ex.Member is PropertyInfo prop)
                prop.SetValue(item, value);
            else
                throw new NotSupportedException("Can only set values to fields and properties.");
        }

        /// <summary>
        /// Parse the given arguments to <see cref="T"/>.
        /// </summary>
        public T Parse(string args, out IEnumerable<string> unsetRequiredProperties)
        {
#pragma warning disable 168
            return Parse(FluentParser.Split(args), out unsetRequiredProperties);
#pragma warning restore 168
        }

        /// <summary>
        /// Parse the given arguments to <see cref="T"/>.
        /// </summary>
        public T Parse(string args)
        {
#pragma warning disable 168
            return Parse(FluentParser.Split(args), out IEnumerable<string> unsetRequiredProperties);
#pragma warning restore 168
        }

        /// <summary>
        /// Parse the given arguments to <see cref="T"/>.
        /// </summary>
        public T Parse(string[] args)
        {
#pragma warning disable 168
            return Parse(args, out IEnumerable<string> unsetRequiredProperties);
#pragma warning restore 168
        }

        #region Actual Parse() method
        /// <summary>
        /// Parse the given arguments to <see cref="T"/>.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="unsetRequiredProperties">A list of all required properties that weren't found.</param>
        public T Parse(string[] args, out IEnumerable<string> unsetRequiredProperties)
        {
            List<string> setProperties = new List<string>();
            string main = string.Join(" ", args.TakeWhile(x => x[0] != '-'));
            T item = new T();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (!IsPropertyKey(arg))
                    continue;

                foreach (var fpb in parsers.Where(x => arg.Length == 2 && x.CanParse(arg[1]) || x.CanParse(arg.Substring(2))))
                {
                    // parse values depending on the type accepted by fpb
                    if (fpb.ParsesTo == typeof(bool))
                    {
                        Set(fpb.Expression, item, !arg.StartsWith("no-"));
                        setProperties.Add(fpb.Expression.Member.Name);
                        break;
                    }
                    if (fpb.ParsesTo == typeof(string))
                    {
                        Set(fpb.Expression, item, args.Length > ++i && IsPropertyKey(args[i]) ? null : args[i]);
                        setProperties.Add(fpb.Expression.Member.Name);
                        break;
                    }
                    if (fpb.ParsesTo == typeof(int))
                    {
                        if (args.Length > ++i && int.TryParse(args[i], out int val))
                        {
                            Set(fpb.Expression, item, val);
                            setProperties.Add(fpb.Expression.Member.Name);
                            break;
                        }
                        continue;
                    }
                    if (fpb.ParsesTo == typeof(string[]))
                    {
                        List<string> s = new List<string>();

                        while (args.Length > ++i && !IsPropertyKey(args[i]))
                            s.Add(args[i]);
                        i--;

                        Set(fpb.Expression, item, s.ToArray());
                        setProperties.Add(fpb.Expression.Member.Name);
                        break;
                    }
                    if (fpb.ParsesTo == typeof(int[]))
                    {
                        List<int> s = new List<int>();

                        bool success = true;
                        while (args.Length > ++i && !IsPropertyKey(args[i]))
                        {
                            if (int.TryParse(args[i], out int val))
                            {
                                s.Add(val);
                            }
                            else
                            {
                                success = false;
                                i--;
                                break;
                            }
                        }

                        if (success)
                        {
                            Set(fpb.Expression, item, s.ToArray());
                            setProperties.Add(fpb.Expression.Member.Name);
                            break;
                        }
                        continue;
                    }
                    throw new NotSupportedException("Cast to " + fpb.ParsesTo.Name + " not supported.");
                }
            }

            if (main != "")
            {
                foreach (IFluentParserBuilderInternal fpb in parsers.Where(x => x.IsMain))
                {
                    // parse values depending on the type accepted by fpb
                    if (fpb.ParsesTo != typeof(string))
                        throw new NotSupportedException("Cast to " + fpb.ParsesTo.Name + " not supported.");

                    Set(fpb.Expression, item, main);
                    setProperties.Add(fpb.Expression.Member.Name);
                    break;
                }
            }

            unsetRequiredProperties = from parser in parsers
                                      where parser.IsRequired
                                      let name = parser.Expression.Member.Name
                                      where setProperties.IndexOf(name) == -1
                                      select name;

            return item;
        }
        #endregion

        #endregion

        #region Builder

        #region Interfaces
        /// <summary>
        /// Interface used internally, since
        /// <see cref="FluentParserBuilder{TReturn}"/>
        /// requires a type argument that is not <see cref="T"/>.
        /// </summary>
        private interface IFluentParserBuilderInternal
        {
            Type ParsesTo { get; }
            bool CanParse(string name);
            bool CanParse(char name);

            bool IsRequired { get; }
            bool IsMain { get; }
            object DefaultValue { get; }

            MemberExpression Expression { get; }
        }

        /// <summary>
        /// Fluent interface used to define arguments in <see cref="FluentParser{T}"/>.
        /// It is recommended to chain calls to methods of this interface.
        /// </summary>
        /// <typeparam name="TReturn">The type of the argument; can be <see cref="string"/>, <see cref="bool"/>, <see cref="int"/>,
        /// <see cref="string"/>[] and <see cref="int"/>[].</typeparam>
        public interface IFluentParserBuilder<in TReturn>
        {
            /// <summary>
            /// The parameter "--[long name]" will be accepted for parsing.
            /// </summary>
            IFluentParserBuilder<TReturn> As(params string[] longNames);

            /// <summary>
            /// The parameter "-[short name]" will be accepted for parsing.
            /// </summary>
            IFluentParserBuilder<TReturn> As(params char[] shortNames);

            /// <summary>
            /// If this argument isn't given by the user, this property
            /// will have the value <paramref name="defaultValue"/> instead.
            /// </summary>
            IFluentParserBuilder<TReturn> Default(TReturn defaultValue);

            /// <summary>
            /// If this argument isn't given by the user,
            /// the name of this property will be found in the out parameter
            /// of <see cref="FluentParser{T}.Parse(string)"/>.
            /// </summary>
            /// <remarks>
            /// This method will not throw.
            /// </remarks>
            IFluentParserBuilder<TReturn> Required(bool isRequired);

            /// <summary>
            /// This argument has no name.
            /// </summary>
            /// <example>
            /// app.exe command
            /// -> "command" will be the main return type.
            /// </example>
            /// <remarks>
            /// Only supports <see cref="string"/>s.
            /// </remarks>
            IFluentParserBuilder<TReturn> Main();
        }
        #endregion

        #region Implementation
        /// <inheritdoc />
        private sealed class FluentParserBuilder<TReturn> : IFluentParserBuilder<TReturn>, IFluentParserBuilderInternal
        {
            public Type ParsesTo => typeof(TReturn);
            public bool IsRequired { get; private set; }
            public bool IsMain { get; private set; }
            public object DefaultValue { get; private set; }
            public MemberExpression Expression { get; }

            private readonly List<string> long_names = new List<string>();
            private readonly List<char> short_names = new List<char>();

            /// <summary>*internal*</summary>
            public FluentParserBuilder(MemberExpression ex)
            {
                this.Expression = ex;
            }

            /// <summary>*internal*</summary>
            public bool CanParse(string longName)
            {
                if (typeof(TReturn) == typeof(bool) && longName.StartsWith("no-"))
                    return long_names.Contains(longName.Substring(3));
                return long_names.Contains(longName);
            }

            /// <summary>*internal*</summary>
            public bool CanParse(char longName)
            {
                return short_names.Contains(longName);
            }

            public IFluentParserBuilder<TReturn> As(params string[] longNames)
            {
                long_names.AddRange(longNames);
                return this;
            }

            public IFluentParserBuilder<TReturn> As(params char[] shortNames)
            {
                short_names.AddRange(shortNames);
                return this;
            }

            public IFluentParserBuilder<TReturn> Default(TReturn value)
            {
                DefaultValue = value;
                return this;
            }

            public IFluentParserBuilder<TReturn> Main()
            {
                IsMain = true;
                return this;
            }

            public IFluentParserBuilder<TReturn> Required(bool isRequired)
            {
                IsRequired = isRequired;
                return this;
            }
        }
        #endregion

        #endregion
    }
}