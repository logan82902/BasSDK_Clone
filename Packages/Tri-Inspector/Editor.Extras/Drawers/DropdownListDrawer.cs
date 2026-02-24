using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using TriInspector.Drawers;
using TriInspector.Elements;
using TriInspector.Utilities;
using TriInspectorUnityInternalBridge;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: RegisterTriAttributeDrawer(typeof(DropdownListDrawer), TriDrawerOrder.Drawer)]

namespace TriInspector.Drawers
{
    public class DropdownListDrawer : TriAttributeDrawer<DropdownListAttribute>
    {
        public override TriExtensionInitializationResult Initialize(TriPropertyDefinition propertyDefinition)
        {
            if (!propertyDefinition.IsArray)
            {
                return "[TableList] valid only on lists";
            }

            return TriExtensionInitializationResult.Ok;
        }

        public override TriElement CreateElement(TriProperty property, TriElement next)
        {
            return new TriListElement(property);
        }
    }
}