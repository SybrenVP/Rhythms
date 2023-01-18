using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

public class GraphViewTest : GraphView
{
    public new class UxmlFactory : UxmlFactory<GraphViewTest, GraphView.UxmlTraits> { }

    public GraphViewTest()
    {
        Insert(0, new GridBackground());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIBuilderImplementations/Editor/GraphViewTest.uss");
        styleSheets.Add(styleSheet);

        style.width = 2000f;
    }
}
