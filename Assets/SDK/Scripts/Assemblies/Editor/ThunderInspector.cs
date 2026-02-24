using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TriInspector.Editors;
using UnityEngine.UIElements;

namespace ThunderRoad
{
    public class ThunderInspector : Editor
    {
        protected TriEditorCore _core;

        public void OnEnable()
        {
            OnEnablePreCore();
            _core = new TriEditorCore(this);
            OnEnablePostCore();
        }

        protected virtual void OnEnablePreCore() { }

        protected virtual void OnEnablePostCore() { }

        public void OnDisable()
        {
            OnDisablePreDispose();
            _core.Dispose();
            OnDisablePostDispose();
        }

        protected virtual void OnDisablePreDispose() { }

        protected virtual void OnDisablePostDispose() { }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            root.Add(new IMGUIContainer(() => OnCreateGUIPreCoreElement()));
            root.Add(_core.CreateVisualElement());
            root.Add(new IMGUIContainer(() => OnCreateGUIPostCoreElement()));

            return root;
        }

        protected virtual void OnCreateGUIPreCoreElement() { }

        protected virtual void OnCreateGUIPostCoreElement() { }
    }
}
