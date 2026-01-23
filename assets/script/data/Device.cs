namespace AnimaParty.assets.script.data;

public struct Device(int deviceId)
{
    private int _deviceId = deviceId;
    public int DeviceId => _deviceId;

    public int CompareTo(Device other)
    {
        if(this._deviceId>other._deviceId) return 1;
        if(this._deviceId<other._deviceId) return -1;
        return 0;
    }
}