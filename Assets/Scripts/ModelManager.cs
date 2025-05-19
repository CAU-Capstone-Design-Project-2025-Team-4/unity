using System.Collections.Generic;
using UnityEngine;

namespace Prism
{
    public class ModelManager
    {
        private readonly Dictionary<string, GameObject> loadedModels = new();
        
        public bool TryGetModel(string id, out GameObject model)
        {
            return loadedModels.TryGetValue(id, out model);
        }

        public void AddModel(string id, GameObject model)
        {
            loadedModels.Add(id, model);
        }

        public bool ContainsModel(string id)
        {
            return loadedModels.ContainsKey(id);
        }

        public void RemoveModel(string id)
        {
            loadedModels.Remove(id);
        }

        public GameObject GetModel(string id)
        {
            return loadedModels[id];
        }
    }
}