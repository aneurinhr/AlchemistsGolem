using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlotNutrientPointers : MonoBehaviour
{

    public float[] xPositionValues;

    public RectTransform[] pointers;

    private void Start()
    {
        Deactivate();
    }

    public void ChangePosition(int value, int pointer)
    {
        int xPosition = value * -1;
        pointers[pointer].gameObject.SetActive(true);
        Vector3 newPosition = new Vector3(xPositionValues[xPosition], pointers[pointer].localPosition.y, pointers[pointer].localPosition.z);
        pointers[pointer].localPosition = newPosition;
    }

    public void Deactivate()
    {
        for (int i = 0; i < pointers.Length; i++)
        {
            pointers[i].gameObject.SetActive(false);
        }
    }
}
