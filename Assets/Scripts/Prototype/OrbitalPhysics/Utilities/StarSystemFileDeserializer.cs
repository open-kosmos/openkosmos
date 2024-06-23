using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public class StarSystemFileDeserializer
    {
        public async Awaitable<StarSystemFile> DeserializeStarSystemFile(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<StarSystemFile>(json); //, settings);
        }
        
        public async Awaitable<CelestialBodyData> DeserializeCelestialBodyData(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<CelestialBodyData>(json);
        }
    }
}
