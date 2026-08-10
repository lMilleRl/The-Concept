public enum SurfaceType
{
    None = -1,
    Grass,
    Ground,
    Asphalt,
    Wood,
    Snow
}

public interface ISurfaceInfo
{
    SurfaceType SurfaceType { get; }
}
