using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace KeyChanger
{
	[ApiVersion(1, 16)]
	public class KeyChanger : TerrariaPlugin
	{
		#region Plugin Info
		public override Version Version
		{
			get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
		}

		public override string UpdateURL
		{
			get { return "https://dl.dropboxusercontent.com/u/31979270/tshockplugins/keychanger-update.json?raw=true"; }
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


		public KeyChanger(Main game)
			: base(game)
		{
		}
		#endregion

		#region Initialize & Dispose
		public override void Initialize()
		{
			//This is the main command, which branches to everything the plugin can do, by checking the first parameter a player inputs
			Commands.ChatCommands.Add(new Command(new List<string>() { "key.change", "key.reload", "key.mode" }, KeyChange, "key")
			{
				HelpDesc = new[]
				{
					"/key - Shows plugin info",
					"/key change <type> - Exchanges a key of the input type",
					"/key list - Shows a list of available keys and items",
					"/key mode <mode> - Changes exchange mode",
					"/key reload - Reloads the config file",
					"If an exchange fails, make sure your inventory has free slots"
				}
			});
			if (!Config.ReadConfig())
			{
				Log.ConsoleError("Failed to read KeyChangerConfig.json. Consider deleting the file so that it may be recreated.");
			}
			Utils.InitKeys();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		#endregion

		#region KeyChange
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
				ply.SendMessage(string.Format("KeyChanger (v{0}) by Enerdy", version), Color.SkyBlue);
				ply.SendMessage("Description: Changes special chest keys into their specific items", Color.SkyBlue);
				ply.SendMessage("Syntax: /key <list/mode/change/reload> [type]", Color.SkyBlue);
				ply.SendMessage("Type /help key for more info", Color.SkyBlue);
			}
			else if (args.Parameters[0].ToLower() == "change" && args.Parameters.Count == 1)
			{
				ply.SendErrorMessage("Invalid syntax! Proper syntax: /key change <type>");
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
							ply.SendErrorMessage("Invalid key type! Available types: " + string.Join(", ",
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

						if (Config.contents.EnableRegionExchanges)
						{
							Region region;
							if (Config.contents.MarketMode)
								region = TShock.Regions.GetRegionByName(Config.contents.MarketRegion);
							else
								region = key.Region;

							// Checks if the required region is set to null
							if (region == null)
							{
								ply.SendInfoMessage("No valid region was set for this key.");
								return;
							}

							// Checks if the player is inside the region
							if (!region.Area.Contains(ply.TileX, ply.TileY))
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
									NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, string.Empty, ply.Index, i);
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

							if (Config.ReadConfig())
							{
								Utils.InitKeys();
								ply.SendMessage("KeyChangerConfig.json reloaded successfully.", Color.Green);
								break;
							}
							else
							{
								ply.SendErrorMessage("Failed to read KeyChangerConfig.json. Consider deleting the file so that it may be recreated.");
								break;
							}
						}

					case "list":
						{
							ply.SendMessage("Temple Key - " + string.Join(", ", Key.Temple.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Jungle Key - " + string.Join(", ", Key.Jungle.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Corruption Key - " + string.Join(", ", Key.Corruption.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Crimson Key - " + string.Join(", ", Key.Crimson.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Hallowed Key - " + string.Join(", ", Key.Hallowed.Items.Select(i => i.name)), Color.Goldenrod);
							ply.SendMessage("Frozen Key - " + string.Join(", ", Key.Frozen.Items.Select(i => i.name)), Color.Goldenrod);
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
								ply.SendErrorMessage("Invalid syntax! Proper syntax: /key mode <normal/region/market>");
								break;
							}

							string query = args.Parameters[1].ToLower();

							if (query == "normal")
							{
								Config.contents.EnableRegionExchanges = false;
								ply.SendSuccessMessage("Exchange mode set to normal (exchange everywhere).");
							}
							else if (query == "region")
							{
								Config.contents.EnableRegionExchanges = true;
								Config.contents.MarketMode = false;
								ply.SendSuccessMessage("Exchange mode set to region (a region for each type).");
							}
							else if (query == "market")
							{
								Config.contents.EnableRegionExchanges = true;
								Config.contents.MarketMode = true;
								ply.SendSuccessMessage("Exchange mode set to market (one region for every type).");
							}
							else
							{
								ply.SendErrorMessage("Invalid syntax! Proper syntax: /key mode <normal/region/market>");
								return;
							}
							Config.UpdateConfig();
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
		#endregion
	}
}
