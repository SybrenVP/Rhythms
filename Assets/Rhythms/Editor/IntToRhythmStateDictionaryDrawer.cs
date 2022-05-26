using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEditor.CustomPropertyDrawer(typeof(Rhythms.IntStateDictionary))]
public class IntStateDictionaryDrawer : SerializableDictionaryDrawer<int, Rhythms.RhythmState>
{
    protected override SerializableKeyValueTemplate<int, Rhythms.RhythmState> GetTemplate()
    {
        return GetGenericTemplate<SerializableIntStateTemplate>();
    }
}
internal class SerializableIntStateTemplate : SerializableKeyValueTemplate<int, Rhythms.RhythmState> { }
