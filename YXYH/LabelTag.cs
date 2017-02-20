using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YHYXYH.YXYH
{
    /// <summary>
    /// 用于Label标签中Tag属性保存数据的类
    /// </summary>
    public class LabelTag
    {
        public LabelTag()
        { }
        public LabelTag(bool isSetToolTipText, int tagID, string tagDesc,string tagUnit)
        {
            IsSetToolTipText = isSetToolTipText;
            TagDesc = tagDesc;
            TagID = tagID;
            TagUnit = tagUnit;
        }
        public LabelTag(int tagID, string tagDesc,string tagUnit)
        {
            TagID = tagID;
            TagDesc = tagDesc;
            TagUnit = tagUnit;
        }
        public LabelTag(string reSize)
        {
            ReSizeStr = reSize;
        }
        public static LabelTag addReSize(LabelTag myLabelTag, string reSize)
        {
            myLabelTag.ReSizeStr = reSize;
            return myLabelTag;
        }
        /// <summary>
        /// 缩放时储存信息
        /// </summary>
        public string ReSizeStr = "";//缩放时储存信息
        /// <summary>
        /// 是否已设置ToolTip文本
        /// </summary>
        public bool IsSetToolTipText = false;//是否已设置ToolTip文本
        /// <summary>
        /// 测点ID
        /// </summary>
        public int TagID = 0;// 测点ID
        /// <summary>
        /// 保存测点描述
        /// </summary>
        public string TagDesc = "";//保存测点描述
        /// <summary>
        /// 测点单位，如：t/h
        /// </summary>
        public string TagUnit = "";//测点单位
        /// <summary>
        /// X坐标
        /// </summary>
        public int ControlX = 0;//X坐标
        /// <summary>
        /// Y坐标
        /// </summary>
        public int ControlY = 0;//Y坐标
        /// <summary>
        /// 父控件高度
        /// </summary>
        public int ParentHeight = 0;//父控件高度
        /// <summary>
        /// 父控件宽度
        /// </summary>
        public int ParentWidth = 0;//父控件宽度
        /// <summary>
        /// 控件高度
        /// </summary>
        public int Height = 0;//控件高度
        /// <summary>
        /// 控件宽度
        /// </summary>
        public int Width = 0;//控件宽度
        /// <summary>
        /// 所属机组的编号。1：1号机组，2：2号机组，3：公用
        /// </summary>
        public byte UnitNo = 1;//所属机组的编号。1：1号机组，2：2号机组，3：公用
    }
}
