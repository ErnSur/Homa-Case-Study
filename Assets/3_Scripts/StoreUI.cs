using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    [SerializeField]
    UIStoreItem itemUIPrefab;
    void Start()
    {
        foreach (StoreItem item in Store.Instance.StoreItems.Where(i=>i.Valid))
        {
            UIStoreItem listItem = Instantiate(itemUIPrefab, transform);
            listItem.Initialize(item);
        }
    }
}
