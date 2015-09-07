using System;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace KeyChanger
{
	public class Config
	{
		private static string savepath = TShock.SavePath;
		public static Contents contents;

		#region Contents
		public class Contents
		{
			public bool EnableRegionExchanges = false;						// |Default set to false
			public bool MarketMode = false;									// |Use only a general market region
			//public bool EnableGoldKey = true;								// |Gold Key works similar to a currency
			public bool EnableTempleKey = true;
			public bool EnableJungleKey = true;
			public bool EnableCorruptionKey = true;
			public bool EnableCrimsonKey = true;
			public bool EnableHallowedKey = true;
			public bool EnableFrozenKey = true;

			public int[] TempleKeyItem = new int[] { 1293 };				// |Lihzahrd Power Cell
			public int[] JungleKeyItem = new int[] { 1156 };				// |Piranha Gun
			public int[] CorruptionKeyItem = new int[] { 1571 };			// |Scourge of the Corruptor
			public int[] CrimsonKeyItem = new int[] { 1569 };				// |Vampire Knifes
			public int[] HallowedKeyItem = new int[] { 1260 };				// |Rainbow Gun
			public int[] FrozenKeyItem = new int[] { 1572 };				// |Frost Hydra Staff

			// Those are optional; They're only needed if EnableRegionExchanges is set to true. Default is set to null,
			// so that players can be informed of non-existing regions.
			public string MarketRegion = null;
			public string TempleRegion = null;
			public string JungleRegion = null;
			public string CorruptionRegion = null;
			public string CrimsonRegion = null;
			public string HallowedRegion = null;
			public string FrozenRegion = null;
		}
		#endregion

		#region Create Config
		public static void CreateConfig()
		{
			string filepath = Path.Combine(savepath, "KeyChangerConfig.json");
			try
			{
				using (var stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.Write))
				{
					using (var sr = new StreamWriter(stream))
					{
						contents = new Contents();
						var configString = JsonConvert.SerializeObject(contents, Formatting.Indented);
						sr.Write(configString);
					}
					stream.Close();
				}
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.Message);
				contents = new Contents();
			}
		}
		#endregion

		#region Read Config
		public static bool ReadConfig()
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
							contents = JsonConvert.DeserializeObject<Contents>(configString);
						}
						stream.Close();
					}
					return true;
				}
				else
				{
					CreateConfig();
					TShock.Log.ConsoleInfo("Created KeyChangerConfig.json.");
					return true;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.Message);
			}
			return false;
		}
		#endregion

		#region Update Config
		public static bool UpdateConfig()
		{
			string filepath = Path.Combine(savepath, "KeyChangerConfig.json");
			try
			{
				if (!File.Exists(filepath))
					return false;

				string query = JsonConvert.SerializeObject(contents, Formatting.Indented);
				using (var stream = new StreamWriter(filepath, false))
				{
					stream.Write(query);
				}
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.Message);
				return false;
			}
		}
		#endregion
	}
}
