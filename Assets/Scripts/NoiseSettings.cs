using System;
using System.Diagnostics;
using UnityEngine;
[Serializable]
public class NoiseSettings 
{
    public enum FilterType { Simple, Rigid } public FilterType filterType;
    public bool enabled = true;
    public bool useFirstLayerMask = false;
    [ConditionalHide("filterType", 0)] 
    public SimpleNoiseSettings simpleNoiseSettings; 
    
    [ConditionalHide("filterType", 1)] 
    public RigidNoiseSettings rigidNoiseSettings;

    [Serializable]
    public class SimpleNoiseSettings 
    { 

        public float strength = 1; 
        [Range(1, 8)] 
        public int numLayers = 1; 
        public float baseRoughness = 1; 
        public float roughness = 2; public float persistence = 0.5f; 
        public Vector3 center; public float minValue; 
    }
    [Serializable]
    public class RigidNoiseSettings :SimpleNoiseSettings
    { 
        public float weightMultiplier = 0.8f; 
    }


}