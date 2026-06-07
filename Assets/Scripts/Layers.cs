static public class Layers
{
    static public bool ContainsLayer(int mask, int layer)
    {
        return ( mask & (1 << layer)) != 0;
    }
 
    public const int Default = 0;
    public const int DefaultMask = 1 << 0;

 
    public const int TransparentFX = 1;
    public const int TransparentFXMask = 1 << 1;

 
    public const int Ignore_Raycast = 2;
    public const int Ignore_RaycastMask = 1 << 2;

 
    public const int Water = 4;
    public const int WaterMask = 1 << 4;

 
    public const int UI = 5;
    public const int UIMask = 1 << 5;

 
    public const int TransformHandle = 6;
    public const int TransformHandleMask = 1 << 6;

 
    public const int Court = 7;
    public const int CourtMask = 1 << 7;

 
    public const int Char = 8;
    public const int CharMask = 1 << 8;

}
