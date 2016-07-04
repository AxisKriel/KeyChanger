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

		public Key()
		{
			Items = new List<Terraria.Item>();
		}

		public Key(string name, KeyTypes type, bool enabled) : this()
		{
			Name = name;
			Type = type;
			Enabled = enabled;
		}
	}
}
