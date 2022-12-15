using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEditor.CustomPropertyDrawer(typeof(Rhythm.IntStateDictionary))]
public class IntStateDictionaryDrawer : SerializableDictionaryDrawer<int, Rhythm.State>
{
    protected override SerializableKeyValueTemplate<int, Rhythm.State> GetTemplate()
    {
        return GetGenericTemplate<SerializableIntStateTemplate>();
    }
}
internal class SerializableIntStateTemplate : SerializableKeyValueTemplate<int, Rhythm.State> { }
