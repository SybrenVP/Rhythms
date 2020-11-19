using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateOnSequenceEvent : DoOnSequenceEvent
{
    [SerializeField] private GameObject _prefabToSpawn = null;

    protected override void OnSequence(float offset)
    {
        Instantiate(_prefabToSpawn);
    }
}
