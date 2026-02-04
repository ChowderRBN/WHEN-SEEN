using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI slotNameText;
    public TextMeshProUGUI saveDateText;
    public TextMeshProUGUI playTimeText;
    public Button loadButton;
    public Button deleteButton;
    public GameObject emptySlotPanel;
    public GameObject saveDataPanel;

    private int slotNumber;
    private SaveData saveData;

    public void Initialize(int slot)
    {
        slotNumber = slot;
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (SaveSystem.SaveExists(slotNumber))
        {
            // Load and display save data
            saveData = SaveSystem.LoadGame(slotNumber);

            emptySlotPanel.SetActive(false);
            saveDataPanel.SetActive(true);

            slotNameText.text = "Save Slot " + slotNumber;
            saveDateText.text = saveData.saveDate;

            // Format play time
            int hours = Mathf.FloorToInt(saveData.playTime / 3600);
            int minutes = Mathf.FloorToInt((saveData.playTime % 3600) / 60);
            playTimeText.text = string.Format("Play Time: {0:00}h {1:00}m", hours, minutes);

            loadButton.interactable = true;
            deleteButton.interactable = true;
        }
        else
        {
            // Show empty slot
            emptySlotPanel.SetActive(true);
            saveDataPanel.SetActive(false);

            loadButton.interactable = false;
            deleteButton.interactable = false;
        }
    }

    public void OnLoadButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame(slotNumber);
        }
    }

    public void OnNewGameButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NewGame(slotNumber);
        }
    }

    public void OnDeleteButtonClicked()
    {
        SaveSystem.DeleteSave(slotNumber);
        RefreshDisplay();
    }
}