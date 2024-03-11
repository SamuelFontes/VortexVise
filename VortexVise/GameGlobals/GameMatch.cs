using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.Enums;

namespace VortexVise.GameGlobals;
public static class GameMatch
{
    public static GameMode CurrentGameMode { get; set; } = GameMode.DeathMatch;
    public static int CurrentGravity = 1000;
}
