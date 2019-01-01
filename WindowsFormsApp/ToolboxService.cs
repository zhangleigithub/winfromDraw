using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsFormsApp.Shapes;

namespace WindowsFormsApp
{
    /// <summary>
    /// ToolboxService
    /// </summary>
    public class ToolboxService : IToolboxService
    {
        #region 字段

        /// <summary>
        /// items
        /// </summary>
        private List<Type> items = new List<Type>();

        #endregion

        #region 属性

        /// <summary>
        /// 
        /// </summary>
        public Type GetSelectItem
        {
            get
            {
                if (this.Index == -1)
                {
                    return null;
                }

                return this.items[this.Index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 构造函数
        /// </summary>
        public ToolboxService()
        {
            this.Index = -1;
            this.items.Add(typeof(EntityShape));
        }

        #endregion
    }
}
