using UnityEngine;

namespace Assets.Scripts.CustomObjects.VoxelEngine
{
    public class BlockInfo
    {
        private int type;
        private Vector2 location;

        public BlockInfo(int type, Vector2 location)
        {
            this.type = type;
            this.location = location;
        }

        public int Type
        {
            get { return type; }
        }

        public Vector2 Location
        {
            get { return location; }
        }
    }
}