using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DrawTestButton : MonoBehaviour
{
    public HandManager handManager;

    public void OnClick_DrawCard()
    {
        handManager.DrawCard();
    }
}