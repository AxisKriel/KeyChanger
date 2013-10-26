﻿using System;
using System.IO;
using System.Collections.Generic;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using Newtonsoft.Json;

namespace KeyChanger
{
    [ApiVersion(1, 14)]
    public class KeyChanger : TerrariaPlugin
    {

        public KeyChanger(Main game)
            : base(game)
        {
        }
        private static string savepath = TShock.SavePath; //NEW
        private static Config config;
        private static string KeyTypes = /*"gold | */"jungle, temple, crimson, frozen, hallowed, corruption";
        public Region marketregion = new Region();                      // |All allowed
        public Region jungleregion = new Region();                      // |Jungle allowed
        public Region templeregion = new Region();                      // |Temple allowed
        public Region crimsonregion = new Region();                     // |Crimson Allowed
        public Region frozenregion = new Region();                      // |Frozen Allowed
        public Region hallowedregion = new Region();                    // |Hallowed Allowed
        public Region corruptionregion = new Region();                   // |Corruption Allowed

        public override Version Version
        {
            get { return new Version("1.0"); }
        }


        public override string Name
        {
            get { return "KeyChanger"; }
        }


        public override string Author
        {
            get { return "Enerdy"; }
        }


        public override string Description
        {
            get { return "SBPlanet KeyChanger System: Enchanges special chest keys by their correspondent items"; }
        }


        public override void Initialize()
        {
            //This is the main command, which branches to everything the plugin can do, by checking the first parameter a player inputs
            Commands.ChatCommands.Add(new Command("key.change", KeyChange, "key"));
            ReadConfig();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        //Config Contents
        class Config
        {
            public bool EnableRegionEnchanges = false;                      // |Default set to false
            public bool MarketOnly = true;                                  // |Use only a general market region
            public bool EnableJungleKey = true;
            public bool EnableTempleKey = true;
            public bool EnableFrozenKey = true;
            public bool EnableCrimsonKey = true;
            public bool EnableHallowedKey = true;
            public bool EnableCorruptionKey = true;

            //public int[] GoldKeyItem = new int[] { 1 };                     // |Placeholder
            public int JungleKeyItem = 1156;                                // |Piranha Gun
            public int TempleKeyItem = 1293;                                // |Lihzahrd Power Cell
            public int FrozenKeyItem = 1572;                                // |Frost Hydra Staff
            public int CrimsonKeyItem = 1569;                               // |Vampire Knifes
            public int HallowedKeyItem = 1260;                              // |Rainbow Gun
            public int CorruptionKeyItem = 1571;                            // |Scourge of the Corruptor

            public string JungleItemName = "Piranha Gun";                   // |Jungle Item Name
            public string TempleItemName = "Lihzahrd Power Cell";           // |Temple Item Name
            public string FrozenItemName = "Staff of the Frost Hydra";      // |Frost Item Name
            public string CrimsonItemName = "Vampire Knifes";               // |Crimson Item Name
            public string HallowedItemName = "Rainbow Gun";                 // |Hallowed Item Name
            public string CorruptionItemName = "Scourge of the Corruptor";  // |Jungle Item Name

            // Those are optional; They're only needed if EnableRegionEnchanges is set to true. Default is set to null, so that players can be informed of non-existing regions.
            public string MarketRegion = null;
            public string JungleRegion = null;
            public string TempleRegion = null;
            public string CrimsonRegion = null;
            public string FrozenRegion = null;
            public string HallowedRegion = null;
            public string CorruptionRegion = null;
            
        }

        //Creates the Config
        private static void CreateConfig()
        {
            string filepath = Path.Combine(savepath, "KeyChangerConfig.json");
            try
            {
                using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    using (var sr = new StreamWriter(stream))
                    {
                        config = new Config();
                        var configString = JsonConvert.SerializeObject(config, Formatting.Indented);
                        sr.Write(configString);
                    }
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
                config = new Config();
            }
        }

        //Reads the Config
        private static bool ReadConfig()
        {
            string filepath = Path.Combine(savepath, "KeyChangerConfig.json");
            try
            {
                if (File.Exists(filepath))
                {
                    using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var configString = sr.ReadToEnd();
                            config = JsonConvert.DeserializeObject<Config>(configString);
                        }
                        stream.Close();
                    }
                    return true;
                }
                else
                {
                    Log.ConsoleError("KeyChanger config not found. Creating new one...");
                    CreateConfig();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
            }
            return false;
        }

        private static void KeyChange(CommandArgs args)
        {
            TSPlayer ply = args.Player;

            if (args.Parameters.Count == 0)
            {
                // Plugin Info
                ply.SendMessage("KeyChanger plugin by Enerdy", Color.Goldenrod);
                ply.SendMessage("Description: Changes special chest keys into their specific items", Color.Goldenrod);
                ply.SendMessage("Syntax: /key <change/reload> [type]", Color.Goldenrod);
                ply.SendMessage("Available types: " + KeyTypes, Color.Goldenrod);
                ply.SendMessage("Type /key help for more info", Color.Goldenrod);
            }
            else if (args.Parameters[0].ToLower() == "change" && args.Parameters.Count == 1)
            {
                ply.SendErrorMessage("Invalid syntax! Proper syntax: /key change <type>");
            }
            else if (args.Parameters[0].ToLower() == "change" || args.Parameters[0].ToLower() == "reload" || args.Parameters[0].ToLower() == "help")
            {
                string cmd = args.Parameters[0].ToLower();
                switch (cmd)
                {
                    case "change":
                        {
                            // Required values
                            string keyType = args.Parameters[1].ToLower();
                            int keyID = 0;
                            int keyItem = 0;
                            bool keyGiven = false;

                            #region Jungle
                            if (keyType == "jungle")
                            {
                                keyItem = config.JungleKeyItem;
                                keyID = 1533;

                                if (!config.EnableJungleKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                if (config.EnableRegionEnchanges && config.TempleRegion != null)
                                {
                                    Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
                                    Region jungleregion = TShock.Regions.GetRegionByName(config.JungleRegion);

                                    if (config.MarketOnly)
                                    {
                                        if (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height)
                                        {
                                            // Keep running the code 
                                        }
                                        else
                                        {
                                            ply.SendErrorMessage("You're not inside the required region!");
                                            break;
                                        }
                                    }

                                    if ((ply.TileX >= jungleregion.Area.X && ply.TileX <= jungleregion.Area.X + jungleregion.Area.Width && ply.TileY >= jungleregion.Area.Y && ply.TileY <= jungleregion.Area.Y + jungleregion.Area.Height) || (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height))
                                    {
                                        // Keep running the code 
                                    }
                                    else
                                    {
                                        ply.SendErrorMessage("You're not inside the required region!");
                                        break;
                                    }
                                }
                                else if (config.EnableRegionEnchanges)
                                {
                                    ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                    break;
                                }

                                // Loops between inventory slots in search of the key. If it's not found, it returns the error message, since the if statement is never executed, and the command never broken.
                                for (int i = 0; i < 50; i++)
                                {
                                    if (ply.InventorySlotAvailable && args.TPlayer.inventory[i].netID == keyID)
                                    {
                                        ply.SaveServerCharacter();
                                        ply.PlayerData.inventory[i].stack -= 1;
                                        ply.SendServerCharacter();
                                        ply.GiveItem(keyItem, "", 0, 0, 1);
                                        keyGiven = true;
                                        break;
                                    }
                                }
                                if (keyGiven)
                                {
                                    ply.SendMessage("Enchanged 1 " + config.JungleItemName + "(s)!", Color.Goldenrod);
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Enchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Temple
                            else if (keyType == "temple")
                            {
                                keyItem = config.TempleKeyItem;
                                keyID = 1141;

                                if (!config.EnableTempleKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                if (config.EnableRegionEnchanges && config.TempleRegion != null)
                                {
                                    Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
                                    Region templeregion = TShock.Regions.GetRegionByName(config.TempleRegion);

                                    if (config.MarketOnly)
                                    {
                                        if (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height)
                                        {
                                            // Keep running the code 
                                        }
                                        else
                                        {
                                            ply.SendErrorMessage("You're not inside the required region!");
                                            break;
                                        }
                                    }

                                    if ((ply.TileX >= templeregion.Area.X && ply.TileX <= templeregion.Area.X + templeregion.Area.Width && ply.TileY >= templeregion.Area.Y && ply.TileY <= templeregion.Area.Y + templeregion.Area.Height) || (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height))
                                    {
                                        // Keep running the code 
                                    }
                                    else
                                    {
                                        ply.SendErrorMessage("You're not inside the required region!");
                                        break;
                                    }
                                }
                                else if (config.EnableRegionEnchanges)
                                {
                                    ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                    break;
                                }

                                for (int i = 0; i < 50; i++)
                                {
                                    if (ply.InventorySlotAvailable && args.TPlayer.inventory[i].netID == keyID)
                                    {
                                        ply.SaveServerCharacter();
                                        ply.PlayerData.inventory[i].stack -= 1;
                                        ply.SendServerCharacter();
                                        ply.GiveItem(keyItem, "", 0, 0, 1);
                                        keyGiven = true;
                                        break;
                                    }
                                }
                                if (keyGiven)
                                {
                                    ply.SendMessage("Enchanged 1 " + config.TempleItemName + "(s)!", Color.Goldenrod);
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Enchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Crimson
                            else if (keyType == "crimson")
                            {
                                keyItem = config.CrimsonKeyItem;
                                keyID = 1535;

                                if (!config.EnableCrimsonKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                if (config.EnableRegionEnchanges && config.CrimsonRegion != null)
                                {
                                    Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
                                    Region crimsonregion = TShock.Regions.GetRegionByName(config.CrimsonRegion);

                                    if (config.MarketOnly)
                                    {
                                        if (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height)
                                        {
                                            // Keep running the code 
                                        }
                                        else
                                        {
                                            ply.SendErrorMessage("You're not inside the required region!");
                                            break;
                                        }
                                    }

                                    if ((ply.TileX >= crimsonregion.Area.X && ply.TileX <= crimsonregion.Area.X + crimsonregion.Area.Width && ply.TileY >= crimsonregion.Area.Y && ply.TileY <= crimsonregion.Area.Y + crimsonregion.Area.Height) || (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height))
                                    {
                                        // Keep running the code 
                                    }
                                    else
                                    {
                                        ply.SendErrorMessage("You're not inside the required region!");
                                        break;
                                    }
                                }
                                else if (config.EnableRegionEnchanges)
                                {
                                    ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                    break;
                                }

                                for (int i = 0; i < 50; i++)
                                {
                                    if (ply.InventorySlotAvailable && args.TPlayer.inventory[i].netID == keyID)
                                    {
                                        ply.SaveServerCharacter();
                                        ply.PlayerData.inventory[i].stack -= 1;
                                        ply.SendServerCharacter();
                                        ply.GiveItem(keyItem, "", 0, 0, 1);
                                        keyGiven = true;
                                        break;
                                    }
                                }
                                if (keyGiven)
                                {
                                    ply.SendMessage("Enchanged 1 " + config.CrimsonItemName + "(s)!", Color.Goldenrod);
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Enchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Frozen
                            else if (keyType == "frozen")
                            {
                                keyItem = config.FrozenKeyItem;
                                keyID = 1537;

                                if (!config.EnableFrozenKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                if (config.EnableRegionEnchanges && config.FrozenRegion != null)
                                {
                                    Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
                                    Region frozenregion = TShock.Regions.GetRegionByName(config.FrozenRegion);

                                    if (config.MarketOnly)
                                    {
                                        if (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height)
                                        {
                                            // Keep running the code 
                                        }
                                        else
                                        {
                                            ply.SendErrorMessage("You're not inside the required region!");
                                            break;
                                        }
                                    }

                                    if ((ply.TileX >= frozenregion.Area.X && ply.TileX <= frozenregion.Area.X + frozenregion.Area.Width && ply.TileY >= frozenregion.Area.Y && ply.TileY <= frozenregion.Area.Y + frozenregion.Area.Height) || (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height))
                                    {
                                        // Keep running the code 
                                    }
                                    else
                                    {
                                        ply.SendErrorMessage("You're not inside the required region!");
                                        break;
                                    }
                                }
                                else if (config.EnableRegionEnchanges)
                                {
                                    ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                    break;
                                }

                                for (int i = 0; i < 50; i++)
                                {
                                    if (ply.InventorySlotAvailable && args.TPlayer.inventory[i].netID == keyID)
                                    {
                                        ply.SaveServerCharacter();
                                        ply.PlayerData.inventory[i].stack -= 1;
                                        ply.SendServerCharacter();
                                        ply.GiveItem(keyItem, "", 0, 0, 1);
                                        keyGiven = true;
                                        break;
                                    }
                                }
                                if (keyGiven)
                                {
                                    ply.SendMessage("Enchanged 1 " + config.FrozenItemName + "(s)!", Color.Goldenrod);
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Enchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Hallowed
                            else if (keyType == "hallowed")
                            {
                                keyItem = config.HallowedKeyItem;
                                keyID = 1536;

                                if (!config.EnableHallowedKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                if (config.EnableRegionEnchanges && config.HallowedRegion != null)
                                {
                                    Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
                                    Region hallowedregion = TShock.Regions.GetRegionByName(config.HallowedRegion);

                                    if (config.MarketOnly)
                                    {
                                        if (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height)
                                        {
                                            // Keep running the code 
                                        }
                                        else
                                        {
                                            ply.SendErrorMessage("You're not inside the required region!");
                                            break;
                                        }
                                    }

                                    if ((ply.TileX >= hallowedregion.Area.X && ply.TileX <= hallowedregion.Area.X + hallowedregion.Area.Width && ply.TileY >= hallowedregion.Area.Y && ply.TileY <= hallowedregion.Area.Y + hallowedregion.Area.Height) || (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height))
                                    {
                                        // Keep running the code 
                                    }
                                    else
                                    {
                                        ply.SendErrorMessage("You're not inside the required region!");
                                        break;
                                    }
                                }
                                else if (config.EnableRegionEnchanges)
                                {
                                    ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                    break;
                                }

                                for (int i = 0; i < 50; i++)
                                {
                                    if (ply.InventorySlotAvailable && args.TPlayer.inventory[i].netID == keyID)
                                    {
                                        ply.SaveServerCharacter();
                                        ply.PlayerData.inventory[i].stack -= 1;
                                        ply.SendServerCharacter();
                                        ply.GiveItem(keyItem, "", 0, 0, 1);
                                        keyGiven = true;
                                        break;
                                    }
                                }
                                if (keyGiven)
                                {
                                    ply.SendMessage("Enchanged 1 " + config.HallowedItemName + "(s)!", Color.Goldenrod);
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Enchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Corruption
                            else if (keyType == "corruption")
                            {
                                keyItem = config.CorruptionKeyItem;
                                keyID = 1534;

                                if (!config.EnableCorruptionKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                if (config.EnableRegionEnchanges && config.CorruptionRegion != null)
                                {
                                    Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
                                    Region corruptionregion = TShock.Regions.GetRegionByName(config.CorruptionRegion);

                                    if (config.MarketOnly)
                                    {
                                        if (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height)
                                        {
                                            // Keep running the code 
                                        }
                                        else
                                        {
                                            ply.SendErrorMessage("You're not inside the required region!");
                                            break;
                                        }
                                    }

                                    if ((ply.TileX >= corruptionregion.Area.X && ply.TileX <= corruptionregion.Area.X + corruptionregion.Area.Width && ply.TileY >= corruptionregion.Area.Y && ply.TileY <= corruptionregion.Area.Y + corruptionregion.Area.Height) || (ply.TileX >= marketregion.Area.X && ply.TileX <= marketregion.Area.X + marketregion.Area.Width && ply.TileY >= marketregion.Area.Y && ply.TileY <= marketregion.Area.Y + marketregion.Area.Height))
                                    {
                                        // Keep running the code 
                                    }
                                    else
                                    {
                                        ply.SendErrorMessage("You're not inside the required region!");
                                        break;
                                    }
                                }
                                else if (config.EnableRegionEnchanges)
                                {
                                    ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                    break;
                                }

                                for (int i = 0; i < 50; i++)
                                {
                                    if (ply.InventorySlotAvailable && args.TPlayer.inventory[i].netID == keyID)
                                    {
                                        ply.SaveServerCharacter();
                                        ply.PlayerData.inventory[i].stack -= 1;
                                        ply.SendServerCharacter();
                                        ply.GiveItem(keyItem, "", 0, 0, 1);
                                        keyGiven = true;
                                        break;
                                    }
                                }
                                if (keyGiven)
                                {
                                    ply.SendMessage("Enchanged 1 " + config.CorruptionItemName + "(s)!", Color.Goldenrod);
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Enchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            else
                            {
                                ply.SendErrorMessage(string.Format("Invalid type! Available types: {0}", KeyTypes));
                                break;
                            }

                            #region Scrapped Code
                            // Some code copied from AutoShop which ended up being scrapped. Might remove it later...

                            //    ply.SendMessage(string.Format("Key type set to {0}! Waiting for the key to drop nearby...", keyType), Color.Goldenrod);
                        //    TimeSpan time = new TimeSpan(0, 0, 10);

                        //    {
                        //        for (int j = 0; j < 200; j++)
                        //        {
                        //            if (
                        //(Math.Sqrt(Math.Pow(Main.item[j].position.X - ply.X, 2) +
                        //           Math.Pow(Main.item[j].position.Y - ply.Y, 2)) < 7 * 16) && (Main.item[j].active))
                        //            //if (Main.item[i].position.X - com
                        //            {
                        //                Main.item[j].active = false;
                        //                NetMessage.SendData(0x15, -1, -1, "", j, 0f, 0f, 0f, 0);
                        //                gotKey = true;
                        //                break; //found the item, break.

                        //            } //end of if item

                        //        } //end for loop

                                
                        //    }

                        //    if (gotKey)
                        //    {
                        //        args.Player.GiveItem(keyItem, itemName, 0, 0, 1);
                        //        ply.SendMessage(string.Format("Enchange complete! Received 1 {0}!", itemName), Color.Goldenrod);
                        //        timeout = true;
                        //        break;
                        //    }
                        //    else if (timeout)
                        //    {
                        //        break;
                            //    }
                            #endregion

                        }

                    case "reload":
                        {
                            if (!ply.Group.HasPermission("key.reload"))
                            {
                                ply.SendErrorMessage("You do not have access to this command.");
                                break;
                            }

                            if (ReadConfig())
                            {
                                ply.SendMessage("KeyChange config reloaded sucessfully.", Color.Goldenrod);
                                break;
                            }
                            else
                            {
                                ply.SendErrorMessage("KeyChange config reloaded unsucessfully. Check logs for details.");
                                break;
                            }
                        }

                    case "help":
                        {
                            ply.SendInfoMessage("KeyChanger Help File");
                            ply.SendInfoMessage("With this plugin, you may trade your special keys in enchange for special items defined in the config file. Default items are set based off the correspondent Dungeon Chest's contents.");
                            ply.SendInfoMessage("How to use: Firstly, make sure you have the key you plan on enchanging in your inventory, and that you have free inventory slots. Then, type /key change <key type> in order to enchange the key in.");
                            break;
                        }

                    default:
                        {
                            ply.SendErrorMessage("Invalid syntax! Proper syntax: /key <help/change/reload> [type]");
                            break;
                        }
                }
            }
            else
            {
                ply.SendErrorMessage("Invalid syntax! Proper syntax: /key <help/change/reload> [type]");
            }
        }
    }
}