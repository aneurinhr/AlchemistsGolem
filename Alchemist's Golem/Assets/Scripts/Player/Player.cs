using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera look;

    public int money;
    //public Inventory inventory
    public bool isPlotHightlighted = false;
    public Plot highlightedPlot = null;

    private void Update()
    {
        Ray ray = look.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject lookingAt = hit.collider.gameObject;

            if (highlightedPlot != null)
            {
                highlightedPlot.Highlight(false);
            }

            if (lookingAt.GetComponent<Plot>())
            {
                highlightedPlot = lookingAt.GetComponent<Plot>();
                highlightedPlot.Highlight(true);
            }
        }
    }
}
