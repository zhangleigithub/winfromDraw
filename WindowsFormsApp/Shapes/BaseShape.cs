using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WindowsFormsApp.Shapes
{
    /// <summary>
    /// BaseShape
    /// </summary>
    [Serializable]
    public abstract class BaseShape
    {
        #region 字段

        /// <summary>
        /// brushs
        /// </summary>
        private static readonly Dictionary<Color, Brush> Brushes = new Dictionary<Color, Brush>();

        #endregion

        #region 属性

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public PointF Location { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public SizeF Size { get; set; }

        /// <summary>
        /// Bounds
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return new RectangleF(this.Location, this.Size);
            }
        }

        /// <summary>
        /// ZIndex
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// Selected
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// BackColor
        /// </summary>
        public Color BackColor { get; set; }

        /// <summary>
        /// ForeColor
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        /// Font
        /// </summary>
        public Font Font { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseShape()
        {
            this.BackColor = Color.AntiqueWhite;
            this.ForeColor = Color.Black;
            this.Font = SystemFonts.DefaultFont;
            this.Selected = false;
            this.ZIndex = 0;
            this.Location = Point.Empty;
            this.Size = new SizeF(100, 100);
        }

        /// <summary>
        /// OnPaint
        /// </summary>
        /// <param name="g">g</param>
        public virtual void OnPaint(Graphics g)
        {
            //当前选中
            if (this.Selected)
            {
                RectangleF rect = this.Bounds;
                rect.Inflate(1f, 1f);

                //边框
                g.DrawRectangle(Pens.LightBlue, rect.X, rect.Y, rect.Width, rect.Height);

                //高亮显示
                rect.Inflate(2f, 2f);
                rect.Offset(-2f, -2f);
                SizeF size = new SizeF(4f, 4f);
                RectangleF[] rectangles = new RectangleF[8];
                rectangles[0] = new RectangleF(new PointF(rect.X, rect.Y), size);
                rectangles[1] = new RectangleF(new PointF(rect.X + rect.Width / 2, rect.Y), size);
                rectangles[2] = new RectangleF(new PointF(rect.X + rect.Width, rect.Y), size);
                rectangles[3] = new RectangleF(new PointF(rect.X + rect.Width, rect.Y + rect.Height / 2), size);
                rectangles[4] = new RectangleF(new PointF(rect.X + rect.Width, rect.Y + rect.Height), size);
                rectangles[5] = new RectangleF(new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height), size);
                rectangles[6] = new RectangleF(new PointF(rect.X, rect.Y + rect.Height), size);
                rectangles[7] = new RectangleF(new PointF(rect.X, rect.Y + rect.Height / 2), size);

                g.DrawRectangles(Pens.LightBlue, rectangles);
            }
        }

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="p">p</param>
        /// <returns>bool</returns>
        public bool Contains(PointF p)
        {
            return this.Bounds.Contains(p);
        }

        /// <summary>
        /// GetBrush
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Brush</returns>
        protected Brush GetBrush(Color color)
        {
            if (!BaseShape.Brushes.ContainsKey(color))
            {
                BaseShape.Brushes.Add(color, new SolidBrush(color));
            }

            return BaseShape.Brushes[color];
        }

        #endregion
    }
}
