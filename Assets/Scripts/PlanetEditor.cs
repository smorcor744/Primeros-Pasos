using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Planeta))]
public class PlanetaEditor : Editor
{

    Planeta planeta;
    Editor shapeEditor;
    Editor colorEditor;
    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planeta.GeneratePlanet();
            }
        }
        
        if (GUILayout.Button("Generate Planet"))
        {
            planeta.GeneratePlanet();
        }

        DrawSettingsEditor(planeta.shapeSettings, planeta.OnShapeSettingsUpdated, ref planeta.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planeta.colorSettings, planeta.OnColorSettingsUpdated, ref planeta.colorSettingsFoldout, ref colorEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings == null)
        {
            return;
        }
        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
        using (var scope = new EditorGUI.ChangeCheckScope())
        {
            
            if (foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();

                if (scope.changed)
                {
                    if (onSettingsUpdated != null)
                    {
                        onSettingsUpdated();
                    }

                }
            }
        }

    }

    void OnEnable()
    {
        planeta = (Planeta)target;
    } 
}
