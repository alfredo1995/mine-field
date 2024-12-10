using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using OpaGames.Blackcopter.UI.AutoFlowLayout;
using System;
using System.ComponentModel;

[CustomPropertyDrawer(typeof(RectAjustment))]
public class RectAjustEditor : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new VisualElement();
        Label title = new Label($"<b><size=110%>{property.name}</b>");
        title.enableRichText = true;
        root.Add(title);
        
        SerializedProperty p_parent = property.FindPropertyRelative(nameof(RectAjustment.parent));
        SerializedProperty p_siblingIndex = property.FindPropertyRelative(nameof(RectAjustment.siblingIndex));
        SerializedProperty p_rectTransform = property.FindPropertyRelative(nameof(RectAjustment.rectTransform));
        SerializedProperty p_anchoredPosition = property.FindPropertyRelative(nameof(RectAjustment.anchoredPosition));
        SerializedProperty p_sizeDelta = property.FindPropertyRelative(nameof(RectAjustment.sizeDelta));
        SerializedProperty p_anchorMin = property.FindPropertyRelative(nameof(RectAjustment.anchorMin));
        SerializedProperty p_anchorMax = property.FindPropertyRelative(nameof(RectAjustment.anchorMax));
        SerializedProperty p_pivot = property.FindPropertyRelative(nameof(RectAjustment.pivot));
        SerializedProperty p_scale = property.FindPropertyRelative(nameof(RectAjustment.scale));
        
        root.Add(new PropertyField(p_parent));
        root.Add(new PropertyField(p_siblingIndex));
        root.Add(new PropertyField(p_rectTransform));
        root.Add(new PropertyField(p_anchoredPosition));
        root.Add(new PropertyField(p_sizeDelta));
        root.Add(new PropertyField(p_anchorMin));
        root.Add(new PropertyField(p_anchorMax));
        root.Add(new PropertyField(p_pivot));
        root.Add(new PropertyField(p_scale));
        p_anchoredPosition.vector2Value = Vector2.one;
        
        Button saveData = new Button(() => 
        {
            if (p_rectTransform.objectReferenceValue == null) return;
            RectTransform rectTransform = p_rectTransform.objectReferenceValue as RectTransform;
            p_parent.objectReferenceValue = rectTransform.parent;
            p_siblingIndex.intValue = rectTransform.GetSiblingIndex();
            p_anchoredPosition.vector2Value = rectTransform.anchoredPosition;
            p_sizeDelta.vector2Value = rectTransform.sizeDelta;
            p_anchorMin.vector2Value = rectTransform.anchorMin;
            p_anchorMax.vector2Value = rectTransform.anchorMax;
            p_pivot.vector2Value = rectTransform.pivot;
            p_scale.vector2Value = rectTransform.localScale;
            p_rectTransform.serializedObject.ApplyModifiedProperties();
        });
        saveData.Add(new Label("Save Rect data"));
        root.Add(saveData);

        return root;
    }
}
