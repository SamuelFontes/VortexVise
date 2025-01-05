﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.Core.Interfaces
{
    public interface IWindowService
    {
        public void InitializeWindow();
        public void CloseWindow();
        public void HandleWindowEvents();
    }
}
