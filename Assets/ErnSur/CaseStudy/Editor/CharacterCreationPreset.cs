using System;
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace ErnSur.CaseStudy.Editor
{
    [CreateAssetMenu]
    class CharacterCreationPreset : ScriptableObject
    {
        public Preset iconPreset;
        public Preset colliderPreset;
        public RuntimeAnimatorController animatorController;
        public Material materialTemplate;
        public string characterPrefabsDirectory = "Assets/2_Prefabs/Characters";
        public string materialsDirectory = "Assets/1_Graphics/Materials";
        public string iconDirectory = "Assets/1_Graphics/Store/Icons";
        public string modelDirectory = "Assets/1_Graphics/Models/Characters";

        public StoreItemViewModel CreateNewCharacter(CharacterCreationArgs args, string fileName)
        {
            var path = Path.Combine(characterPrefabsDirectory, $"{fileName}.prefab");
            var prefab = CreateCharacterPrefab(args, path);
            CreateMaterialAsset(args.mainTexture);
            return prefab;
        }

        public void CreateNewCharacterWithUserNameInput(CharacterCreationArgs args, Action<StoreItemViewModel> onEnd)
        {
            var endEditAction = DynamicEndNameEditAction.New(path =>
            {
                var prefab = CreateCharacterPrefab(args, path);
                CreateMaterialAsset(args.mainTexture);
                onEnd?.Invoke(prefab);
            });
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
                endEditAction,
                Path.Combine(characterPrefabsDirectory, "NewCharacter.prefab"),
                (Texture2D)EditorGUIUtility.IconContent("PrefabVariant Icon").image,
                null);
        }

        void CreateMaterialAsset(Texture2D mainTexture)
        {
            if (mainTexture == null)
                return;
            var material = new Material(materialTemplate);
            material.mainTexture = mainTexture;
            var path = $"{materialsDirectory}/{mainTexture.name}.mat";
            AssetDatabase.CreateAsset(material, path);
            AssetDatabase.ImportAsset(path);
        }

        StoreItemViewModel CreateCharacterPrefab(CharacterCreationArgs args, string prefabPath)
        {
            var prefab = (GameObject)PrefabUtility.InstantiatePrefab(args.model);
            var iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(args.shopIcon));
            GameObject prefabAsset;
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

                prefabAsset = PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                AssetDatabase.ImportAsset(prefabPath);
            }
            finally
            {
                prefab.hideFlags = HideFlags.DontSaveInEditor;
                DestroyImmediate(prefab);
            }

            return prefab == null ? null : prefabAsset.GetComponent<StoreItemViewModel>();
        }
    }
}