using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApp
{
    public interface IToolboxService
    {
        #region 属性

        /// <summary>
        /// 
        /// </summary>
        Type GetSelectItem { get; }

        /// <summary>
        /// 
        /// </summary>
        int Index { get; set; }

        #endregion
    }
}
