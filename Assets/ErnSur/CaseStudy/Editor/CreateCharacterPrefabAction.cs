using System;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace ErnSur.CaseStudy.Editor
{
    class CreateCharacterPrefabAction : EndNameEditAction
    {
        public static CreateCharacterPrefabAction New(CharacterCreationArgs args, Action<StoreItemViewModel> onEnd)
        {
            EditorUtility.FocusProjectWindow();
            var instance = CreateInstance<CreateCharacterPrefabAction>();
            instance.args = args;
            instance.onEnd = onEnd;
            return instance;
        }

        CharacterCreationArgs args;
        Action<StoreItemViewModel> onEnd;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var prefab = PrefabImporter.CreateCharacterPrefab(args, pathName);
            EditorGUIUtility.PingObject(prefab);
            onEnd?.Invoke(prefab);
        }
    }
    
    class DynamicEndNameEditAction : EndNameEditAction
    {
        public static DynamicEndNameEditAction New(Action<string> onPathNameSet)
        {
            EditorUtility.FocusProjectWindow();
            var instance = CreateInstance<DynamicEndNameEditAction>();
            instance.onEnd = onPathNameSet;
            return instance;
        }
        Action<string> onEnd;
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            onEnd?.Invoke(pathName);
        }
    }
}