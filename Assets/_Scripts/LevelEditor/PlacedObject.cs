﻿using System.Linq;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public abstract class PlacedObject : MonoBehaviour, IPlacedObject
    {
        public GameObject UnityObject { get { return gameObject; } }

        public virtual string Type { get { return GetType().Name; } }

        public abstract int[] Layers { get; }

        [UnityMessage]
        public void Start()
        {
            GetComponent<SpriteRenderer>().sortingLayerName = "LevelLayer" + Layers.Max(x => x);
        }

        public virtual string Serialize()
        {
            return "";
        }

        public virtual void Deserialize(string serialized)
        {
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
        }
    }
}