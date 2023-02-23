using System;
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace ErnSur.CaseStudy.Editor
{
    static class PrefabImporter
    {
        const string CharacterColliderPresetPath = "Assets/0_Test/CharacterColliderPreset.preset";
        const string AnimatorControllerPath = "Assets/1_Graphics/AnimatorControllers/Controller.controller";

        public static StoreItemViewModel CreateCharacterPrefab(CharacterCreationArgs args, string prefabPath)
        {
            var animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(AnimatorControllerPath);
            var colliderPreset = AssetDatabase.LoadAssetAtPath<Preset>(CharacterColliderPresetPath);
            var prefab = (GameObject)PrefabUtility.InstantiatePrefab(args.model);
            var iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(args.shopIcon));
            try
            {
                prefab.AddComponent<StoreItemViewModel>().icon = iconSprite;
                var collider = prefab.AddComponent<CapsuleCollider>();
                colliderPreset.ApplyTo(collider);
                if (!prefab.TryGetComponent<Animator>(out var animator))
                {
                    // handle this case
                    throw new Exception("No animator");
                }

                animator.runtimeAnimatorController = animatorController;

                PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            }
            finally
            {
                Object.DestroyImmediate(prefab);
            }

            return AssetDatabase.LoadAssetAtPath<StoreItemViewModel>(prefabPath);
        }
    }
}