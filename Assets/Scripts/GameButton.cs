using System;
using UnityEngine;

public static class GameButton
{
    private static InputCommand targetLockCommand = new InputCommand("Bumper");
    private static InputCommand rightThumbHorizontal = new InputCommand("Right Thumb Horizontal");
    private static InputCommand rightThumbVertical = new InputCommand("Right Thumb Vertical");
    private static InputCommand dPadLeft = new InputCommand("D-Pad Horizontal");
    private static InputCommand dPadRight = new InputCommand("D-Pad Horizontal");
    private static InputCommand dPadVertical = new InputCommand("D-Pad Vertical");

    public enum Command
    {
        TargetLock,
        RightThumbHorizontal,
        RightThumbVertical,
        DPadLeft,
        DPadRight,
        DPadVertical
    }

    public enum Status
    {
        Down,
        Pressed,
        Released,
        Up
    }

    public static Boolean IsPressed(Command command) 
    {
        return GetStatus(command) == Status.Pressed;
    }
    public static Boolean IsReleased(Command command)
    {
        return GetStatus(command) == Status.Released;
    }

    public static Status GetStatus(Command command)
    {
        switch (command) 
        {
            case Command.TargetLock:
                return targetLockCommand.GetStatus();
            case Command.DPadLeft:
                return dPadLeft.GetStatusNegative();
            case Command.DPadRight:
                return dPadRight.GetStatusPositive();
            case Command.DPadVertical:
                return dPadVertical.GetStatus();
        }

        return Status.Up;
    }

    public static float GetValue(Command command)
    {
        switch (command)
        {
            case Command.TargetLock:
                return targetLockCommand.GetValue();
            case Command.RightThumbHorizontal:
                return rightThumbHorizontal.GetValue();
            case Command.RightThumbVertical:
                return rightThumbVertical.GetValue();
            case Command.DPadLeft:
                return dPadLeft.GetValue();
            case Command.DPadVertical:
                return dPadVertical.GetValue();

        }
        return 0f;
    }

    private class InputCommand 
    {
        private readonly string axisName;
        public InputCommand(string name) 
        {
            this.axisName = name;
        }

        private bool isPressed = false;

        public Status GetStatus()
        {
            return GetStatusForRequirement(Input.GetAxisRaw(axisName) != 0);
        }
        public Status GetStatusNegative() 
        {
            return GetStatusForRequirement(Input.GetAxisRaw(axisName) < 0);
        }
        public Status GetStatusPositive() 
        {
            return GetStatusForRequirement(Input.GetAxisRaw(axisName) > 0);
        }

        private Status GetStatusForRequirement(bool meetsRequirements) 
        {
            if (meetsRequirements)
            {
                if (!isPressed)
                {
                    isPressed = true;
                    return Status.Pressed;
                }
                else
                {
                    return Status.Down;
                }
            }
            else
            {
                if (isPressed)
                {
                    isPressed = false;
                    return Status.Released;
                }
            }
            return Status.Up;
        }

        public float GetValue() 
        {
            return Input.GetAxisRaw(axisName);
        }
    }    
}
