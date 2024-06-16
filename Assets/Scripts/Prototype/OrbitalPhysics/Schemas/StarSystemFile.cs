using Newtonsoft.Json;
using Unity.Mathematics;

namespace Kosmos.Prototype.OrbitalPhysics
{
    //[JsonConverter(typeof(StarSystemFileConverter))]
    public class StarSystemFile
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }
        
        [JsonProperty("bodies")]
        public StarSystemFileBodyEntry[] CelestialBodies { get; set; }
    }
    
    public class StarSystemFileBodyEntry
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("parent_id")]
        public string ParentId { get; set; }
        
        [JsonProperty("body_file")]
        public string BodyFile { get; set; }
        
        [JsonProperty("rotation")]
        public RotationData Rotation { get; set; }
        
        [JsonProperty("gal_coords_ly")]
        public double3 GalacticCoordinates { get; set; }
        
        [JsonProperty("orbit")]
        public OrbitData Orbit { get; set; }
        
        [JsonIgnore]
        public CelestialBodyData BodyData { get; set; }

        [JsonIgnore] 
        public int UpdateOrder { get; set; } = -1;
    }

    public class CelestialBodyData
    {
        [JsonProperty("formal_name")]
        public string FormalName { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("TEMP_color")]
        public string ColorCode { get; set; }
        
        [JsonProperty("equatorial_radius_m")]
        public double EquatorialRadiusM { get; set; }
        
        [JsonProperty("mass_kg")]
        public double MassKg { get; set; }
    }
    
    public class OrbitData
    {
        [JsonProperty("semimajor_axis_m")]
        public double SemiMajorAxisM { get; set; }
        
        [JsonProperty("eccentricity")]
        public double Eccentricity { get; set; }
        
        [JsonProperty("inclination_deg")]
        public double InclinationDeg { get; set; }
        
        [JsonProperty("longitude_asc_node_deg")]
        public double LongitudeAscNodeDeg { get; set; }
        
        [JsonProperty("arg_periapsis_deg")]
        public double ArgPeriapsisDeg { get; set; }
        
        [JsonProperty("mean_anomaly_epoch_deg")]
        public double MeanAnomalyAtEpochDeg { get; set; }
    }
    
    public class RotationData
    {
        [JsonProperty("period_sec")]
        public double PeriodSec { get; set; }
        
        [JsonProperty("obliquity_deg")]
        public double ObliquityDeg { get; set; }
        
        [JsonProperty("rotation_at_epoch_deg")]
        public double RotationAtEpochDeg { get; set; }
    }
}