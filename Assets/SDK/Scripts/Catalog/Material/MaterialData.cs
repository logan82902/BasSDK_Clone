using UnityEngine;
using System;
using System.Collections.Generic;

using System.Collections;
using TriInspector;

namespace ThunderRoad
{
    [Serializable]
#if UNITY_EDITOR
    [EditorLoadPriority(5)]
#endif
    [DeclareBoxGroup("General", Title = "General")]
    [DeclareBoxGroup("Collision(s)", Title = "Collision(s)")]
    public class MaterialData : CatalogData
    {
        [Group("General")]
        [ReadOnly]
        [NonSerialized]
        public int physicMaterialHash;
        [Group("General")]
        public int apparelProtectionLevel = 0;
        [Group("General")]
        [ShowInInspector]
        [ReadOnly]
        [NonSerialized]
        [Tooltip("The color of the material in the ID map")]
        public List<Color> idMapColors = new();
        public IEnumerable GetAllIDMapColors()
        {
            TriDropdownList<int> list = new();
            List<GameData.IDMapColors> idmaps = Catalog.gameData.physicMaterialIDMapColours;
            foreach (GameData.IDMapColors idmap in idmaps)
            {
                list.Add(idmap.label, idmap.id);
            }
            return list;
        }
        [Group("General")]
        [DropdownList(nameof(GetAllIDMapColors))]
        public List<int> idMapColorIds = new();

        [Group("General")]
        public List<EffectBundle> defaultEffects = new();
        protected Collision defaultCollision;

        [Group("General")]
        public bool isMetal;

        [Group("Collision(s)")]
        public List<Collision> collisions = new();

        [Serializable]
        [DeclareHorizontalGroup("Split")]
        [DeclareBoxGroup("Split/Materials", Title = "Materials")]
        [DeclareBoxGroup("Split/Effect(s)", Title = "Effect(s)")]
        public class Collision
        {
            [NonSerialized]
            public MaterialData sourceMaterialData;

            [Group("Split/Materials")]
            [DropdownList(nameof(GetAllMaterialID))]
            [LabelText("Target")]
            public List<string> targetMaterialIds = new();
            [NonSerialized]
            public List<MaterialData> targetMaterials = new List<MaterialData>();

            [Group("Split/Effect(s)")]
            [LabelText("EffectID / Ignored modules")]
            public List<EffectBundle> effects = new();

            public TriDropdownList<string> GetAllMaterialID() => Catalog.GetDropdownAllID(Category.Material, "Default");

            public enum Parenting
            {
                None,
                SourceCollider,
                TargetCollider,
            }

            public enum Align
            {
                Normal,
                Velocity,
            }

            public enum Rotation
            {
                SourceCollider,
                TargetCollider,
                Random,
            }

            public void OnCatalogRefresh(MaterialData source)
            {
                sourceMaterialData = source;

                targetMaterials = new List<MaterialData>();
                foreach (string materialId in targetMaterialIds)
                {
                    MaterialData materialData = Catalog.GetData<MaterialData>(materialId);
                    if (materialData != null) targetMaterials.Add(materialData);
                }

                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    effects[i].OnCatalogRefresh();
                    if (Application.isPlaying && effects[i].effectData == null)
                    {
                        effects.RemoveAt(i);
                    }
                }
            }
        }
        public override int GetCurrentVersion()
        {
            // v4 idmap colour changes
            return 3; 
        }
        
    }
}