using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace ErnSur.CaseStudy.Editor
{
    internal static class StoreLibraryEditorExtensions
    {
        public static void AddNewCharacter(this StoreLibrary lib, string characterName, StoreItemViewModel prefab)
        {
            var storeItem = lib.items.FirstOrDefault(i => i.Name == characterName);
            if (storeItem != null)
            {
                storeItem.Prefab = prefab;
                return;
            }

            PullNewContent(lib, newContentList =>
            {
                storeItem = newContentList.FirstOrDefault(i => i.Name == characterName);
                if (storeItem == null)
                {
                    storeItem = new StoreItem
                    {
                        Id = lib.items.Max(i => i.Id) + 1,
                        Name = characterName,
                        Prefab = prefab,
                        Price = -1
                    };
                }

                storeItem.Prefab = prefab;
                lib.items.Add(storeItem);
                EditorUtility.SetDirty(lib);
                Debug.Log($"Added new character ({characterName}) to the store library.");
            });
        }
        
        public static void PullNewContent(this StoreLibrary lib, Action<List<StoreItem>> onEnd)
        {
            EditorCoroutineUtility.StartCoroutine(PullNewContent(lib.sheetUrl, onEnd), lib);
        }
        
        static IEnumerator PullNewContent(string sheetUrl, Action<List<StoreItem>> onEnd)
        {
            var request = UnityWebRequest.Get(sheetUrl);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Get the downloaded data as a string
                string csvData = request.downloadHandler.text;

                var data = CsvParser.ToStoreItems(csvData);
                onEnd?.Invoke(data);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}