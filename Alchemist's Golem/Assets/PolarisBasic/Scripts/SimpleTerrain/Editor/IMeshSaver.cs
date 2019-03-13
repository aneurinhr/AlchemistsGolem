using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.PolarisBasic
{
    public interface IMeshSaver
    {
        void Save(Mesh m, Material mat, string path);
    }
}