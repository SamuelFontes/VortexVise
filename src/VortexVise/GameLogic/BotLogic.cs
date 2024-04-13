using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VortexVise.GameGlobals;
using VortexVise.States;

namespace VortexVise.Logic;

public static class BotLogic
{
    public static InputState GenerateBotInput(GameState state, PlayerState botState)
    {
        var rand = new Random();
        var input = new InputState();
        var bot = GameMatch.Bots.First(x => x.Id == botState.Id);
        if (botState.WeaponStates.Count == 0 && bot.DropTarget == null)
        {
            bot.DropTarget = state.WeaponDrops.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
        else if (botState.WeaponStates.Count > 0 && bot.DropTarget != null)
        {
            bot.DropTarget = null;
        }
        if (bot.DropTarget != null)
        {
            if (bot.DropTarget.Position.X < botState.Position.X)
            {
                input.Left = true;
            }
            else
            {
                input.Right = true;
            }
            if(bot.DropTarget.Position.Y < botState.Position.Y)
            {
                input.Up = rand.NextDouble() >= 0.3;
            }
            else
            {
                input.Down = rand.NextDouble() >= 0.3;
            }
        }
        else
        {
            input.Left = rand.NextDouble() >= 0.5;
            input.Right = rand.NextDouble() >= 0.5;
            input.Up = rand.NextDouble() >= 0.5;
            input.Down = rand.NextDouble() >= 0.5;
        }
        input.FireWeapon = rand.NextDouble() >= 0.5;
        input.GrabDrop = rand.NextDouble() >= 0.5;
        input.Jump = rand.NextDouble() >= 0.9;
        input.Hook = rand.NextDouble() >= 0.005;
        input.CancelHook = rand.NextDouble() >= 0.99;
        if (input.CancelHook) input.Hook = false;
        return input;
    }
}
