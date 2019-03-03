using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.PolarisBasic
{
    /// <summary>
    /// Indicate an index in 2D grid with 2 component X, Z
    /// </summary>
    [System.Serializable]
    public struct Index2D
    {
        [SerializeField]
        private int x;
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        [SerializeField]
        private int z;
        public int Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
            }
        }

        public Index2D(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public static Index2D operator +(Index2D i1, Index2D i2)
        {
            return new Index2D(i1.x + i2.x, i1.z + i2.z);
        }

        public static Index2D operator -(Index2D i1, Index2D i2)
        {
            return new Index2D(i1.x - i2.x, i1.z - i2.z);
        }

        public static bool operator ==(Index2D i1, Index2D i2)
        {
            return i1.x == i2.x && i1.z == i2.z;
        }

        public static bool operator !=(Index2D i1, Index2D i2)
        {
            return i1.x != i2.x || i1.z != i2.z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", x, z);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Index2D))
            {
                return false;
            }

            var d = (Index2D)obj;
            return x == d.x &&
                   z == d.z;
        }

        public override int GetHashCode()
        {
            var hashCode = 1553271884;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }
    }
}