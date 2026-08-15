using UnityEngine;

public abstract class AnalogGlitchBase : MonoBehaviour
{
    public abstract bool Enabled { get; set; }
    public abstract float ScanLineJitter { get; set; }
    public abstract float ColorDrift { get; set; }
    public abstract float VerticalJump { get; set; }
    public abstract float HorizontalShake { get; set; }
}
