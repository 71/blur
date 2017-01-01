#pragma warning disable CS1574
#pragma warning disable CS0419

namespace Blur
{
    partial class ILWriter
    {
        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer.
        /// </summary>
        public ILWriter Delegate(System.Action action) => this.Delegate(new object[0], action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer.
        /// </summary>
        public ILWriter Delegate<TReturn>(System.Func<TReturn> func) => this.Delegate(new object[0], func);


        
        #region TA
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA}"/>.
        /// </summary>
        public delegate void ILAction<in TA>(TA a);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, out TReturn>(TA a);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA>(TA a, ILAction<TA> action) => this.Delegate(new object[] { a }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TReturn>(TA a, ILFunc<TA, TReturn> func) => this.Delegate(new object[] { a }, func);
        #endregion

        
        #region TA, TB
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB>(TA a, TB b);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, out TReturn>(TA a, TB b);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB>(TA a, TB b, ILAction<TA, TB> action) => this.Delegate(new object[] { a, b }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TReturn>(TA a, TB b, ILFunc<TA, TB, TReturn> func) => this.Delegate(new object[] { a, b }, func);
        #endregion

        
        #region TA, TB, TC
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC>(TA a, TB b, TC c);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, out TReturn>(TA a, TB b, TC c);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC>(TA a, TB b, TC c, ILAction<TA, TB, TC> action) => this.Delegate(new object[] { a, b, c }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TReturn>(TA a, TB b, TC c, ILFunc<TA, TB, TC, TReturn> func) => this.Delegate(new object[] { a, b, c }, func);
        #endregion

        
        #region TA, TB, TC, TD
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD>(TA a, TB b, TC c, TD d);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, out TReturn>(TA a, TB b, TC c, TD d);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD>(TA a, TB b, TC c, TD d, ILAction<TA, TB, TC, TD> action) => this.Delegate(new object[] { a, b, c, d }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TReturn>(TA a, TB b, TC c, TD d, ILFunc<TA, TB, TC, TD, TReturn> func) => this.Delegate(new object[] { a, b, c, d }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE>(TA a, TB b, TC c, TD d, TE e);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, out TReturn>(TA a, TB b, TC c, TD d, TE e);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE>(TA a, TB b, TC c, TD d, TE e, ILAction<TA, TB, TC, TD, TE> action) => this.Delegate(new object[] { a, b, c, d, e }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TReturn>(TA a, TB b, TC c, TD d, TE e, ILFunc<TA, TB, TC, TD, TE, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF>(TA a, TB b, TC c, TD d, TE e, TF f);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF>(TA a, TB b, TC c, TD d, TE e, TF f, ILAction<TA, TB, TC, TD, TE, TF> action) => this.Delegate(new object[] { a, b, c, d, e, f }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, ILFunc<TA, TB, TC, TD, TE, TF, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG>(TA a, TB b, TC c, TD d, TE e, TF f, TG g);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, ILAction<TA, TB, TC, TD, TE, TF, TG> action) => this.Delegate(new object[] { a, b, c, d, e, f, g }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, ILFunc<TA, TB, TC, TD, TE, TF, TG, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, ILAction<TA, TB, TC, TD, TE, TF, TG, TH> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV, in TW>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV, in TW, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV, in TW, in TX>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV, in TW, in TX, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV, in TW, in TX, in TY>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, TY y);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV, in TW, in TX, in TY, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, TY y);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, TY y, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, TY y, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y }, func);
        #endregion

        
        #region TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TZ
        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TZ}"/>.
        /// </summary>
        public delegate void ILAction<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV, in TW, in TX, in TY, in TZ>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, TY y, TZ z);

        /// <summary>
        /// Represents a <see langword="delegate"/> that can be printed to IL, using <see cref="Delegate{TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TZ, TReturn}"/>.
        /// </summary>
        public delegate TReturn ILFunc<in TA, in TB, in TC, in TD, in TE, in TF, in TG, in TH, in TI, in TJ, in TK, in TL, in TM, in TN, in TO, in TP, in TQ, in TR, in TS, in TT, in TU, in TV, in TW, in TX, in TY, in TZ, out TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, TY y, TZ z);

        /// <summary>
        /// Emits the IL code of the given <paramref name="action"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TZ>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, TY y, TZ z, ILAction<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TZ> action) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z }, action);

        /// <summary>
        /// Emits the IL code of the given <paramref name="func"/> to the writer, passing
        /// it the given arguments.
        /// </summary>
        public ILWriter Delegate<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TZ, TReturn>(TA a, TB b, TC c, TD d, TE e, TF f, TG g, TH h, TI i, TJ j, TK k, TL l, TM m, TN n, TO o, TP p, TQ q, TR r, TS s, TT t, TU u, TV v, TW w, TX x, TY y, TZ z, ILFunc<TA, TB, TC, TD, TE, TF, TG, TH, TI, TJ, TK, TL, TM, TN, TO, TP, TQ, TR, TS, TT, TU, TV, TW, TX, TY, TZ, TReturn> func) => this.Delegate(new object[] { a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z }, func);
        #endregion

            }
}
