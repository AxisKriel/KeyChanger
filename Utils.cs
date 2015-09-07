using System;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace KeyChanger
{
	class Utils
	{
		#region InitKeys
		/// <summary>
		/// Loads all keys.
		/// </summary>
		public static void InitKeys()
		{
			Key.Temple = LoadKey(KeyTypes.Temple, TShock.Regions.GetRegionByName(Config.contents.TempleRegion));
			Key.Jungle = LoadKey(KeyTypes.Jungle, TShock.Regions.GetRegionByName(Config.contents.JungleRegion));
			Key.Corruption = LoadKey(KeyTypes.Corruption, TShock.Regions.GetRegionByName(Config.contents.CorruptionRegion));
			Key.Crimson = LoadKey(KeyTypes.Crimson, TShock.Regions.GetRegionByName(Config.contents.CrimsonRegion));
			Key.Hallowed = LoadKey(KeyTypes.Hallowed, TShock.Regions.GetRegionByName(Config.contents.HallowedRegion));
			Key.Frozen = LoadKey(KeyTypes.Frozen, TShock.Regions.GetRegionByName(Config.contents.FrozenRegion));
		}
		#endregion

		#region LoadKey
		/// <summary>
		/// Loads a key from KeyChangerConfig.json.
		/// </summary>
		/// <param name="type">The type of key to load.</param>
		/// <returns>The key with all the required data.</returns>
		public static Key LoadKey(KeyTypes type, Region region = null)
		{
			Key key;
			switch (type)
			{
				case KeyTypes.Temple:
					key = new Key("temple", KeyTypes.Temple, Config.contents.EnableTempleKey);
					key.Items = GetItems(Config.contents.TempleKeyItem);
					key.Region = TShock.Regions.GetRegionByName(Config.contents.TempleRegion);
					break;
				case KeyTypes.Jungle:
					key = new Key("jungle", KeyTypes.Jungle, Config.contents.EnableJungleKey);
					key.Items = GetItems(Config.contents.JungleKeyItem);
					key.Region = TShock.Regions.GetRegionByName(Config.contents.JungleRegion);
					break;
				case KeyTypes.Corruption:
					key = new Key("corruption", KeyTypes.Corruption, Config.contents.EnableCorruptionKey);
					key.Items = GetItems(Config.contents.CorruptionKeyItem);
					key.Region = TShock.Regions.GetRegionByName(Config.contents.CorruptionRegion);
					break;
				case KeyTypes.Crimson:
					key = new Key("crimson", KeyTypes.Crimson, Config.contents.EnableCrimsonKey);
					key.Items = GetItems(Config.contents.CrimsonKeyItem);
					key.Region = TShock.Regions.GetRegionByName(Config.contents.CrimsonRegion);
					break;
				case KeyTypes.Hallowed:
					key = new Key("hallowed", KeyTypes.Hallowed, Config.contents.EnableHallowedKey);
					key.Items = GetItems(Config.contents.HallowedKeyItem);
					key.Region = TShock.Regions.GetRegionByName(Config.contents.HallowedRegion);
					break;
				case KeyTypes.Frozen:
					key = new Key("frozen", KeyTypes.Frozen, Config.contents.EnableFrozenKey);
					key.Items = GetItems(Config.contents.FrozenKeyItem);
					key.Region = TShock.Regions.GetRegionByName(Config.contents.FrozenRegion);
					break;
				default:
					return null;
			}
			return key;
		}
		#endregion

		#region GetItems
		/// <summary>
		/// Returns a list of Terraria.Item from a list of Item ids.
		/// </summary>
		/// <param name="id">The int[] containing the Item ids.</param>
		/// <returns>List[Item]</returns>
		public static List<Item> GetItems(int[] id)
		{
			List<Item> list = new List<Item>();
			foreach (int item in id)
			{
				list.Add(TShock.Utils.GetItemById(item));
			}
			return list;
		}
		#endregion

		#region ErrorMessageHandler
		/// <summary>
		/// Handles error messages thrown by erroneous / lack of parameters by checking a player's group permissions.
		/// </summary>
		/// <param name="ply">The player executing the command.</param>
		/// <returns>A string matching the error message.</returns>
		public static string ErrorMessage(TSPlayer ply)
		{
			string error;
			var list = new List<string>()
			{
				ply.Group.HasPermission("key.change") ? "change" : null,
				ply.Group.HasPermission("key.reload") ? "reload" : null,
				ply.Group.HasPermission("key.mode") ? "mode" : null,
				"list"
			};

			string valid = String.Join("/", list.FindAll(i => i != null));
			error = String.Format("Invalid syntax! Proper syntax: {0}key <{1}> [type]", Commands.Specifier, valid);
			return error;
		}
		#endregion
	}
}
