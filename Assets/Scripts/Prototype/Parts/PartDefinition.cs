using UnityEngine;
using UnityEngine.Windows;

namespace Kosmos.Prototype.Parts
{
	[System.Serializable]
	public class PartDefinition
	{
		public string Guid;
		public string Name;
		public string Path;
		public string Category;
		public string Description;
		
		public static PartDefinition Load(string json)
		{
			return JsonUtility.FromJson<PartDefinition>(json);
		}

		public static void Save(PartDefinition part, string path)
		{
			string json = JsonUtility.ToJson(part);
			System.IO.File.WriteAllText(path, json);
		}
	}
}