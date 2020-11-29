﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DigimonWorld2Tool.UserControls
{
    public partial class RenderLayoutTab : UserControl
    {
        public RenderLayoutTab()
        {
            InitializeComponent();
            MapRenderLayer.Controls.Add(GridRenderLayer);
        }
    }
}
