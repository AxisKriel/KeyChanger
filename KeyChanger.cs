using System;
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
        private static string savepath = TShock.SavePath;
        private static Config config;
        //private static string[] KeyTypes = new string[] { "jungle", "temple", "crimson", "frozen", "hallowed", "corruption" };
        public static Random rand = new Random();
        //public static List<int> goldItem = new List<int>();             // |List for Gold Items
        public Region marketregion = new Region();                      // |All allowed
        public Region jungleregion = new Region();                      // |Jungle allowed
        public Region templeregion = new Region();                      // |Temple allowed
        public Region crimsonregion = new Region();                     // |Crimson Allowed
        public Region frozenregion = new Region();                      // |Frozen Allowed
        public Region hallowedregion = new Region();                    // |Hallowed Allowed
        public Region corruptionregion = new Region();                  // |Corruption Allowed

        public static List<string> jungleitems = new List<string>();
        public static List<string> templeitems = new List<string>();
        public static List<string> crimsonitems = new List<string>();
        public static List<string> frozenitems = new List<string>();
        public static List<string> halloweditems = new List<string>();
        public static List<string> corruptionitems = new List<string>();

        public override Version Version
        {
            get { return new Version("1.3"); }
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
            get { return "SBPlanet KeyChanger System: Exchanges special chest keys by their correspondent items"; }
        }


        public override void Initialize()
        {
            //This is the main command, which branches to everything the plugin can do, by checking the first parameter a player inputs
            Commands.ChatCommands.Add(new Command("key.change", KeyChange, "key"));
            ReadConfig();
            IdToName();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public static void IdToName()
        {
            for (int i = 0; i < config.JungleKeyItem.Length; i++)
            {
                if (!jungleitems.Contains(TShock.Utils.GetItemById(config.JungleKeyItem[i]).name))
                {
                    jungleitems.Add(TShock.Utils.GetItemById(config.JungleKeyItem[i]).name);
                }
            }
            for (int i = 0; i < config.TempleKeyItem.Length; i++)
            {
                if (!templeitems.Contains(TShock.Utils.GetItemById(config.TempleKeyItem[i]).name))
                {
                    templeitems.Add(TShock.Utils.GetItemById(config.TempleKeyItem[i]).name);
                }
            }
            for (int i = 0; i < config.CrimsonKeyItem.Length; i++)
            {
                if (!crimsonitems.Contains(TShock.Utils.GetItemById(config.CrimsonKeyItem[i]).name))
                {
                    crimsonitems.Add(TShock.Utils.GetItemById(config.CrimsonKeyItem[i]).name);
                }
            }
            for (int i = 0; i < config.FrozenKeyItem.Length; i++)
            {
                if (!frozenitems.Contains(TShock.Utils.GetItemById(config.FrozenKeyItem[i]).name))
                {
                    frozenitems.Add(TShock.Utils.GetItemById(config.FrozenKeyItem[i]).name);
                }
            }
            for (int i = 0; i < config.HallowedKeyItem.Length; i++)
            {
                if (!halloweditems.Contains(TShock.Utils.GetItemById(config.HallowedKeyItem[i]).name))
                {
                    halloweditems.Add(TShock.Utils.GetItemById(config.HallowedKeyItem[i]).name);
                }
            }
            for (int i = 0; i < config.CorruptionKeyItem.Length; i++)
            {
                if (!corruptionitems.Contains(TShock.Utils.GetItemById(config.CorruptionKeyItem[i]).name))
                {
                    corruptionitems.Add(TShock.Utils.GetItemById(config.CorruptionKeyItem[i]).name);
                }
            }
        }

        //Config Contents
        class Config
        {
            public bool EnableRegionExchanges = false;                      // |Default set to false
            public bool MarketMode = false;                                 // |Use only a general market region
            //public bool EnableGoldKey = true;                               // |Gold Key works similar to a currency
            public bool EnableJungleKey = true;
            public bool EnableTempleKey = true;
            public bool EnableFrozenKey = true;
            public bool EnableCrimsonKey = true;
            public bool EnableHallowedKey = true;
            public bool EnableCorruptionKey = true;
 
            public int[] JungleKeyItem = new int[] { 1156 };                // |Piranha Gun
            public int[] TempleKeyItem = new int[] { 1293 };                // |Lihzahrd Power Cell
            public int[] FrozenKeyItem = new int[] { 1572 };                // |Frost Hydra Staff
            public int[] CrimsonKeyItem = new int[] { 1569 };               // |Vampire Knifes
            public int[] HallowedKeyItem = new int[] { 1260 };              // |Rainbow Gun
            public int[] CorruptionKeyItem = new int[] { 1571 };            // |Scourge of the Corruptor

            // Those are optional; They're only needed if EnableRegionExchanges is set to true. Default is set to null, so that players can be informed of non-existing regions.
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
                ply.SendMessage("KeyChanger (v1.3) by Enerdy", Color.SkyBlue);
                ply.SendMessage("Description: Changes special chest keys into their specific items", Color.SkyBlue);
                ply.SendMessage("Syntax: /key <help/list/mode/change/reload> [type]", Color.SkyBlue);
                ply.SendMessage("Type /key help for more info", Color.SkyBlue);
            }
            else if (args.Parameters[0].ToLower() == "change" && args.Parameters.Count == 1)
            {
                ply.SendErrorMessage("Invalid syntax! Proper syntax: /key change <type>");
            }
            else if (args.Parameters.Count >= 1)
            {
                string cmd = args.Parameters[0].ToLower();
                switch (cmd)
                {
                    case "change":
                        {
                            // Required values
                            string keyType = args.Parameters[1].ToLower();
                            string itemName = "";
                            int keyID = 0;
                            int keyItem = 0;
                            bool keyGiven = false;

                            #region Jungle
                            if (keyType == "jungle")
                            {
                                keyItem = config.JungleKeyItem[rand.Next(0, config.JungleKeyItem.Length)];
                                keyID = 1533;
                                itemName = TShock.Utils.GetItemById(keyItem).name;

                                if (!config.EnableJungleKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                #region EnableRegionExchanges
                                if (config.EnableRegionExchanges)
                                {
                                    if (config.MarketMode)
                                    {
                                        if (config.MarketRegion != null)
                                        {
                                            Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
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
                                        else if (config.MarketRegion == null)
                                        {
                                            ply.SendErrorMessage("KeyChanger is disabled because Market Mode is enabled but the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                    else if (!config.MarketMode)
                                    {
                                        if (config.JungleRegion != null)
                                        {
                                            Region jungleregion = TShock.Regions.GetRegionByName(config.JungleRegion);
                                            if (ply.TileX >= jungleregion.Area.X && ply.TileX <= jungleregion.Area.X + jungleregion.Area.Width && ply.TileY >= jungleregion.Area.Y && ply.TileY <= jungleregion.Area.Y + jungleregion.Area.Height)
                                            {
                                                // Keep running the code 
                                            }
                                            else
                                            {
                                                ply.SendErrorMessage("You're not inside the required region!");
                                                break;
                                            }
                                        }
                                        else if (config.JungleRegion == null)
                                        {
                                            ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                }
                                #endregion

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
                                    ply.SendSuccessMessage("Exchanged 1 " + itemName + "(s)!");
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Exchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Temple
                            else if (keyType == "temple")
                            {
                                keyItem = config.TempleKeyItem[rand.Next(0, config.TempleKeyItem.Length)];
                                keyID = 1141;
                                itemName = TShock.Utils.GetItemById(keyItem).name;

                                if (!config.EnableTempleKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                #region EnableRegionExchanges
                                if (config.EnableRegionExchanges)
                                {
                                    if (config.MarketMode)
                                    {
                                        if (config.MarketRegion != null)
                                        {
                                            Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
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
                                        else if (config.MarketRegion == null)
                                        {
                                            ply.SendErrorMessage("KeyChanger is disabled because Market Mode is enabled but the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                    else if (!config.MarketMode)
                                    {
                                        if (config.TempleRegion != null)
                                        {
                                            Region templeregion = TShock.Regions.GetRegionByName(config.TempleRegion);
                                            if (ply.TileX >= templeregion.Area.X && ply.TileX <= templeregion.Area.X + templeregion.Area.Width && ply.TileY >= templeregion.Area.Y && ply.TileY <= templeregion.Area.Y + templeregion.Area.Height)
                                            {
                                                // Keep running the code 
                                            }
                                            else
                                            {
                                                ply.SendErrorMessage("You're not inside the required region!");
                                                break;
                                            }
                                        }
                                        else if (config.TempleRegion == null)
                                        {
                                            ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                }
                                #endregion

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
                                    ply.SendSuccessMessage("Exchanged 1 " + itemName + "(s)!");
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Exchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Crimson
                            else if (keyType == "crimson")
                            {
                                keyItem = config.CrimsonKeyItem[rand.Next(0, config.CrimsonKeyItem.Length)];
                                keyID = 1535;
                                itemName = TShock.Utils.GetItemById(keyItem).name;

                                if (!config.EnableCrimsonKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                #region EnableRegionExchanges
                                if (config.EnableRegionExchanges)
                                {
                                    if (config.MarketMode)
                                    {
                                        if (config.MarketRegion != null)
                                        {
                                            Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
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
                                        else if (config.MarketRegion == null)
                                        {
                                            ply.SendErrorMessage("KeyChanger is disabled because Market Mode is enabled but the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                    else if (!config.MarketMode)
                                    {
                                        if (config.CrimsonRegion != null)
                                        {
                                            Region crimsonregion = TShock.Regions.GetRegionByName(config.CrimsonRegion);
                                            if (ply.TileX >= crimsonregion.Area.X && ply.TileX <= crimsonregion.Area.X + crimsonregion.Area.Width && ply.TileY >= crimsonregion.Area.Y && ply.TileY <= crimsonregion.Area.Y + crimsonregion.Area.Height)
                                            {
                                                // Keep running the code 
                                            }
                                            else
                                            {
                                                ply.SendErrorMessage("You're not inside the required region!");
                                                break;
                                            }
                                        }
                                        else if (config.CrimsonRegion == null)
                                        {
                                            ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                }
                                #endregion

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
                                    ply.SendSuccessMessage("Exchanged 1 " + itemName + "(s)!");
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Exchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Frozen
                            else if (keyType == "frozen")
                            {
                                keyItem = config.FrozenKeyItem[rand.Next(0, config.FrozenKeyItem.Length)];
                                keyID = 1537;
                                itemName = TShock.Utils.GetItemById(keyItem).name;

                                if (!config.EnableFrozenKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                #region EnableRegionExchanges
                                if (config.EnableRegionExchanges)
                                {
                                    if (config.MarketMode)
                                    {
                                        if (config.MarketRegion != null)
                                        {
                                            Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
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
                                        else if (config.MarketRegion == null)
                                        {
                                            ply.SendErrorMessage("KeyChanger is disabled because Market Mode is enabled but the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                    else if (!config.MarketMode)
                                    {
                                        if (config.FrozenRegion != null)
                                        {
                                            Region frozenregion = TShock.Regions.GetRegionByName(config.FrozenRegion);
                                            if (ply.TileX >= frozenregion.Area.X && ply.TileX <= frozenregion.Area.X + frozenregion.Area.Width && ply.TileY >= frozenregion.Area.Y && ply.TileY <= frozenregion.Area.Y + frozenregion.Area.Height)
                                            {
                                                // Keep running the code 
                                            }
                                            else
                                            {
                                                ply.SendErrorMessage("You're not inside the required region!");
                                                break;
                                            }
                                        }
                                        else if (config.FrozenRegion == null)
                                        {
                                            ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                }
                                #endregion

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
                                    ply.SendSuccessMessage("Exchanged 1 " + itemName + "(s)!");
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Exchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Hallowed
                            else if (keyType == "hallowed")
                            {
                                keyItem = config.HallowedKeyItem[rand.Next(0, config.HallowedKeyItem.Length)];
                                keyID = 1536;
                                itemName = TShock.Utils.GetItemById(keyItem).name;

                                if (!config.EnableHallowedKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                #region EnableRegionExchanges
                                if (config.EnableRegionExchanges)
                                {
                                    if (config.MarketMode)
                                    {
                                        if (config.MarketRegion != null)
                                        {
                                            Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
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
                                        else if (config.MarketRegion == null)
                                        {
                                            ply.SendErrorMessage("KeyChanger is disabled because Market Mode is enabled but the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                    else if (!config.MarketMode)
                                    {
                                        if (config.HallowedRegion != null)
                                        {
                                            Region hallowedregion = TShock.Regions.GetRegionByName(config.HallowedRegion);
                                            if (ply.TileX >= hallowedregion.Area.X && ply.TileX <= hallowedregion.Area.X + hallowedregion.Area.Width && ply.TileY >= hallowedregion.Area.Y && ply.TileY <= hallowedregion.Area.Y + hallowedregion.Area.Height)
                                            {
                                                // Keep running the code 
                                            }
                                            else
                                            {
                                                ply.SendErrorMessage("You're not inside the required region!");
                                                break;
                                            }
                                        }
                                        else if (config.HallowedRegion == null)
                                        {
                                            ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                }
                                #endregion

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
                                    ply.SendSuccessMessage("Exchanged 1 " + itemName + "(s)!");
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Exchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            #region Corruption
                            else if (keyType == "corruption")
                            {
                                keyItem = config.CorruptionKeyItem[rand.Next(0, config.CorruptionKeyItem.Length)];
                                keyID = 1534;
                                itemName = TShock.Utils.GetItemById(keyItem).name;

                                if (!config.EnableCorruptionKey)
                                {
                                    ply.SendErrorMessage("This key type is disabled.");
                                    break;
                                }

                                #region EnableRegionExchanges
                                if (config.EnableRegionExchanges)
                                {
                                    if (config.MarketMode)
                                    {
                                        if (config.MarketRegion != null)
                                        {
                                            Region marketregion = TShock.Regions.GetRegionByName(config.MarketRegion);
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
                                        else if (config.MarketRegion == null)
                                        {
                                            ply.SendErrorMessage("KeyChanger is disabled because Market Mode is enabled but the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                    else if (!config.MarketMode)
                                    {
                                        if (config.CorruptionRegion != null)
                                        {
                                            Region corruptionregion = TShock.Regions.GetRegionByName(config.CorruptionRegion);
                                            if (ply.TileX >= corruptionregion.Area.X && ply.TileX <= corruptionregion.Area.X + corruptionregion.Area.Width && ply.TileY >= corruptionregion.Area.Y && ply.TileY <= corruptionregion.Area.Y + corruptionregion.Area.Height)
                                            {
                                                // Keep running the code 
                                            }
                                            else
                                            {
                                                ply.SendErrorMessage("You're not inside the required region!");
                                                break;
                                            }
                                        }
                                        else if (config.CorruptionRegion == null)
                                        {
                                            ply.SendErrorMessage("This key type is disabled because the required region doesn't exist.");
                                            break;
                                        }
                                    }
                                }
                                #endregion

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
                                    ply.SendSuccessMessage("Exchanged 1 " + itemName + "(s)!");
                                    break;
                                }
                                else
                                {
                                    ply.SendErrorMessage("Exchange failed: Key not found / No free slots");
                                    break;
                                }
                            }
                            #endregion
                            else
                            {
                                ply.SendErrorMessage("Invalid syntax! Proper syntax: /key change <type>");
                                break;
                            }
                        }

                    case "reload":
                        {
                            if (!ply.Group.HasPermission("key.reload"))
                            {
                                ply.SendErrorMessage("You do not have access to this command.");
                                break;
                            }

                            IdToName();
                            if (ReadConfig())
                            {
                                ply.SendMessage("KeyChanger config reloaded sucessfully.", Color.Green);
                                break;
                            }
                            else
                            {
                                ply.SendErrorMessage("KeyChanger config reloaded unsucessfully. Check logs for details.");
                                break;
                            }
                        }

                    case "help":
                        {
                            ply.SendInfoMessage("KeyChanger Help File");
                            ply.SendInfoMessage("/key - Shows plugin info");
                            ply.SendInfoMessage("/key change <type> - Exchanges a key of the input type");
                            ply.SendInfoMessage("/key list - Shows a list of available keys and items");
                            ply.SendInfoMessage("/key mode <mode> - Changes exchange mode");
                            ply.SendInfoMessage("/key reload - Reloads the config file");
                            ply.SendInfoMessage("If an exchange fails, make sure your inventory has free slots");
                            break;
                        }

                    case "list":
                        {
                            ply.SendMessage("jungle key - " + string.Join(", ", jungleitems), Color.Goldenrod);
                            ply.SendMessage("temple key - " + string.Join(", ", templeitems), Color.Goldenrod);
                            ply.SendMessage("crimson key - " + string.Join(", ", crimsonitems), Color.Goldenrod);
                            ply.SendMessage("frozen key - " + string.Join(", ", frozenitems), Color.Goldenrod);
                            ply.SendMessage("hallowed key - " + string.Join(", ", halloweditems), Color.Goldenrod);
                            ply.SendMessage("corruption key - " + string.Join(", ", corruptionitems), Color.Goldenrod);
                            break;
                        }

                    case "mode":
                        {
                            if (args.Parameters.Count < 2)
                            {
                                ply.SendErrorMessage("Invalid syntax! Proper syntax: /key mode <normal/region/market>");
                                break;
                            }
                            if (args.Parameters[1] == "normal")
                            {
                                config.EnableRegionExchanges = false;
                                ply.SendSuccessMessage("Exchange mode set to normal (exchange everywhere).");
                                break;
                            }
                            else if (args.Parameters[1] == "region")
                            {
                                config.EnableRegionExchanges = true;
                                config.MarketMode = false;
                                ply.SendSuccessMessage("Exchange mode set to region (a region for each type).");
                                break;
                            }
                            else if (args.Parameters[1] == "market")
                            {
                                config.EnableRegionExchanges = true;
                                config.MarketMode = true;
                                ply.SendSuccessMessage("Exchange mode set to market (one region for every type).");
                                break;
                            }
                            else
                            {
                                ply.SendErrorMessage("Invalid syntax! Proper syntax: /key mode <normal/region/market>");
                                break;
                            }
                        }
                    default:
                        {
                            ply.SendErrorMessage("Invalid syntax! Proper syntax: /key <help/list/mode/change/reload> [type]");
                            break;
                        }
                }
            }
            else
            {
                ply.SendErrorMessage("Invalid syntax! Proper syntax: /key <help/list/mode/change/reload> [type]");
            }
        }
    }
}
