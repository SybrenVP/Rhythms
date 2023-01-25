using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using UnityEditor.Overlays;

public class BehaviourTreeEditor : EditorWindow, ISupportsOverlays
{
    private BehaviourTreeView _treeView;
    private InspectorView _inspectorView;
    private IMGUIContainer _blackboardView;
    private TrackView _trackView;

    private SerializedObject _treeObject;
    private SerializedProperty _blackboardProperty;

    [MenuItem("BehaviourTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIBuilderImplementations/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIBuilderImplementations/Editor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        _trackView = root.Q<TrackView>();
        _trackView.OnStateSelected = OnStateSelectionChanged;
        _treeView = root.Q<BehaviourTreeView>();
        if (_treeView == null)
            return;
        _treeView.OnNodeSelected = OnNodeSelectionChanged;
        _inspectorView = root.Q<InspectorView>();
        _blackboardView = root.Q<IMGUIContainer>();
        _blackboardView.pickingMode = PickingMode.Ignore;
        //_blackboardView.onGUIHandler = () =>
        //{
        //    _treeObject.Update();
        //    //EditorGUILayout.PropertyField(_blackboardProperty);
        //    _treeObject.ApplyModifiedProperties();
        //};

        ToolChange(ToolbarView.EToolType.Select, ToolbarView.EToolType.Select);
        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        switch (stateChange)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }


    private void OnSelectionChange()
    {
        Track track = Selection.activeObject as Track;
        if (!track)
        {
            if (Selection.activeGameObject != null)
            {
                BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (runner)
                    track = runner.Track;
            }
        }

        if (track != null )
        {
            if (Application.isPlaying || !Application.isPlaying && AssetDatabase.CanOpenAssetInEditor(track.GetInstanceID()))
                _trackView.PopulateView(track);

            _treeObject = new SerializedObject(track);
            _blackboardProperty = _treeObject.FindProperty("Blackboard");
        }
    }

    private void OnNodeSelectionChanged(NodeView nodeView)
    {
        _inspectorView.UpdateSelected(nodeView);
    }

    private void OnStateSelectionChanged(StateView stateView)
    {
        _treeView.UpdateSelected(stateView);
    }

    public void ToolChange(ToolbarView.EToolType prevType, ToolbarView.EToolType newType)
    {
        switch (prevType)
        {
            case ToolbarView.EToolType.Select:
                _trackView.DisableSelection();
                break;

            case ToolbarView.EToolType.Move:
                _trackView.DisableMove();
                break;

            case ToolbarView.EToolType.Resize:

                break;
        }

        switch (newType)
        {
            case ToolbarView.EToolType.Select:
                _trackView.EnableSelection();
                break;

            case ToolbarView.EToolType.Move:
                _trackView.EnableMove();
                break;

            case ToolbarView.EToolType.Resize:

                break;
        }
    }

    private void OnInspectorUpdate()
    {
        _treeView?.UpdateNodeStates();

        //TODO: Update track view
    }
}