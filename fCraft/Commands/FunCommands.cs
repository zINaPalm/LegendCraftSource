//Copyright (C) <2012>  <Jon Baker, Glenn Mariën and Lao Tszy>

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomMaze;
using AIMLbot;
using System.Threading;

namespace fCraft
{
    internal static class FunCommands
    {
        internal static void Init()
        {
            CommandManager.RegisterCommand(CdRandomMaze);
            CommandManager.RegisterCommand(CdMazeCuboid);
            CommandManager.RegisterCommand(CdFirework);
            CommandManager.RegisterCommand(CdLife);
            CommandManager.RegisterCommand(CdPossess);
            CommandManager.RegisterCommand(CdUnpossess);
            CommandManager.RegisterCommand(CdThrow);
            Player.Moving += PlayerMoved;
        }

        public static void PlayerMoved(object sender, fCraft.Events.PlayerMovingEventArgs e)
        {
            if (e.Player.Info.IsFrozen || e.Player.SpectatedPlayer != null || !e.Player.SpeedMode)
                return;
            Vector3I oldPos = e.OldPosition.ToBlockCoords();
            Vector3I newPos = e.NewPosition.ToBlockCoords();
            //check if has moved 1 whole block
            if (newPos.X == oldPos.X + 1 || newPos.X == oldPos.X-1 || newPos.Y == oldPos.Y + 1 || newPos.Y == oldPos.Y-1)
            {
                Server.Players.Message("Old: " + newPos.ToString());
                Vector3I move = newPos - oldPos;
                int AccelerationFactor = 4;
                Vector3I acceleratedNewPos = oldPos + move * AccelerationFactor;
                //do not forget to check for all the null pointers here - TODO
                Map m = e.Player.World.Map;
                //check if can move through all the blocks along the path
                Vector3F normal = move.Normalize();
                Vector3I prevBlockPos = e.OldPosition.ToBlockCoords();
                for (int i = 1; i <= AccelerationFactor * move.Length; ++i)
                {
                    Vector3I pos = (oldPos + i * normal).Round();
                    if (prevBlockPos == pos) //didnt yet hit the next block
                        continue;
                    if (!m.InBounds(pos) || m.GetBlock(pos) != Block.Air) //got out of bounds or some solid block
                    {
                        acceleratedNewPos = (oldPos + normal * (i - 1)).Round();
                        break;
                    }
                    prevBlockPos = pos;
                }
                //teleport keeping the same orientation
                Server.Players.Message("New: "+ acceleratedNewPos.ToString());
                e.Player.Send(PacketWriter.MakeSelfTeleport(new Position((short)(acceleratedNewPos.X * 32), (short)(acceleratedNewPos.Y * 32), e.Player.Position.Z, e.NewPosition.R, e.NewPosition.L)));
            }
        }

        

            static readonly CommandDescriptor CdThrow = new CommandDescriptor
            {
                Name = "Throw",
                Aliases = new string[] { "Toss" },
                Category = CommandCategory.Chat | CommandCategory.Fun,
                Permissions = new Permission[] { Permission.Mute },
                IsConsoleSafe = true,
                Usage = "/Throw playername",
                Help = "Throw's a player.",
                NotRepeatable = true,
                Handler = ThrowHandler,
            };
  
        static void ThrowHandler(Player player, Command cmd)
        {
            string name = cmd.Next();
            string item = cmd.Next();
            if (name == null)
            {
                player.Message("Please enter a name");
                return;
            }
            Player target = Server.FindPlayerOrPrintMatches(player, name, false, true);
            if (target == null) return;
            if (target.Immortal)
            {
                player.Message("&SYou failed to throw {0}&S, they are immortal", target.ClassyName);
                return;
            }
            if (target == player)
            {
                player.Message("&sYou can't throw yourself... It's just physically impossible...");
                return;
            }
            double time = (DateTime.Now - player.Info.LastUsedSlap).TotalSeconds;
            if (time < 10)
            {
                player.Message("&WYou can use /Throw again in " + Math.Round(10 - time) + " seconds.");
                return;
            }
            Random random = new Random();
            int randomNumber = random.Next(1, 4);
           

                         if (randomNumber == 1)
                            if (player.Can(Permission.Slap, target.Info.Rank))
                            {
                                Position slap = new Position(target.Position.Z, target.Position.X, (target.World.Map.Bounds.YMax) * 32);
                                target.TeleportTo(slap);
                                Server.Players.CanSee(target).Except(target).Message("&SPlayer {0}&S was &eThrown&s by {1}&S.", target.ClassyName, player.ClassyName);
                                IRC.PlayerSomethingMessage(player, "thrown", target, null);
                                target.Message("&sYou were &eThrown&s by {0}&s.", player.ClassyName);
                                return;
                            }
                            else
                            {
                                player.Message("&sYou can only Throw players ranked {0}&S or lower",
                                               player.Info.Rank.GetLimit(Permission.Slap).ClassyName);
                                player.Message("{0}&S is ranked {1}", target.ClassyName, target.Info.Rank.ClassyName);
                            }
                       
                    
                       
                         if (randomNumber == 2)
                            if (player.Can(Permission.Slap, target.Info.Rank))
                            {
                                Position slap = new Position(target.Position.X, target.Position.Z, (target.World.Map.Bounds.YMax) * 32);
                                target.TeleportTo(slap);
                                Server.Players.CanSee(target).Except(target).Message("Player {0} was &eThrown&s by {1}&s.", target.ClassyName, player.ClassyName);
                                IRC.PlayerSomethingMessage(player, "thrown", target, null);
                                target.Message("&sYou were &eThrown&s by {0}&s.", player.ClassyName);
                                return;
                            }
                            else
                            {
                                player.Message("&sYou can only Throw players ranked {0}&S or lower",
                                               player.Info.Rank.GetLimit(Permission.Slap).ClassyName);
                                player.Message("{0}&S is ranked {1}", target.ClassyName, target.Info.Rank.ClassyName);
                            }

                         if (randomNumber == 3)
                            if (player.Can(Permission.Slap, target.Info.Rank))
                            {
                                Position slap = new Position(target.Position.Z, target.Position.Y, (target.World.Map.Bounds.XMax) * 32);
                                target.TeleportTo(slap);
                                Server.Players.CanSee(target).Except(target).Message("Player {0} was &eThrown&s by {1}&s.", target.ClassyName, player.ClassyName);
                                IRC.PlayerSomethingMessage(player, "thrown", target, null);
                                target.Message("&sYou were &eThrown&s by {0}&s.", player.ClassyName);
                                return;
                            }
                            else
                            {
                                player.Message("&sYou can only Throw players ranked {0}&S or lower",
                                               player.Info.Rank.GetLimit(Permission.Slap).ClassyName);
                                player.Message("{0}&S is ranked {1}", target.ClassyName, target.Info.Rank.ClassyName);
                            }
                        
                          if (randomNumber == 4)
                             if (player.Can(Permission.Slap, target.Info.Rank))
                             {
                                 Position slap = new Position(target.Position.Y, target.Position.Z, (target.World.Map.Bounds.XMax) * 32);
                                 target.TeleportTo(slap);
                                 Server.Players.CanSee(target).Except(target).Message("Player {0} was &eThrown&s by {1}&s.", target.ClassyName, player.ClassyName);
                                 IRC.PlayerSomethingMessage(player, "thrown", target, null);
                                 target.Message("&sYou were &eThrown&s by {0}&s.", player.ClassyName);
                                 return;
                             }
                             else
                             {
                                 player.Message("&sYou can only Throw players ranked {0}&S or lower",
                                                player.Info.Rank.GetLimit(Permission.Slap).ClassyName);
                                 player.Message("{0}&S is ranked {1}", target.ClassyName, target.Info.Rank.ClassyName);
                             }
                }
            
     

        #region Possess / UnPossess

        static readonly CommandDescriptor CdPossess = new CommandDescriptor
        {
            Name = "Possess",
            Category = CommandCategory.Fun,
            Permissions = new[] { Permission.Possess },
            Usage = "/Possess PlayerName",
            Handler = PossessHandler
        };

        static void PossessHandler(Player player, Command cmd)
        {
            string targetName = cmd.Next();
            if (targetName == null)
            {
                CdPossess.PrintUsage(player);
                return;
            }
            Player target = Server.FindPlayerOrPrintMatches(player, targetName, false, true);
            if (target == null) return;
            if (target.Immortal)
            {
                player.Message("You cannot possess {0}&S, they are immortal", target.ClassyName);
                return;
            }
            if (target == player)
            {
                player.Message("You cannot possess yourself.");
                return;
            }

            if (!player.Can(Permission.Possess, target.Info.Rank))
            {
                player.Message("You may only possess players ranked {0}&S or lower.",
                player.Info.Rank.GetLimit(Permission.Possess).ClassyName);
                player.Message("{0}&S is ranked {1}",
                                target.ClassyName, target.Info.Rank.ClassyName);
                return;
            }

            if (!player.Possess(target))
            {
                player.Message("Already possessing {0}", target.ClassyName);
            }
        }


        static readonly CommandDescriptor CdUnpossess = new CommandDescriptor
        {
            Name = "unpossess",
            Category = CommandCategory.Fun,
            Permissions = new[] { Permission.Possess },
            NotRepeatable = true,
            Usage = "/Unpossess target",
            Handler = UnpossessHandler
        };

        static void UnpossessHandler(Player player, Command cmd)
        {
            string targetName = cmd.Next();
            if (targetName == null)
            {
                CdUnpossess.PrintUsage(player);
                return;
            }
            Player target = Server.FindPlayerOrPrintMatches(player, targetName, true, true);
            if (target == null) return;

            if (!player.StopPossessing(target))
            {
                player.Message("You are not currently possessing anyone.");
            }
        }

        #endregion

        static readonly CommandDescriptor CdLife = new CommandDescriptor
        {
            Name = "Life",
            Category = CommandCategory.Fun,
            Permissions = new[] { Permission.DrawAdvanced },
            IsConsoleSafe = false,
            NotRepeatable = true,
            Usage = "/Life <command> [params]",
            Help = "&SGoogle \"Conwey's Game of Life\"\n'&H/Life help'&S for more usage info\n(c) 2012 LaoTszy",
            UsableByFrozenPlayers = false,
            Handler = LifeHandlerFunc,
        };
        
        static readonly CommandDescriptor CdFirework = new CommandDescriptor
        {
            Name = "Firework",
            Category = CommandCategory.Fun,
            Permissions = new[] { Permission.Fireworks },
            IsConsoleSafe = false,
            NotRepeatable = false,
            Usage = "/Firework",
            Help = "&SToggles Firework Mode on/off for yourself. " +
            "All Gold blocks will be replaced with fireworks if " +
            "firework physics are enabled for the current world.",
            UsableByFrozenPlayers = false,
            Handler = FireworkHandler
        };

        static void FireworkHandler(Player player, Command cmd)
        {
            if (player.fireworkMode)
            {
                player.fireworkMode = false;
                player.Message("Firework Mode has been turned off.");
                return;
            }
            else
            {
                player.fireworkMode = true;
                player.Message("Firework Mode has been turned on. " +
                    "All Gold blocks are now being replaced with Fireworks.");
            }
        }


        static readonly CommandDescriptor CdRandomMaze = new CommandDescriptor
        {
            Name = "RandomMaze",
            Aliases = new string[] { "3dmaze" },
            Category = CommandCategory.Fun,
            Permissions = new Permission[] { Permission.DrawAdvanced },
            RepeatableSelection = true,
            Help =
                "Choose the size (width, length and height) and it will draw a random maze at the chosen point. " +
                "Optional parameters tell if the lifts are to be drawn and if hint blocks (log) are to be added. \n(C) 2012 Lao Tszy",
            Usage = "/randommaze <width> <length> <height> [nolifts] [hints]",
            Handler = MazeHandler
        };
        static readonly CommandDescriptor CdMazeCuboid = new CommandDescriptor
        {
            Name = "MazeCuboid",
            Aliases = new string[] { "Mc", "Mz", "Maze" },
            Category = CommandCategory.Fun,
            Permissions = new Permission[] { Permission.DrawAdvanced },
            RepeatableSelection = true,
            Help =
                "Draws a cuboid with the current brush and with a random maze inside.(C) 2012 Lao Tszy",
            Usage = "/MazeCuboid [block type]",
            Handler = MazeCuboidHandler,
        };

        private static void MazeHandler(Player p, Command cmd)
        {
            try
            {
                RandomMazeDrawOperation op = new RandomMazeDrawOperation(p, cmd);
                BuildingCommands.DrawOperationBegin(p, cmd, op);
            }
            catch (Exception e)
            {
                Logger.Log(LogType.Error, "Error: " + e.Message);
            }
        }
        private static void MazeCuboidHandler(Player p, Command cmd)
        {
            try
            {
                MazeCuboidDrawOperation op = new MazeCuboidDrawOperation(p);
                BuildingCommands.DrawOperationBegin(p, cmd, op);
            }
            catch (Exception e)
            {
                Logger.Log(LogType.Error, "Error: " + e.Message);
            }
        }
        private static void LifeHandlerFunc(Player p, Command cmd)
        {
        	try
        	{
                if (!cmd.HasNext)
                {
                    p.Message("&H/Life <command> <params>. Commands are Help, Create, Delete, Start, Stop, Set, List, Print");
                    p.Message("Type /Life help <command> for more information");
                    return;
                }
				LifeHandler.ProcessCommand(p, cmd);
        	}
        	catch (Exception e)
        	{
				p.Message("Error: " + e.Message);
        	}
        }

        
    }
}
