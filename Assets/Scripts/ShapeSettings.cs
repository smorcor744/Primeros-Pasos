using UnityEngine;

[CreateAssetMenu()  ]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1;
    public NoiseSettings[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerMask = false;
        public NoiseSettings noiseSettings;
    }
}
