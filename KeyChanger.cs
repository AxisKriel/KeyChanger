using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace KeyChanger
{
	[ApiVersion(1, 23)]
	public class KeyChanger : TerrariaPlugin
	{
		public override string Author => "Enerdy";

		public static Config Config { get; private set; }

		public override string Description => "SBPlanet KeyChanger System: Exchanges special chest keys by their correspondent items.";

		public override string Name => "KeyChanger";

		public override Version Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

		public KeyChanger(Main game) : base(game) { }

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GameInitialize.Deregister(this, onInitialize);
			}
		}

		public override void Initialize()
		{
			ServerApi.Hooks.GameInitialize.Register(this, onInitialize);
		}

		private void onInitialize(EventArgs e)
		{
			Config = Config.Read();

			//This is the main command, which branches to everything the plugin can do, by checking the first parameter a player inputs
			Commands.ChatCommands.Add(new Command(new List<string>() { "key.change", "key.reload", "key.mode" }, KeyChange, "key")
			{
				HelpDesc = new[]
				{
					$"{Commands.Specifier}key - Shows plugin info.",
					$"{Commands.Specifier}key change <type> - Exchanges a key of the input type.",
					$"{Commands.Specifier}key list - Shows a list of available keys and items.",
					$"{Commands.Specifier}key mode <mode> - Changes exchange mode.",
					$"{Commands.Specifier}key reload - Reloads the config file.",
					"If an exchange fails, make sure your inventory has free slots."
				}
			});

			Utils.InitKeys();
		}

		private void KeyChange(CommandArgs args)
		{
			TSPlayer ply = args.Player;

			// SSC check to alert users
			if (!Main.ServerSideCharacter)
			{
				ply.SendWarningMessage("[Warning] This plugin will not work properly with ServerSideCharacters disabled.");
			}

			if (args.Parameters.Count < 1)
			{
				// Plugin Info
				var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
				ply.SendMessage($"KeyChanger (v{version}) by Enerdy", Color.SkyBlue);
				ply.SendMessage("Description: Changes special chest keys into their specific items", Color.SkyBlue);
				ply.SendMessage($"Syntax: {Commands.Specifier}key <list/mode/change/reload> [type]", Color.SkyBlue);
				ply.SendMessage($"Type {Commands.Specifier}help key for more info", Color.SkyBlue);
			}
			else if (args.Parameters[0].ToLower() == "change" && args.Parameters.Count == 1)
			{
				ply.SendErrorMessage("Invalid syntax! Proper syntax: {0}key change <type>", Commands.Specifier);
			}
			else if (args.Parameters.Count > 0)
			{
				string cmd = args.Parameters[0].ToLower();
				switch (cmd)
				{
					case "change":
						if (!ply.Group.HasPermission("key.change"))
						{
							ply.SendErrorMessage("You do not have access to this command.");
							break;
						}

						// Prevents cast from the server console
						if (ply == TSPlayer.Server)
						{
							ply.SendErrorMessage("You must use this command in-game.");
							return;
						}

						Key key;
						string str = args.Parameters[1].ToLower();

						if (str == Key.Temple.Name)
							key = Key.Temple;
						else if (str == Key.Jungle.Name)
							key = Key.Jungle;
						else if (str == Key.Corruption.Name)
							key = Key.Corruption;
						else if (str == Key.Crimson.Name)
							key = Key.Crimson;
						else if (str == Key.Hallowed.Name)
							key = Key.Hallowed;
						else if (str == Key.Frozen.Name)
							key = Key.Frozen;
						else
						{
							ply.SendErrorMessage("Invalid key type! Available types: " + String.Join(", ",
								Key.Temple.Enabled ? Key.Temple.Name : null,
								Key.Jungle.Enabled ? Key.Jungle.Name : null,
								Key.Corruption.Enabled ? Key.Corruption.Name : null,
								Key.Crimson.Enabled ? Key.Crimson.Name : null,
								Key.Hallowed.Enabled ? Key.Hallowed.Name : null,
								Key.Frozen.Enabled ? Key.Frozen.Name : null));
							return;
						}

						// Verifies whether the key has been enabled
						if (!key.Enabled)
						{
							ply.SendInfoMessage("The selected key is disabled.");
							return;
						}

						// Checks if the player carries the necessary key
						var lookup = ply.TPlayer.inventory.FirstOrDefault(i => i.netID == (int)key.Type);
						if (lookup == null)
						{
							ply.SendErrorMessage("Make sure you carry the selected key in your inventory.");
							return;
						}

						if (Config.EnableRegionExchanges)
						{
							Region region;
							if (Config.MarketMode)
								region = TShock.Regions.GetRegionByName(Config.MarketRegion);
							else
								region = key.Region;

							// Checks if the required region is set to null
							if (region == null)
							{
								ply.SendInfoMessage("No valid region was set for this key.");
								return;
							}

							// Checks if the player is inside the region
							if (args.Player.CurrentRegion != region)
							{
								ply.SendErrorMessage("You are not in a valid region to make this exchange.");
								return;
							}
						}

						Item item;
						for (int i = 0; i < 50; i++)
						{
							item = ply.TPlayer.inventory[i];
							// Loops through the player's inventory
							if (item.netID == (int)key.Type)
							{
								// Found the item, checking for available slots
								if (item.stack == 1 || ply.InventorySlotAvailable)
								{
									ply.TPlayer.inventory[i].stack--;
									NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, String.Empty, ply.Index, i);
									Random rand = new Random();
									Item give = key.Items[rand.Next(0, key.Items.Count)];
									ply.GiveItem(give.netID, give.name, give.width, give.height, 1);
									Item take = TShock.Utils.GetItemById((int)key.Type);
									ply.SendSuccessMessage("Exchanged a {0} for 1 {1}!", take.name, give.name);
									return;
								}
								// Sent if neither of the above conditions were fulfilled.
								ply.SendErrorMessage("Make sure you have at least one available inventory slot.");
								return;
							}
						}
						break;

					case "reload":
						{
							if (!ply.Group.HasPermission("key.reload"))
							{
								ply.SendErrorMessage("You do not have access to this command.");
								break;
							}

							Config = Config.Read();
							Utils.InitKeys();
							ply.SendSuccessMessage("KeyChangerConfig.json reloaded successfully.");
							break;
						}

					case "list":
						{
							ply.SendMessage("Temple Key - " + String.Join(", ", Key.Temple.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Jungle Key - " + String.Join(", ", Key.Jungle.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Corruption Key - " + String.Join(", ", Key.Corruption.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Crimson Key - " + String.Join(", ", Key.Crimson.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Hallowed Key - " + String.Join(", ", Key.Hallowed.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Frozen Key - " + String.Join(", ", Key.Frozen.Items.Select(i => i.name)), Color.Goldenrod);
							break;
						}

					case "mode":
						{
							if (!ply.Group.HasPermission("key.mode"))
							{
								ply.SendErrorMessage("You do not have access to this command.");
								break;
							}

							if (args.Parameters.Count < 2)
							{
								ply.SendErrorMessage("Invalid syntax! Proper syntax: {0}key mode <normal/region/market>", Commands.Specifier);
								break;
							}

							string query = args.Parameters[1].ToLower();

							if (query == "normal")
							{
								Config.EnableRegionExchanges = false;
								ply.SendSuccessMessage("Exchange mode set to normal (exchange everywhere).");
							}
							else if (query == "region")
							{
								Config.EnableRegionExchanges = true;
								Config.MarketMode = false;
								ply.SendSuccessMessage("Exchange mode set to region (a region for each type).");
							}
							else if (query == "market")
							{
								Config.EnableRegionExchanges = true;
								Config.MarketMode = true;
								ply.SendSuccessMessage("Exchange mode set to market (one region for every type).");
							}
							else
							{
								ply.SendErrorMessage("Invalid syntax! Proper syntax: {0}key mode <normal/region/market>", Commands.Specifier);
								return;
							}
							Config.Write();
							break;
						}
					default:
						{
							ply.SendErrorMessage(Utils.ErrorMessage(ply));
							break;
						}
				}
			}
			else
			{
				ply.SendErrorMessage(Utils.ErrorMessage(ply));
			}
		}
	}
}
