using System;
using UnityEngine;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{

    public enum BlockType
    {
        STONE,GRASS
    }

    [Serializable]
    public class BlockTextureInfo
    {
        [SerializeField]
        public int id;
        [SerializeField]
        public BlockType Type;
        [SerializeField]
        public Vector2 vectorOnAtlas;
    }
}