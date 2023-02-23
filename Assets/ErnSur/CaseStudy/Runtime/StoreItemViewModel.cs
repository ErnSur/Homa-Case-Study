using UnityEngine;

namespace ErnSur.CaseStudy
{
    // Moved view data to a component
    // This way we never bundle more than one view dependency to the store item list
    public class StoreItemViewModel : MonoBehaviour
    {
        public Sprite icon;
    }
}