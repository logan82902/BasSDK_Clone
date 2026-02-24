using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TriInspector;


namespace ThunderRoad
{
    public class LevelAreaModule : LevelModule
    {
        #region Constants

        private const string AREA_MANAGER_ROOT_NAME = "AreaManager";

        #endregion Constants

        #region InternalClass

        [Serializable]
        public class AreaSpawnerData
        {
            [Dropdown(nameof(GetAllAreaCollectionID))]
            public string areaCollectionId = "";
            public int numberCreature;
            public bool isSharedNPCAlert;
            public TriDropdownList<string> GetAllAreaCollectionID() => Catalog.GetDropdownAllID(Category.AreaCollection);
        }

        #endregion InternalClass

        #region PublicFields

        public int areaFullMemoryDepth = 100;
        public int areaLiteMemoryDepth = 100;
        public int areaCullDepth = 2;

        public List<AreaSpawnerData> areaByLength;

        #endregion PublicFields

        #region Methods

        public TriDropdownList<string> GetAllAreaID() => Catalog.GetDropdownAllID(Category.Area);


        #endregion Methods
    }
}
