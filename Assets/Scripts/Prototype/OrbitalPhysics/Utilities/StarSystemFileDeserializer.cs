using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kosmos.Prototype.OrbitalPhysics
{
    public class StarSystemFileDeserializer
    {
        public StarSystemFile DeserializeStarSystemFile(string filePath)
        {
            //var settings = new JsonSerializerSettings();
            //settings.Converters.Add(new StarSystemFileConverter());
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<StarSystemFile>(json); //, settings);
        }
        
        public CelestialBodyData DeserializeCelestialBodyData(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<CelestialBodyData>(json);
        }
    }
    
    
    
    
    
    /*public class StarSystemFileConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(StarSystemFile);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            StarSystemFile starSystemFile = new StarSystemFile
            {
                Id = jsonObject["id"].ToString(),
                Name = jsonObject["name"].ToString(),
                Subtitle = jsonObject["subtitle"].ToString()
            };

            JArray bodiesArray = (JArray)jsonObject["bodies"];
            starSystemFile.CelestialBodies = new StarSystemFileBodyEntry[bodiesArray.Count];

            for (int i = 0; i < bodiesArray.Count; i++)
            {
                JObject bodyObject = (JObject)bodiesArray[i];
                string type = bodyObject["type"].ToString();

                StarSystemFileBodyEntry bodyEntry;
                CelestialBodyData bodyData;
                switch (type)
                {
                    case "star":
                        //bodyEntry = bodyObject.ToObject<StarBodyData>(serializer);
                        break;
                    case "planet":
                        //bodyEntry = bodyObject.ToObject<PlanetBodyData>(serializer);
                        break;
                    default:
                        throw new Exception($"Unknown body type: {type}");
                }

                //starSystemFile.CelestialBodies[i] = bodyEntry;
            }

            return starSystemFile;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            StarSystemFile starSystemFile = (StarSystemFile)value;
            writer.WriteStartObject();

            writer.WritePropertyName("id");
            writer.WriteValue(starSystemFile.Id);

            writer.WritePropertyName("name");
            writer.WriteValue(starSystemFile.Name);

            writer.WritePropertyName("subtitle");
            writer.WriteValue(starSystemFile.Subtitle);

            writer.WritePropertyName("bodies");
            writer.WriteStartArray();
            foreach (var body in starSystemFile.CelestialBodies)
            {
                JObject bodyObject = JObject.FromObject(body, serializer);
                bodyObject.WriteTo(writer);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }*/
}