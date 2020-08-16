﻿namespace SMLHelper.V2.Assets
{
    using System;
    using System.Collections.Generic;
    using MonoBehaviours;
    using UnityEngine;

    /// <summary>
    /// The abstract class to inherit when you want to add new PreFabs into the game.
    /// </summary>
    public abstract class ModPrefab
    {
        private static readonly Dictionary<string, ModPrefab> FileNameDictionary = new Dictionary<string, ModPrefab>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Dictionary<string, ModPrefab> ClassIdDictionary = new Dictionary<string, ModPrefab>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly List<ModPrefab> PreFabsList = new List<ModPrefab>();
        internal static bool ModPrefabsPatched = false;

        internal static void Add(ModPrefab prefab)
        {
            FileNameDictionary.Add(prefab.PrefabFileName, prefab);
            ClassIdDictionary.Add(prefab.ClassID, prefab);
            PreFabsList.Add(prefab);
            ModPrefabsPatched = false;
        }

        internal static IEnumerable<ModPrefab> Prefabs => PreFabsList;
        internal static bool TryGetFromFileName(string classId, out ModPrefab prefab)
        {
            return FileNameDictionary.TryGetValue(classId, out prefab);
        }

        internal static bool TryGetFromClassId(string classId, out ModPrefab prefab)
        {
            return ClassIdDictionary.TryGetValue(classId, out prefab);
        }

        internal readonly string ModName;

        /// <summary>
        /// The class identifier used for the <see cref="PrefabIdentifier" /> component whenever applicable.
        /// </summary>
        public string ClassID { get; protected set; }

        /// <summary>
        /// Name of the prefab file.
        /// </summary>
        public string PrefabFileName { get; protected set; }

        /// <summary>
        /// The <see cref="TechType"/> of the corresponding item.
        /// Used for <see cref="Fixer" />, <see cref="TechTag" />, and <see cref="Constructable" /> components whenever applicable.
        /// </summary>
        public TechType TechType { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModPrefab" /> class.
        /// </summary>
        /// <param name="classId">The class identifier used for the <see cref="PrefabIdentifier" /> component whenever applicable.</param>
        /// <param name="prefabFileName">Name of the prefab file.</param>
        /// <param name="techType">The techtype of the corresponding item. 
        /// Used for the <see cref="Fixer" />, <see cref="TechTag" />, and <see cref="Constructable" /> components whenever applicable.
        /// Can also be set later in the constructor if it is not yet provided.</param>
        protected ModPrefab(string classId, string prefabFileName, TechType techType = TechType.None)
        {
            this.ClassID = classId;
            this.PrefabFileName = prefabFileName;
            this.TechType = techType;

            ModName = GetType().Assembly.GetName().Name;
        }

        internal GameObject GetGameObjectInternal()
        {
            GameObject go = GetGameObject();

            if (go == null)
            {
                return null;
            }

            go.transform.position = new Vector3(-5000, -5000, -5000);
            go.name = this.ClassID;

            if (this.TechType != TechType.None)
            {
                if (go.activeInHierarchy) // don't add Fixer to inactive prefabs
                    go.AddComponent<Fixer>();

                if (go.GetComponent<TechTag>() is TechTag tag)
                {
                    tag.type = this.TechType;
                }

                if (go.GetComponent<Constructable>() is Constructable cs)
                {
                    cs.techType = this.TechType;
                }
            }

            if (go.GetComponent<PrefabIdentifier>() is PrefabIdentifier pid)
            {
                pid.ClassId = this.ClassID;
            }

            return go;
        }

        /// <summary>
        /// Gets the prefab game object. Set up your prefab components here.
        /// The <see cref="TechType"/> and ClassID are already handled.
        /// </summary>
        /// <returns>The game object to be instantiated into a new in-game entity.</returns>
        public abstract GameObject GetGameObject();
    }
}
