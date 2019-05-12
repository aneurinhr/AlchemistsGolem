using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropMotherMangager : MonoBehaviour
{
    public CropMother[] mothers;
    
    public CropMother FindMotherOnName(string name)
    {
        for (int i = 0; i < mothers.Length; i++)
        {
            string tempName = mothers[i].name;

            if (tempName == name)
            {
                return mothers[i];
            }
        }

        return null;
    }
}
