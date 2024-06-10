namespace Kosmos.Prototype.Parts
{
    //TODO - This should probably have a load of sub-classes for different types of files
    // with their own parameters (e.g. [FloatTweakable(min, max)])
    // Should also have a DisplayName parameter for what's shown in the part info panel
    
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class TweakableAttribute : System.Attribute
    {
        
    }
}