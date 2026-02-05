using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGamePanelManager : MonoBehaviour
{
    [Header("Save Slots")]
    public SaveSlotUI saveSlot1;
    public SaveSlotUI saveSlot2;
    public SaveSlotUI saveSlot3;

    void OnEnable()
    {
        // Initialize slots when panel is shown
        if (saveSlot1 != null) saveSlot1.Initialize(1);
        if (saveSlot2 != null) saveSlot2.Initialize(2);
        if (saveSlot3 != null) saveSlot3.Initialize(3);
    }
}