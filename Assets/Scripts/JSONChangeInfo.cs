
/// <summary>
/// This class is used as part of the json state diff system to provide information on what change triggered a callback
/// </summary>
public class JSONChangeInfo {
    public enum ChangeType {
        ADDED, DELETED, CHANGED
    }

    public JSONObject OldValue, NewValue;

    public string Path;

    public ChangeType Type;

    internal JSONChangeInfo(ChangeType type, string path, JSONObject oldVal, JSONObject newVal) {
        Type = type;
        Path = path;
        OldValue = oldVal;
        NewValue = newVal;
    }

}