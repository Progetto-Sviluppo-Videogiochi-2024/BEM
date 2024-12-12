using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : SlotBaseManager
{
    protected override void LoadSlotUI(Transform slot, string savedSlotName)
    {
        var gameData = saveLoadSystem.dataService.Load(savedSlotName);
        var slotSavedList = slot.Find("Saved");

        slotSavedList.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = $"Slot {gameData.nSlotSave}"; // "Slot nSlotSave"
        slotSavedList.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = gestoreScena.GetChapterBySceneName(gameData.currentSceneName); // "Capitolo: Nome Capitolo"
        slotSavedList.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = gameData.saveTime.ToString(); // "Data e Ora"

        ConfigureSlotButton(slotSavedList.GetComponentInChildren<Button>(), () => { DeleteSlotUI(slot); ConfigureEmptySlot(slot); });
        ConfigureSlotButton(slot.GetComponent<Button>(), () => OpenConfirmPanel(savedSlotName));
        SetMouseHoverSlot(slot);
    }

    public override void OnYesActionSlot()
    {
        saveLoadSystem.SaveGame(int.Parse(currentSlotOpened[^1].ToString())); // Ultimo carattere
        ListSlotUI();
    }

    protected override string GetConfirmMessage() =>
        saveLoadSystem.dataService.DoesSaveExist(currentSlotOpened)
            ? $"Vuoi sovrascrivere il salvataggio su questo slot ?"
            : $"Vuoi salvare il gioco su questo slot ?";
}
