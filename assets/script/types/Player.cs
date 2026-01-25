using System;
using Godot;

namespace AnimaParty.assets.script.types;

public class Player(int deviceId, Node3D player)
{
    private int deviceId=deviceId;
    //public int playerId { get; }=playerNumber;
    public Node3D player{get; set; } = player;
    public int DeviceId {get;} = deviceId;
    
    public void SetDeviceId(int deviceId)
    {
        if (DeviceId < 0)
        {
            this.deviceId = deviceId;
        }
    }
    
    /// <summary>
    /// Checks if the input event comes from this player's device and is acceptable.
    /// </summary>
    public bool IsEventFromDevice(InputEvent @event)
    {
        return @event.Device == DeviceId && IsAcceptableDevice(@event);
    }

    public bool IsConnected()
    {
        return DeviceId >= 0;
    }

    
    public bool LeftPressed(InputEvent @event)
    {
        return IsEventFromDevice(@event) &&
               @event.IsActionPressed("ui_left") ;
    }

    public bool RightPressed(InputEvent @event)
    {
        return IsEventFromDevice(@event) &&
               @event.IsActionPressed("ui_right") ;
    }
    
    public bool UpPressed(InputEvent @event)
    {
        return IsEventFromDevice(@event) &&
               @event.IsActionPressed("ui_up") ;
    }
    
    public bool DownPressed(InputEvent @event)
    {
        return IsEventFromDevice(@event) &&
               @event.IsActionPressed("ui_down") ;
    }
    public bool ConfirmPressed(InputEvent @event)
    {
        return IsEventFromDevice(@event) &&
               @event.IsActionPressed("ui_accept") ;
    }

    public bool CancelPressed(InputEvent @event)
    {
        return IsEventFromDevice(@event) &&
               @event.IsActionPressed("ui_cancel") ;
    }

    public static bool IsAcceptableDevice(InputEvent @event)
    {
        return !(@event is InputEventMouse || @event is InputEventKey);
    }

    public static Player Intanciate(int device)
    {
        return new Player(device, null);
    }
}