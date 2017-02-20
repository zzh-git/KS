using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;

namespace KSPrj
{
    class Opaquelayer
    {
        private MyOpaqueLayer m_OpaqueLayer = null;//半透明蒙板层
        /// <summary>
        /// 显示遮罩层
        /// </summary>
        /// <param name="control"></param>
        /// <param name="alpha"></param>
        /// <param name="showLoadingImage"></param>
        public void ShowOpaqueLayer(Control control, int alpha, bool showLoadingImage)
        {
            if (this.m_OpaqueLayer == null)
            {
                this.m_OpaqueLayer = new MyOpaqueLayer(alpha, showLoadingImage);
                control.Controls.Add(this.m_OpaqueLayer);
                this.m_OpaqueLayer.Dock = DockStyle.Fill;
                this.m_OpaqueLayer.BringToFront();
            }
            this.m_OpaqueLayer.Enabled = true;
            this.m_OpaqueLayer.Visible = true;


        }

        /// <summary>
        /// 隐藏遮罩层
        /// </summary>
        public  void HideOpaqueLayer()
        {
            if (this.m_OpaqueLayer != null)
            {
                this.m_OpaqueLayer.Enabled = false;
                this.m_OpaqueLayer.Visible = false;
            }
        }
    }
}
