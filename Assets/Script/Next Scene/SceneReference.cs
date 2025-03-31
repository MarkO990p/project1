using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset; // ใช้ใน Editor เท่านั้น
#endif

    [SerializeField] private string sceneName; // ใช้ใน Runtime

    public string SceneName => sceneName;

#if UNITY_EDITOR
    public void UpdateSceneName()
    {
        sceneName = sceneAsset != null ? sceneAsset.name : "";
    }
#endif
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
        var sceneNameProperty = property.FindPropertyRelative("sceneName");

        EditorGUI.BeginChangeCheck();
        var newScene = EditorGUI.ObjectField(position, label, sceneAssetProperty.objectReferenceValue, typeof(SceneAsset), false);
        if (EditorGUI.EndChangeCheck())
        {
            sceneAssetProperty.objectReferenceValue = newScene;
            sceneNameProperty.stringValue = newScene != null ? ((SceneAsset)newScene).name : "";
        }

        EditorGUI.EndProperty();
    }
}
#endif
