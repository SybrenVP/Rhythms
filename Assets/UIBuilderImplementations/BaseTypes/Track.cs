using Rhythm;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class Track : ScriptableObject
{
    public List<BehaviourTree> States = new List<BehaviourTree>();

    public AudioData AudioData;
    public int AmountOfTracks = 1;

    public void Update()
    {
        //TODO: Depending on the beat, we should move through all of the states
        States[0].Update();
    }

    public Track Clone()
    {
        Track track = Instantiate(this);
        track.States = new List<BehaviourTree>();
        track.States.ForEach(s =>
        {
            s.Clone();
        });
        return track;
    }

#if UNITY_EDITOR

    public BehaviourTree CreateState(int beat)
    {
        BehaviourTree state = ScriptableObject.CreateInstance<BehaviourTree>();
        state.Beat = beat;

        Undo.RecordObject(this, "Track (Create State)");
        States.Add(state);

        if (!Application.isPlaying)
            AssetDatabase.AddObjectToAsset(state, this);
        Undo.RegisterCreatedObjectUndo(state, "Track (Create State)");

        AssetDatabase.SaveAssets();
        return state;
    }

    public void DeleteState(BehaviourTree state)
    {
        Undo.RecordObject(this, "Track (Delete State)");
        States.Remove(state);

        Undo.DestroyObjectImmediate(state);

        AssetDatabase.SaveAssets();
    }

#endif

}
