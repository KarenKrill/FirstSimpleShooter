using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model.InventoryItems
{
    [Serializable]
    public class InventoryItem : ScriptableObject
    {
        [field: Header("Common")]

        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        public string Description { get; private set; }

        [field: SerializeField, Min(0)]
        public float Weight { get; private set; }

        [field: SerializeField, Min(1)]
        public int MaxStackCount { get; private set; }

        [field: SerializeField]
        public Sprite Icon {  get; private set; }
    }
}
