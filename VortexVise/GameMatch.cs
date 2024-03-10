using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VortexVise;
public static class GameMatch
{
    public static GameMode CurrentGameMode { get; set; } = GameMode.DeathMatch;
    public static int CurrentGravity = 1000;
}
