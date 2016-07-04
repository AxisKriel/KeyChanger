using System;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace KeyChanger
{
	class Utils
	{
		/// <summary>
		/// Loads a key from KeyChangerConfig.json.
		/// </summary>
		/// <param name="type">The type of key to load.</param>
		/// <returns>The key with all the required data.</returns>
		public static Key LoadKey(KeyTypes type)
		{
			Key key;
			switch (type)
			{
				case KeyTypes.Temple:
					key = new Key("temple", KeyTypes.Temple, KeyChanger.Config.EnableTempleKey);
					key.Items = GetItems(KeyChanger.Config.TempleKeyItem);
					key.Region = TShock.Regions.GetRegionByName(KeyChanger.Config.TempleRegion);
					break;
				case KeyTypes.Jungle:
					key = new Key("jungle", KeyTypes.Jungle, KeyChanger.Config.EnableJungleKey);
					key.Items = GetItems(KeyChanger.Config.JungleKeyItem);
					key.Region = TShock.Regions.GetRegionByName(KeyChanger.Config.JungleRegion);
					break;
				case KeyTypes.Corruption:
					key = new Key("corruption", KeyTypes.Corruption, KeyChanger.Config.EnableCorruptionKey);
					key.Items = GetItems(KeyChanger.Config.CorruptionKeyItem);
					key.Region = TShock.Regions.GetRegionByName(KeyChanger.Config.CorruptionRegion);
					break;
				case KeyTypes.Crimson:
					key = new Key("crimson", KeyTypes.Crimson, KeyChanger.Config.EnableCrimsonKey);
					key.Items = GetItems(KeyChanger.Config.CrimsonKeyItem);
					key.Region = TShock.Regions.GetRegionByName(KeyChanger.Config.CrimsonRegion);
					break;
				case KeyTypes.Hallowed:
					key = new Key("hallowed", KeyTypes.Hallowed, KeyChanger.Config.EnableHallowedKey);
					key.Items = GetItems(KeyChanger.Config.HallowedKeyItem);
					key.Region = TShock.Regions.GetRegionByName(KeyChanger.Config.HallowedRegion);
					break;
				case KeyTypes.Frozen:
					key = new Key("frozen", KeyTypes.Frozen, KeyChanger.Config.EnableFrozenKey);
					key.Items = GetItems(KeyChanger.Config.FrozenKeyItem);
					key.Region = TShock.Regions.GetRegionByName(KeyChanger.Config.FrozenRegion);
					break;
				default:
					return null;
			}
			return key;
		}

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
				ply.HasPermission("key.change") ? "change" : null,
				ply.HasPermission("key.reload") ? "reload" : null,
				ply.HasPermission("key.mode") ? "mode" : null,
				"list"
			};

			string valid = String.Join("/", list.FindAll(i => i != null));
			error = String.Format("Invalid syntax! Proper syntax: {0}key <{1}> [type]", Commands.Specifier, valid);
			return error;
		}
	}
}
