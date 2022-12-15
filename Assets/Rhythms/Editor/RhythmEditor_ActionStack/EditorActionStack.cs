using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmEditor
{ 
    //Preferably we can save this object, allowing us to keep the action stack between instances
    public class EditorActionStack
    {
        public const int MaxActions = 50;

        public Stack<RhythmToolAction> UndoStack = new Stack<RhythmToolAction>(MaxActions);
        public Stack<RhythmToolAction> RedoStack = new Stack<RhythmToolAction>(MaxActions);

        public void Record(RhythmToolAction lastAction)
        {
            UndoStack.Push(lastAction);

            //Ensure we do not cross the max treshold in the stack
        }

        public void Undo()
        {
            RhythmToolAction action = UndoStack.Pop();
            action.Revert();
            
            RedoStack.Push(action);
        }

        public void Redo()
        {
            RhythmToolAction action = RedoStack.Pop();
            action.Apply();

            UndoStack.Push(action);
        }

        public bool HasUndoChanges()
        {
            return UndoStack.Count > 0;
        }

        public bool HasRedoChanges()
        {
            return RedoStack.Count > 0;
        }
    }
}