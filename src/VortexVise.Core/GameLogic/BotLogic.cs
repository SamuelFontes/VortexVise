using VortexVise.Core.Enums;
using VortexVise.Core.GameGlobals;
using VortexVise.Core.Models;
using VortexVise.Core.States;

namespace VortexVise.Core.GameLogic
{
    public static class BotLogic
    {
        public static void Init(GameState state)
        {
            for (int i = 0; i < GameMatch.NumberOfBots; i++)
            {
                var bot = new PlayerState(Guid.NewGuid(), GameAssets.Gameplay.Skins.OrderBy(x => Guid.NewGuid()).First());
                bot.IsBot = true;
                var b = new Bot();
                b.Id = bot.Id;
                GameMatch.Bots.Add(b);
                state.PlayerStates.Add(bot); // add bot

            }
        }
        public static void Unload()
        {
            GameMatch.Bots.Clear();
        }
        public static InputState GenerateBotInput(GameState state, PlayerState botState)
        {
            var rand = new Random();
            var input = new InputState();
            Bot bot = GameMatch.Bots.First(x => x.Id == botState.Id);
            bot.TickCounter++;
            if (bot.TickCounter > GameCore.GameTickRate * 2)
            {
                bot.DropTarget = null;
                bot.EnemyTarget = null;
            }
            if (bot.EnemyTarget == null || bot.EnemyTarget.IsDead)
            {
                bot.EnemyTarget = state.PlayerStates.Where(x => x.Id != botState.Id && !x.IsDead).OrderBy(x => Math.Abs(x.Position.Y - botState.Position.Y)).FirstOrDefault();
            }
            if (botState.WeaponStates.Count == 0 && bot.DropTarget == null)
            {
                bot.DropTarget = state.WeaponDrops.OrderBy(x => Math.Abs(x.Position.Y - botState.Position.Y)).FirstOrDefault();
            }
            else if (botState.HeathPoints < GameMatch.DefaultPlayerHeathPoints * 0.4f)
            {
                bot.DropTarget = state.WeaponDrops.OrderBy(x => Math.Abs(x.Position.Y - botState.Position.Y)).FirstOrDefault(x => x.WeaponState.Weapon.WeaponType == WeaponType.Heal);
            }
            else if (botState.WeaponStates.Count > 0 && bot.DropTarget != null)
            {
                bot.DropTarget = null;
            }
            if (bot.DropTarget != null && (botState.WeaponStates.Count == 0 || botState.HeathPoints < GameMatch.DefaultPlayerHeathPoints * 0.4f))
            {
                if (bot.DropTarget.Position.X < botState.Position.X)
                {
                    input.Left = true;
                }
                else
                {
                    input.Right = true;
                }
                if (bot.DropTarget.Position.Y < botState.Position.Y)
                {
                    input.Up = rand.NextDouble() >= 0.3;
                    if (!input.Up) input.Down = rand.NextDouble() >= 0.3;
                    input.JetPack = rand.NextDouble() >= 0.5;
                }
                else
                {
                    input.Down = rand.NextDouble() >= 0.3;
                    if (!input.Down) input.Up = rand.NextDouble() >= 0.3;
                }
            }
            else if (bot.EnemyTarget != null)
            {
                if (bot.EnemyTarget.Position.X < botState.Position.X)
                {
                    input.Left = rand.NextDouble() >= 0.2;
                }
                else
                {
                    input.Right = rand.NextDouble() >= 0.2;
                }
                if (bot.EnemyTarget.Position.Y < botState.Position.Y)
                {
                    input.Up = rand.NextDouble() >= 0.3;
                    if (!input.Up) input.Down = rand.NextDouble() >= 0.3;
                    input.JetPack = rand.NextDouble() >= 0.2;
                }
                else
                {
                    input.Down = rand.NextDouble() >= 0.3;
                    if (!input.Down) input.Up = rand.NextDouble() >= 0.3;
                }
            }
            else
            {
                input.Left = rand.NextDouble() >= 0.8;
                input.Right = rand.NextDouble() >= 0.8;
                input.Up = rand.NextDouble() >= 0.5;
                input.Down = rand.NextDouble() >= 0.8;
            }


            // Try to avoid bot falling from map
            if (input.Down && botState.Position.Y > GameMatch.CurrentMap.Texture.Height * 0.7)
            {
                input.Down = false;
                input.Up = true;
                input.JetPack = rand.NextDouble() >= 0.8;
            }

            if (bot.EnemyTarget != null && ((int)bot.EnemyTarget.Position.Y == (int)botState.Position.Y || bot.EnemyTarget.Position.Y > botState.Position.Y - 8 && bot.EnemyTarget.Position.Y < botState.Position.Y + 32))
                input.FireWeapon = rand.NextDouble() >= 0.1;
            else
            {
                input.FireWeapon = rand.NextDouble() >= 0.9999;
            }

            if (botState.WeaponStates.Count > 0)
                input.GrabDrop = rand.NextDouble() >= 0.999;
            else
                input.GrabDrop = rand.NextDouble() >= 0.2;
            input.Jump = rand.NextDouble() >= 0.9;
            input.Hook = rand.NextDouble() >= 0.005;
            input.CancelHook = rand.NextDouble() >= 0.99;
            if (input.CancelHook) input.Hook = false;
            return input;
        }
    }
}
