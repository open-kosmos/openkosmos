using UnityEngine;
using UnityEngine.Windows;

namespace Kosmos.Prototype.Parts
{
	//Defines the metadata for a part. The actual structure is stored separately
	//(The separate structure is a throwback to when we were using actual Unity prefabs.
	// This could now be combined with PartPrefabData)
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