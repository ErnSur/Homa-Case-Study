using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ErnSur.CaseStudy.Editor
{
    [CustomEditor(typeof(StoreLibrary))]
    public class StoreLibraryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            PullDataButton();

            base.OnInspectorGUI();
        }

        void PullDataButton()
        {
            if (GUILayout.Button("Download new data"))
            {
                ((StoreLibrary)target).PullNewContent(OnPulledNewContent);
            }
        }

        void OnPulledNewContent(List<StoreItem> storeItems)
        {
            var storeLibrary = (StoreLibrary)target;

            Undo.RecordObject(target, "Content Pull");
            foreach (var newItem in storeItems)
            {
                var entryWithTheSameName = storeLibrary.items.FirstOrDefault(i => i.Name == newItem.Name);
                if (entryWithTheSameName == null)
                {
                    storeLibrary.items.Add(newItem);
                }
                else
                {
                    entryWithTheSameName.Price = newItem.Price;
                }
            }
        }
    }
}