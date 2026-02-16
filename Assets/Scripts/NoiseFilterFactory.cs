using UnityEngine;

public class NoiseFilterFactory : MonoBehaviour
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
        switch (settings.filterType) 
        { 
            case NoiseSettings.FilterType.Simple: 
                return new SimpleNoiseFilter(settings.simpleNoiseSettings); 
            case NoiseSettings.FilterType.Rigid: 
                return new RigidNoiseFilter(settings.rigidNoiseSettings); 
            default: 
                return null; 
        }
    }
}
