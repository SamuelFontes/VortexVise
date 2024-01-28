using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameObjects;

namespace VortexVise.States;

public class GameState
{
    public double CurrentTime { get; set; }
    public float Gravity { get; set; }
    public List<PlayerState> PlayerStates { get; set; } = [];
}
