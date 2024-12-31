using System.Collections.Generic;

public interface IDataService
{
    void Save(GameData data, bool overwrite = true);
    GameData Load(string name);
    void Delete(string name);
    void DeleteAll();
    List<string> ListSaves();
    bool SearchSlotSaved(string savedSlot, string UIslot);
    bool DoesSaveExist(string slotFileName);
}
