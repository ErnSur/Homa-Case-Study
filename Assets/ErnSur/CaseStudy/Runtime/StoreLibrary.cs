using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ErnSur.CaseStudy
{
    [CreateAssetMenu]
    public class StoreLibrary : ScriptableObject
    {
        [SerializeField]
        internal string sheetUrl;

        public List<StoreItem> items = new List<StoreItem>();
    }
}