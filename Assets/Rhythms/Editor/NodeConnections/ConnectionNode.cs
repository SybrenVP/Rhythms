using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RhythmEditor
{
    public enum EConnectionType
    {
        Output,
        Input
    }

    public class ConnectionNode
    {
        //This script is a visual representation of the in- and output variables
        public StateDrawer OwningStateDrawer;
        public Rect View;

        private EConnectionType _connectionType;

        public ConnectionNode(StateDrawer owningStateDrawer, EConnectionType type)
        {
            OwningStateDrawer = owningStateDrawer;
            _connectionType = type;
        }

        public void Draw()
        {
            SetView();

            EditorGUI.DrawRect(View, Color.blue);
        }

        public void SetView()
        {
            Rect owningView = OwningStateDrawer.View;

            View = new Rect(_connectionType == EConnectionType.Input ? owningView.x - 5f : owningView.x + owningView.width, owningView.y + 5f, owningView.width * 0.2f, 5f);
        }
    }
}