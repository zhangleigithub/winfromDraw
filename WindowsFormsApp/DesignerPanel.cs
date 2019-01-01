using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp.Shapes;

namespace WindowsFormsApp
{
    /// <summary>
    /// DesignerPanel
    /// </summary>
    public class DesignerPanel : Panel
    {
        /// <summary>
        /// CursorsExtend
        /// </summary>
        internal enum CursorsExtend
        {
            SizeAll,
            SizeNW,
            SizeSE,
            SizeNE,
            SizeSW,
            SizeN,
            SizeS,
            SizeW,
            SizeE,
            Default
        }

        #region 字段

        /// <summary>
        /// beginPoint
        /// </summary>
        private PointF beginPoint = Point.Empty;

        /// <summary>
        /// lastMousePoint
        /// </summary>
        private PointF lastMousePoint = Point.Empty;

        /// <summary>
        /// selectedRectangle
        /// </summary>
        private RectangleF selectedRectangle = Rectangle.Empty;

        /// <summary>
        /// scaleValue
        /// </summary>
        private float scaleValue = 1.0f;

        /// <summary>
        /// cursorExtend
        /// </summary>
        private CursorsExtend cursorExtend = CursorsExtend.Default;

        #endregion

        #region 属性

        /// <summary>
        /// Shapes
        /// </summary>
        public List<BaseShape> Shapes { get; private set; }

        /// <summary>
        /// ScaleValue
        /// </summary>
        public float ScaleValue
        {
            get
            {
                return this.scaleValue;
            }
            set
            {
                this.scaleValue = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// ToolboxService
        /// </summary>
        public IToolboxService ToolboxService { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 构造函数
        /// </summary>
        public DesignerPanel()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.Shapes = new List<BaseShape>();
        }

        /// <summary>
        /// OnPaint
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            BufferedGraphicsContext graphicsContext = BufferedGraphicsManager.Current;
            BufferedGraphics buffer = graphicsContext.Allocate(e.Graphics, e.ClipRectangle);
            Graphics g = buffer.Graphics;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.CompositingQuality = CompositingQuality.HighSpeed;

            g.ScaleTransform(this.scaleValue, this.scaleValue);
            g.Clear(this.BackColor);

            //绘制Shape
            foreach (var item in this.Shapes)
            {
                item.OnPaint(g);
            }

            //绘制选择区域
            if (this.selectedRectangle != Rectangle.Empty)
            {
                g.DrawRectangle(Pens.LightSkyBlue, this.selectedRectangle.X, this.selectedRectangle.Y, this.selectedRectangle.Width, this.selectedRectangle.Height);
            }

            buffer.Render(e.Graphics);
            g.Dispose();
            buffer.Dispose();
        }

        /// <summary>
        /// OnMouseDown
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            PointF mousePoint = this.TransformMousePoint(e.Location);
            this.beginPoint = mousePoint;
            this.lastMousePoint = mousePoint;
            this.selectedRectangle = Rectangle.Empty;

            if (this.ToolboxService.GetSelectItem != null)
            {
                BaseShape shape = this.ToolboxService.GetSelectItem.Assembly.CreateInstance(this.ToolboxService.GetSelectItem.FullName) as BaseShape;
                shape.Name = "Entity" + (this.Shapes.Count(x => x.Name.StartsWith("Entity")) + 1);
                shape.Location = mousePoint;
                this.Shapes.Add(shape);
            }

            //多选模式
            if (this.Shapes.Count(x => x.Selected) > 1 && this.Shapes.Exists(x => this.GetCursor(x.Bounds, mousePoint) == CursorsExtend.SizeAll))
            {
                //
            }
            else //单选模式
            {
                foreach (var item in this.Shapes)
                {
                    item.Selected = item.Contains(mousePoint);
                }
            }

            this.Refresh();
        }

        /// <summary>
        /// OnMouseMove
        /// </summary>
        /// <param name="e">参数</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            PointF mousePoint = this.TransformMousePoint(e.Location);

            //鼠标左键按下
            if (e.Button == MouseButtons.Left)
            {
                //多选模式
                if (this.Shapes.Count(x => x.Selected) > 1 && this.Shapes.Exists(x => this.GetCursor(x.Bounds, mousePoint) == CursorsExtend.SizeAll))
                {
                    List<BaseShape> shapes = this.Shapes.FindAll(x => x.Selected);

                    float xOffset = mousePoint.X - this.lastMousePoint.X;
                    float yOffset = mousePoint.Y - this.lastMousePoint.Y;

                    foreach (var item in shapes)
                    {
                        item.Location = new PointF(item.Location.X + xOffset, item.Location.Y + yOffset);
                    }
                }
                else if (this.Shapes.Count(x => x.Selected) == 1) //单选模式
                {
                    BaseShape shape = this.Shapes.FirstOrDefault(x => x.Selected);

                    if (shape != null)
                    {
                        this.TransformShape(shape, this.lastMousePoint, mousePoint);
                    }
                }
                else //选择区域
                {
                    this.selectedRectangle = this.GetSelectedRegion(this.beginPoint, mousePoint);
                }

                this.Refresh();
            }
            else //鼠标移动
            {
                BaseShape shape = this.Shapes.FirstOrDefault(x => x.Contains(mousePoint));

                if (shape != null)
                {
                    this.cursorExtend = this.GetCursor(shape.Bounds, mousePoint);
                    this.Cursor = this.GetCursor(this.cursorExtend);
                }
                else
                {
                    this.cursorExtend = CursorsExtend.Default;
                    this.Cursor = Cursors.Default;
                }
            }

            //上一次鼠标位置
            this.lastMousePoint = mousePoint;
        }

        /// <summary>
        /// OnMouseUp
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.selectedRectangle != RectangleF.Empty)
            {
                foreach (var item in this.Shapes)
                {
                    item.Selected = this.selectedRectangle.Contains(item.Bounds);
                }
            }

            this.selectedRectangle = Rectangle.Empty;

            this.Refresh();
        }

        /// <summary>
        /// 获取鼠标状态
        /// </summary>
        /// <param name="rect">rect</param>
        /// <param name="p">Point</param>
        /// <returns>CursorsExtend</returns>
        private CursorsExtend GetCursor(RectangleF rect, PointF p)
        {
            Size rangeSize = new Size(8, 8);
            int range = 8;

            if (new RectangleF(rect.Location, rangeSize).Contains(p))
            {
                return CursorsExtend.SizeNW;
            }
            else if (new RectangleF(new PointF(rect.X + rect.Width - rangeSize.Width, rect.Y), rangeSize).Contains(p))
            {
                return CursorsExtend.SizeNE;
            }
            else if (new RectangleF(new PointF(rect.X + rect.Width - rangeSize.Width, rect.Y + rect.Height - rangeSize.Width), rangeSize).Contains(p))
            {
                return CursorsExtend.SizeSE;
            }
            else if (new RectangleF(new PointF(rect.X, rect.Y + rect.Height - rangeSize.Height), rangeSize).Contains(p))
            {
                return CursorsExtend.SizeSW;
            }
            else if (Math.Abs(rect.Y - p.Y) <= range)
            {
                return CursorsExtend.SizeN;
            }
            else if (Math.Abs((rect.X + rect.Width) - p.X) <= range)
            {
                return CursorsExtend.SizeW;
            }
            else if (Math.Abs((rect.Y + rect.Height) - p.Y) <= range)
            {
                return CursorsExtend.SizeS;
            }
            else if (Math.Abs(rect.X - p.X) <= range)
            {
                return CursorsExtend.SizeE;
            }
            else if (rect.Contains(p))
            {
                return CursorsExtend.SizeAll;
            }
            else
            {
                return CursorsExtend.Default;
            }
        }

        /// <summary>
        /// 转换鼠标状态
        /// </summary>
        /// <param name="cursors">cursors</param>
        /// <returns>Cursor</returns>
        private Cursor GetCursor(CursorsExtend cursors)
        {
            switch (cursors)
            {
                case CursorsExtend.SizeAll:
                    return Cursors.SizeAll;
                case CursorsExtend.SizeNW:
                    return Cursors.SizeNWSE;
                case CursorsExtend.SizeSE:
                    return Cursors.SizeNWSE;
                case CursorsExtend.SizeNE:
                    return Cursors.SizeNESW;
                case CursorsExtend.SizeSW:
                    return Cursors.SizeNESW;
                case CursorsExtend.SizeN:
                    return Cursors.SizeNS;
                case CursorsExtend.SizeS:
                    return Cursors.SizeNS;
                case CursorsExtend.SizeW:
                    return Cursors.SizeWE;
                case CursorsExtend.SizeE:
                    return Cursors.SizeWE;
                case CursorsExtend.Default:
                    return Cursors.Default;
                default:
                    return Cursors.Default;
            }
        }

        /// <summary>
        /// 转换Shape
        /// </summary>
        /// <param name="shape">shape</param>
        /// <param name="beginPoint">beginPoint</param>
        /// <param name="endPoint">endPoint</param>
        private void TransformShape(BaseShape shape, PointF beginPoint, PointF endPoint)
        {
            float xOffset = endPoint.X - beginPoint.X;
            float yOffset = endPoint.Y - beginPoint.Y;

            if (this.cursorExtend == CursorsExtend.SizeAll)
            {
                shape.Location = new PointF(shape.Location.X + xOffset, shape.Location.Y + yOffset);
            }
            else if (this.cursorExtend == CursorsExtend.SizeNW)
            {
                shape.Location = new PointF(shape.Location.X + xOffset, shape.Location.Y + yOffset);
                shape.Size = new SizeF(shape.Size.Width - xOffset, shape.Size.Height - yOffset);
            }
            else if (this.cursorExtend == CursorsExtend.SizeSE)
            {
                shape.Size = new SizeF(shape.Size.Width + xOffset, shape.Size.Height + yOffset);
            }
            else if (this.cursorExtend == CursorsExtend.SizeNE)
            {
                shape.Location = new PointF(shape.Location.X, shape.Location.Y + yOffset);
                shape.Size = new SizeF(shape.Size.Width + xOffset, shape.Size.Height - yOffset);
            }
            else if (this.cursorExtend == CursorsExtend.SizeSW)
            {
                shape.Location = new PointF(shape.Location.X + xOffset, shape.Location.Y);
                shape.Size = new SizeF(shape.Size.Width - xOffset, shape.Size.Height + yOffset);
            }
            else if (this.cursorExtend == CursorsExtend.SizeN)
            {
                shape.Location = new PointF(shape.Location.X, shape.Location.Y + yOffset);
                shape.Size = new SizeF(shape.Size.Width, shape.Size.Height - yOffset);
            }
            else if (this.cursorExtend == CursorsExtend.SizeS)
            {
                shape.Size = new SizeF(shape.Size.Width, shape.Size.Height + yOffset);
            }
            else if (this.cursorExtend == CursorsExtend.SizeW)
            {
                shape.Size = new SizeF(shape.Size.Width + xOffset, shape.Size.Height);
            }
            else if (this.cursorExtend == CursorsExtend.SizeE)
            {
                shape.Location = new PointF(shape.Location.X + xOffset, shape.Location.Y);
                shape.Size = new SizeF(shape.Size.Width - xOffset, shape.Size.Height);
            }
        }

        /// <summary>
        /// 获取选择区域
        /// </summary>
        /// <param name="beginPoint">beginPoint</param>
        /// <param name="endPoint">endPoint</param>
        /// <returns></returns>
        private RectangleF GetSelectedRegion(PointF beginPoint, PointF endPoint)
        {
            RectangleF result = RectangleF.Empty;

            if (this.beginPoint.X < endPoint.X && this.beginPoint.Y < endPoint.Y)//左上角到右下角画矩形
            {
                result = new RectangleF(this.beginPoint.X, this.beginPoint.Y, Math.Abs(endPoint.X - this.beginPoint.X), Math.Abs(endPoint.Y - this.beginPoint.Y));
            }
            else if (this.beginPoint.X > endPoint.X && this.beginPoint.Y < endPoint.Y)//右上角到左小角画矩形
            {
                result = new RectangleF(endPoint.X, beginPoint.Y, Math.Abs(endPoint.X - this.beginPoint.X), Math.Abs(endPoint.Y - this.beginPoint.Y));
            }
            else if (beginPoint.X > endPoint.X && this.beginPoint.Y > endPoint.Y)//右小角到左上角画矩形
            {
                result = new RectangleF(endPoint.X, endPoint.Y, Math.Abs(endPoint.X - this.beginPoint.X), Math.Abs(endPoint.Y - this.beginPoint.Y));
            }
            else if (this.beginPoint.X < endPoint.X && this.beginPoint.Y > endPoint.Y)//左下角到右上角画矩形
            {
                result = new RectangleF(this.beginPoint.X, endPoint.Y, Math.Abs(endPoint.X - this.beginPoint.X), Math.Abs(endPoint.Y - this.beginPoint.Y));
            }

            return result;
        }

        /// <summary>
        /// 转换鼠标点
        /// </summary>
        /// <param name="p">MousePoint</param>
        /// <returns>缩放模式转换后的鼠标位置</returns>
        private PointF TransformMousePoint(Point p)
        {
            return new PointF(p.X / this.scaleValue, p.Y / this.scaleValue);
        }

        #endregion
    }
}
