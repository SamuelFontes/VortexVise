﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameObjects;

namespace VortexVise.States;

public class InputState
{
    public bool Left { get; set; } = false;
    public bool Right { get; set; } = false;
    public bool Up { get; set; } = false;
    public bool Down { get; set; } = false;
    public bool Jump { get; set; } = false;
    public bool Hook { get; set; } = false;
    public bool CancelHook { get; set; } = false;

}
