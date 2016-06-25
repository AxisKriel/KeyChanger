using System.Collections.Generic;

namespace KeyChanger
{
	public class Key
	{
		public string Name { get; set; }
		public KeyTypes Type { get; set; }
		public TShockAPI.DB.Region Region { get; set; }
		public bool Enabled { get; set; }
		public List<Terraria.Item> Items { get; set; }

		public Key(string name, KeyTypes type, bool enabled)
		{
			Name = name;
			Type = type;
			Enabled = enabled;

			Region = null;
			Items = new List<Terraria.Item>();
		}

		public static Key Temple = new Key("temple", KeyTypes.Temple, KeyChanger.Config.EnableTempleKey);
		public static Key Jungle = new Key("jungle", KeyTypes.Jungle, KeyChanger.Config.EnableJungleKey);
		public static Key Corruption = new Key("corruption", KeyTypes.Corruption, KeyChanger.Config.EnableCorruptionKey);
		public static Key Crimson = new Key("crimson", KeyTypes.Crimson, KeyChanger.Config.EnableCrimsonKey);
		public static Key Hallowed = new Key("hallowed", KeyTypes.Hallowed, KeyChanger.Config.EnableHallowedKey);
		public static Key Frozen = new Key("frozen", KeyTypes.Frozen, KeyChanger.Config.EnableFrozenKey);
	}
}
