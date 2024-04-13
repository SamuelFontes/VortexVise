using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.States;

namespace VortexVise.Logic;

public static class BotLogic
{
    public static InputState GenerateBotInput()
    {
        var rand = new Random();
        var input = new InputState();
        input.Left = rand.NextDouble() >= 0.5;
        input.Right = rand.NextDouble() >= 0.5;
        input.Up = rand.NextDouble() >= 0.5;
        input.Down = rand.NextDouble() >= 0.5;
        input.FireWeapon = rand.NextDouble() >= 0.5;
        input.GrabDrop = rand.NextDouble() >= 0.5;
        input.Jump = rand.NextDouble() >= 0.99;
        input.Hook = rand.NextDouble() >= 0.9999;
        input.CancelHook = rand.NextDouble() >= 0.99;
        return input;
    }
}
