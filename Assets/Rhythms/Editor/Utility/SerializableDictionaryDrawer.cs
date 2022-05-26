using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public abstract class SerializableKeyValueTemplate<K, V> : ScriptableObject
{
    public K key;
    public V value;
}

public abstract class SerializableDictionaryDrawer<K, V> : PropertyDrawer
{

    protected abstract SerializableKeyValueTemplate<K, V> GetTemplate();
    protected T GetGenericTemplate<T>() where T : SerializableKeyValueTemplate<K, V>
    {
        return ScriptableObject.CreateInstance<T>();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect varNameRect = position;
        varNameRect.height = EditorGUIUtility.singleLineHeight;

        SerializedProperty keysProp = GetKeysProp(property);
        SerializedProperty valuesProp = GetValuesProp(property);
        
        int numLines = keysProp.arraySize;

        EditorGUI.LabelField(varNameRect, property.name + " (" + numLines + ")");

        //if (property.isExpanded)
        //{
        //    SerializedProperty keysProp = GetKeysProp(property);
        //    SerializedProperty valuesProp = GetValuesProp(property);
        //    
        //    int numLines = keysProp.arraySize;
        //
        //    Rect addButtonRect = varNameRect;
        //    addButtonRect.y += EditorGUIUtility.singleLineHeight;
        //    if (GUI.Button(addButtonRect, "Add"))
        //    {
        //        bool assignment = false;
        //        for (int i = 0; i < numLines; i++)
        //        { // Try to replace existing value
        //            if (SerializedPropertyExtension.EqualBasics(GetIndexedItemProp(keysProp, i), GetTemplateKeyProp(property)))
        //            {
        //                SerializedPropertyExtension.CopyBasics(GetTemplateValueProp(property), GetIndexedItemProp(valuesProp, i));
        //                assignment = true;
        //                break;
        //            }
        //        }
        //        if (!assignment)
        //        { // Create a new value
        //            keysProp.arraySize += 1;
        //            valuesProp.arraySize += 1;
        //            SerializedPropertyExtension.CopyBasics(GetTemplateKeyProp(property), GetIndexedItemProp(keysProp, numLines));
        //            SerializedPropertyExtension.CopyBasics(GetTemplateValueProp(property), GetIndexedItemProp(valuesProp, numLines));
        //        }
        //    }
        //
        //    for (int i = 0; i < numLines; i++)
        //    {
        //        Rect keyRect = addButtonRect;
        //        keyRect.y += EditorGUIUtility.singleLineHeight * (i + 1);
        //
        //        EditorGUIUtility.labelWidth = 100f;
        //
        //        keyRect.x += 15f; //indentation
        //        keyRect.width -= 15f;
        //
        //        float buttonWidth = 60f;
        //        keyRect.width -= buttonWidth;
        //        keyRect.width /= 2f;
        //
        //        Rect valueRect = keyRect;
        //        valueRect.x += keyRect.width;
        //
        //        if (GetTemplateValueProp(property).hasVisibleChildren)
        //        { // if the value has children, indent to make room for fold arrow
        //            valueRect.x += 15;
        //            valueRect.width -= 15;
        //        }
        //
        //        Rect removeButtonRect = valueRect;
        //        removeButtonRect.x += valueRect.width;
        //        removeButtonRect.width = buttonWidth;
        //
        //        float kHeight = EditorGUI.GetPropertyHeight(GetTemplateKeyProp(property));
        //        float vHeight = EditorGUI.GetPropertyHeight(GetTemplateValueProp(property));
        //        float maxHeight = Mathf.Max(kHeight, vHeight);
        //
        //        keyRect.height = maxHeight;
        //        valueRect.height = maxHeight;
        //
        //        EditorGUI.PropertyField(keyRect, GetTemplateKeyProp(property), true);
        //        EditorGUI.PropertyField(valueRect, GetTemplateValueProp(property), true);
        //
        //        if (GUI.Button(removeButtonRect, "Remove"))
        //        {
        //            keysProp.DeleteArrayElementAtIndex(i);
        //            valuesProp.DeleteArrayElementAtIndex(i);
        //        }
        //    }
        //
        //
        //}
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight;

        var total = EditorGUIUtility.singleLineHeight;

        var kHeight = EditorGUI.GetPropertyHeight(GetTemplateKeyProp(property));
        var vHeight = EditorGUI.GetPropertyHeight(GetTemplateValueProp(property));
        total += Mathf.Max(kHeight, vHeight);

        var keysProp = GetKeysProp(property);
        var valuesProp = GetValuesProp(property);
        int numLines = keysProp.arraySize;
        for (int i = 0; i < numLines; i++)
        {
            kHeight = EditorGUI.GetPropertyHeight(GetIndexedItemProp(keysProp, i));
            vHeight = EditorGUI.GetPropertyHeight(GetIndexedItemProp(valuesProp, i));
            total += Mathf.Max(kHeight, vHeight);
        }
        return total;
    }

    private SerializedProperty GetTemplateKeyProp(SerializedProperty mainProp)
    {
        return GetTemplateProp(templateKeyProp, mainProp);
    }
    private SerializedProperty GetTemplateValueProp(SerializedProperty mainProp)
    {
        return GetTemplateProp(templateValueProp, mainProp);
    }

    private SerializedProperty GetTemplateProp(Dictionary<int, SerializedProperty> source, SerializedProperty mainProp)
    {
        SerializedProperty p;
        if (!source.TryGetValue(mainProp.GetObjectCode(), out p))
        {
            var templateObject = GetTemplate();
            var templateSerializedObject = new SerializedObject(templateObject);
            var kProp = templateSerializedObject.FindProperty("key");
            var vProp = templateSerializedObject.FindProperty("value");
            templateKeyProp[mainProp.GetObjectCode()] = kProp;
            templateValueProp[mainProp.GetObjectCode()] = vProp;
            p = source == templateKeyProp ? kProp : vProp;
        }
        return p;
    }
    private Dictionary<int, SerializedProperty> templateKeyProp = new Dictionary<int, SerializedProperty>();
    private Dictionary<int, SerializedProperty> templateValueProp = new Dictionary<int, SerializedProperty>();

    private SerializedProperty GetKeysProp(SerializedProperty mainProp)
    {
        return GetCachedProp(mainProp, "keys", keysProps);
    }
    private SerializedProperty GetValuesProp(SerializedProperty mainProp)
    {
        return GetCachedProp(mainProp, "values", valuesProps);
    }

    private SerializedProperty GetCachedProp(SerializedProperty mainProp, string relativePropertyName, Dictionary<int, SerializedProperty> source)
    {
        SerializedProperty p;
        int objectCode = mainProp.GetObjectCode();
        if (!source.TryGetValue(objectCode, out p))
            source[objectCode] = p = mainProp.FindPropertyRelative(relativePropertyName);
        return p;
    }

    private Dictionary<int, SerializedProperty> keysProps = new Dictionary<int, SerializedProperty>();
    private Dictionary<int, SerializedProperty> valuesProps = new Dictionary<int, SerializedProperty>();

    private Dictionary<int, Dictionary<int, SerializedProperty>> indexedPropertyDicts = new Dictionary<int, Dictionary<int, SerializedProperty>>();

    private SerializedProperty GetIndexedItemProp(SerializedProperty arrayProp, int index)
    {
        Dictionary<int, SerializedProperty> d;
        if (!indexedPropertyDicts.TryGetValue(arrayProp.GetObjectCode(), out d))
            indexedPropertyDicts[arrayProp.GetObjectCode()] = d = new Dictionary<int, SerializedProperty>();
        SerializedProperty result;
        if (!d.TryGetValue(index, out result))
            d[index] = result = arrayProp.FindPropertyRelative(string.Format("Array.data[{0}]", index));
        return result;
    }

}
