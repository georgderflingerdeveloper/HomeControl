﻿using LibUdp.BASIC.RECEIVE;
using HomeControl.ADVANCED_COMPONENTS;

namespace HomeControl.ROOMS.SLEEPING_ROOM.INTERFACE
{
    interface ISleepingRoomController
    {
        DataReceivingEventArgs RemoteControl(DataReceivingEventArgs e);
    }
}
