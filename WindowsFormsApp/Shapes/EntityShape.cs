using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace WindowsFormsApp.Shapes
{
    /// <summary>
    /// EntityShape
    /// </summary>
    [Serializable]
    public class EntityShape : BaseShape
    {
        #region 字段

        /// <summary>
        /// lineHigth
        /// </summary>
        private float lineHigth = 20;

        #endregion

        #region 属性


        /// <summary>
        /// Properties
        /// </summary>
        public List<EntityProperty> Properties { get; private set; }

        /// <summary>
        /// GridColor
        /// </summary>
        public Color GridColor { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 构造函数
        /// </summary>
        public EntityShape() : base()
        {
            this.Properties = new List<EntityProperty>();
            this.GridColor = Color.DarkRed;
        }

        /// <summary>
        /// OnPaint
        /// </summary>
        /// <param name="g">g</param>
        public override void OnPaint(Graphics g)
        {
            base.OnPaint(g);
            RectangleF rect = this.Bounds;
            g.SetClip(rect);
            rect.Inflate(-1, -1);

            RectangleF stringRect = new RectangleF(rect.Location, new SizeF(rect.Width, this.lineHigth));
            PointF beginPoint = new PointF(rect.Location.X, rect.Location.Y + this.lineHigth);
            PointF endPoint = new PointF(rect.Location.X + rect.Size.Width, rect.Location.Y + this.lineHigth);
            Pen gridPen = new Pen(this.GridColor, 1);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            //边框
            g.FillRectangle(base.GetBrush(this.BackColor), rect);
            g.DrawRectangle(gridPen, rect.X, rect.Y, rect.Width, rect.Height);

            //Name
            g.DrawString(this.Name, new Font(this.Font.Name, this.Font.Size, FontStyle.Bold), base.GetBrush(this.ForeColor), stringRect, sf);
            g.DrawLine(gridPen, beginPoint, endPoint);

            //Properties
            sf.Alignment = StringAlignment.Near;

            foreach (var item in this.Properties)
            {
                stringRect.Y += this.lineHigth;
                g.DrawString(string.Format("+{0}:{1}", item.Name, item.Type), this.Font, base.GetBrush(this.ForeColor), stringRect, sf);
            }

            g.ResetClip();
        }

        #endregion

        public class EntityProperty
        {
            #region 属性

            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Type { get; set; }

            #endregion
        }
    }
}
