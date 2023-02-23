using HomaGames.Internal.Utilities;
using System;
using System.Collections.Generic;
using ErnSur.CaseStudy;
using UnityEngine;

[System.Serializable]
public class StoreItem
{
    public string Name;
    public int Id;
    public int Price;
    public StoreItemViewModel Prefab;
    public bool Valid => Prefab != null && Price >= 0;
}

public class Store : Singleton<Store>
{
    [SerializeField]
    private StoreLibrary storeLibrary;

    public Action<StoreItem> OnItemSelected;
    public List<StoreItem> StoreItems => storeLibrary.items;

    public void SelectItem(StoreItem item)
    {
        OnItemSelected?.Invoke(item);
    }
}