using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			this.Name = name;
			this.Type = type;
			this.Enabled = enabled;

			this.Region = null;
			this.Items = new List<Terraria.Item>();
		}

		public static Key Temple = new Key("temple", KeyTypes.Temple, Config.contents.EnableTempleKey);
		public static Key Jungle = new Key("jungle", KeyTypes.Jungle, Config.contents.EnableJungleKey);
		public static Key Corruption = new Key("corruption", KeyTypes.Corruption, Config.contents.EnableCorruptionKey);
		public static Key Crimson = new Key("crimson", KeyTypes.Crimson, Config.contents.EnableCrimsonKey);
		public static Key Hallowed = new Key("hallowed", KeyTypes.Hallowed, Config.contents.EnableHallowedKey);
		public static Key Frozen = new Key("frozen", KeyTypes.Frozen, Config.contents.EnableFrozenKey);
	}
}
