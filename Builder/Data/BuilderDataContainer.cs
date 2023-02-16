using System;
using TravelMind.Blocks;
using TravelMind.Blocks.Core;
using TravelMind.Components.Core.Abstracts;
using UnityEngine;

namespace TravelMind.Builder.Data
{
    [CreateAssetMenu(menuName = "TravelMind/Container/Builder Data Container", fileName = "BuilderDataContainer",
        order = 0)]
    public class BuilderDataContainer : ScriptableObject
    {
        [SerializeField] private BuilderSaveData builderSaveData;

#if UNITY_EDITOR
        private void OnValidate()
        {
            Array.Resize(ref builderSaveData.blocksColumnsData, builderSaveData.columnsCount);
            Array.Resize(ref builderSaveData.componentsColumnsData, builderSaveData.columnsCount);

            for (var i = 0; i < builderSaveData.columnsCount; i++)
            {
                Array.Resize(ref builderSaveData.blocksColumnsData[i].blocks, builderSaveData.rowsCount);
                Array.Resize(ref builderSaveData.componentsColumnsData[i].components, builderSaveData.rowsCount);
            }
        }
#endif

        public BuilderLoadData LoadData()
        {
            var loadData = new BuilderLoadData
            {
                Blocks = new string[builderSaveData.columnsCount, builderSaveData.rowsCount],
                Components = new string[builderSaveData.columnsCount, builderSaveData.rowsCount]
            };

            //Check columns
            if (builderSaveData.blocksColumnsData == null ||
                builderSaveData.blocksColumnsData.Length != builderSaveData.columnsCount)
                throw new Exception(
                    "BuilderDataContainer: ShipBlocks: builderData.blocksColumnsData is null or length is not equal to builderData.columnsCount");
            if (builderSaveData.componentsColumnsData == null ||
                builderSaveData.componentsColumnsData.Length != builderSaveData.columnsCount)
                throw new Exception(
                    "BuilderDataContainer: ShipBlocks: builderData.componentsColumnsData is null or length is not equal to builderData.columnsCount");

            for (var i = 0; i < builderSaveData.columnsCount; i++)
            {
                //Check rows
                if (builderSaveData.blocksColumnsData[i].blocks == null ||
                    builderSaveData.blocksColumnsData[i].blocks.Length != builderSaveData.rowsCount)
                    throw new Exception(
                        "BuilderDataContainer: ShipBlocks: builderData.blocksColumnsData[i] is null or length is not equal to builderData.rowsCount");
                if (builderSaveData.componentsColumnsData[i].components == null ||
                    builderSaveData.componentsColumnsData[i].components.Length != builderSaveData.rowsCount)
                    throw new Exception(
                        "BuilderDataContainer: ShipBlocks: builderData.componentsColumnsData[i] is null or length is not equal to builderData.rowsCount");

                for (var j = 0; j < builderSaveData.rowsCount; j++)
                {
                    //Load block
                    loadData.Blocks[i, j] = builderSaveData.blocksColumnsData[i].blocks[j].id;
                    loadData.Components[i, j] = builderSaveData.componentsColumnsData[i].components[j].id;
                }
            }

            return loadData;
        }

        public void SaveData(ShipBlock[,] blocksToInsert, AComponent[,] componentsToInsert)
        {
            if (blocksToInsert == null || blocksToInsert.Length == 0 || 
                componentsToInsert == null || componentsToInsert.Length != blocksToInsert.Length)
                return;

            builderSaveData.columnsCount = blocksToInsert.GetLength(0);
            builderSaveData.rowsCount = blocksToInsert.GetLength(1);

            builderSaveData.blocksColumnsData = new BlocksColumnData[builderSaveData.columnsCount];
            builderSaveData.componentsColumnsData = new ComponentsColumnData[builderSaveData.rowsCount];

            for (var i = 0; i < builderSaveData.columnsCount; i++)
            {
                builderSaveData.blocksColumnsData[i] = new BlocksColumnData
                    {blocks = new BlockData[builderSaveData.rowsCount]};
                builderSaveData.componentsColumnsData[i] = new ComponentsColumnData
                    {components = new ExtensionData[builderSaveData.rowsCount]};

                for (var j = 0; j < builderSaveData.rowsCount; j++)
                {
                    if (blocksToInsert[i, j] == null)
                        continue;

                    builderSaveData.blocksColumnsData[i].blocks[j] = new BlockData {id = blocksToInsert[i, j].Id};

                    
                    if (componentsToInsert[i,j] == null)
                        continue;

                    builderSaveData.componentsColumnsData[i].components[j] =
                        new ExtensionData {id = componentsToInsert[i,j].Id};
                }
            }

            LoadData();
        }

        #region Structures

        public struct BuilderLoadData
        {
            public string[,] Blocks;
            public string[,] Components;
        }

        [Serializable]
        private struct BlockData
        {
            public string id;
        }

        [Serializable]
        private struct BlocksColumnData
        {
            public BlockData[] blocks;
        }

        [Serializable]
        private struct ExtensionData
        {
            public string id;
        }

        [Serializable]
        private struct ComponentsColumnData
        {
            public ExtensionData[] components;
        }

        [Serializable]
        private struct BuilderSaveData
        {
            public int columnsCount;
            public int rowsCount;
            public BlocksColumnData[] blocksColumnsData;
            public ComponentsColumnData[] componentsColumnsData;
        }

        #endregion
    }
}