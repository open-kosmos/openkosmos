namespace Arkship.Parts
{
    //TODO - This should probably have a load of sub-classes for different types of files
    // with their own parameters (e.g. [FloatTweakable(min, max)])
    
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class TweakableAttribute : System.Attribute
    {
        
    }
}