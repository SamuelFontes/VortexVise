using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise.States;

public class SerializableReplay
{
    public SerializableReplay(List<GameState> states)
    {
        GameStates = states;
    }
    public List<GameState> GameStates { get; set; } = [];
}
