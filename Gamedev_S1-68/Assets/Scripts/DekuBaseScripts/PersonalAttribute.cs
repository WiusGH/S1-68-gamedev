using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ReadOnlyAttribute : PropertyAttribute { }
public class SeparatorUpAttribute : PropertyAttribute { }
public class SeparatorDownAttribute : PropertyAttribute { }
public class HeaderBoxAttribute : PropertyAttribute
{
    public string Title;
    public Color Color;
    public string Tooltip;

    public HeaderBoxAttribute(string title, float r = 0.2f, float g = 0.6f, float b = 1f, string tooltip = null)
    {
        Title = title;
        Color = new Color(r, g, b);
        Tooltip = tooltip;
    }
}
public class SubHeaderAttribute : PropertyAttribute
{
    public string Title;
    public string Tooltip;
    public SubHeaderAttribute(string title, string tooltip = "")
    {
        Title = title;
        Tooltip = tooltip;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

[CustomPropertyDrawer(typeof(SeparatorUpAttribute))]
public class SeparatorUPDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        Color color = new Color(0.45f, 0.45f, 0.45f, 1f);

        float lineY = position.y + position.height - 4f;
        Rect rect = new Rect(position.x, lineY, position.width, 2);
        EditorGUI.DrawRect(rect, color);
    }


    public override float GetHeight()
    {
        return 10f;
    }
}

[CustomPropertyDrawer(typeof(SeparatorDownAttribute))]
public class SeparatorDownDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        Color color = new Color(0.45f, 0.45f, 0.45f, 1f);

        float lineY = position.y + 1f;
        Rect rect = new Rect(position.x, lineY, position.width, 2);
        EditorGUI.DrawRect(rect, color);
    }


    public override float GetHeight()
    {
        return 10f;
    }
}

[CustomPropertyDrawer(typeof(HeaderBoxAttribute))]
public class HeaderBoxDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        var header = (HeaderBoxAttribute)attribute;

        var rect = new Rect(position.x, position.y + 2, position.width, EditorGUIUtility.singleLineHeight + 4);
        EditorGUI.DrawRect(rect, header.Color * 0.6f);

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
            fontSize = 12
        };

        GUIContent content = new GUIContent(header.Title.ToUpper(), header.Tooltip);
        EditorGUI.LabelField(rect, content, style);
    }

    public override float GetHeight()
    {
        return EditorGUIUtility.singleLineHeight + 10;
    }
}

[CustomPropertyDrawer(typeof(SubHeaderAttribute))]
public class SubHeaderDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        var header = (SubHeaderAttribute)attribute;

        // Fondo gris sutil
        Color background = new Color(0.25f, 0.25f, 0.25f, 1f);
        Rect rect = new Rect(position.x, position.y + 4, position.width, EditorGUIUtility.singleLineHeight + 2);
        EditorGUI.DrawRect(rect, background);

        // Texto alineado a la izquierda, en mayúsculas pequeñas
        GUIStyle style = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(0.85f, 0.85f, 0.85f, 1f) },
            fontSize = 11,
            padding = new RectOffset(8, 0, 0, 0)
        };

        GUIContent content = new GUIContent(header.Title.ToUpper(), header.Tooltip);
        EditorGUI.LabelField(rect, content, style);
    }

    public override float GetHeight()
    {
        return EditorGUIUtility.singleLineHeight + 10;
    }
}

#endif