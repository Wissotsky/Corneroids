// Decompiled with JetBrains decompiler
// Type: CornerSpace.StateFrame
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

#nullable disable
namespace CornerSpace
{
  public struct StateFrame
  {
    public int ActiveItemId;
    public sbyte ActiveToolbarIndex;
    public int BoardedEntityId;
    public short ControlButtonsStates;
    public int ButtonsStates;
    public Vector3 CenterOfMass;
    public short Health;
    public Quaternion Orientation;
    public Position3 Position;
    public Vector3 PositionInEntitySpace;
    public float Power;
    public Vector3 Speed;
    public Vector3 SpeedInEntitySpace;
    public float SimulationTime;
    public uint SynchronizationValue;
    public long Tick;

    public StateFrame(InputFrame input)
    {
      this.ActiveToolbarIndex = (sbyte) -1;
      this.ButtonsStates = 0;
      this.BoardedEntityId = -1;
      this.CenterOfMass = Vector3.Zero;
      this.ControlButtonsStates = (short) 0;
      this.Health = (short) 0;
      this.Orientation = Quaternion.Identity;
      this.ActiveItemId = -1;
      this.Position = Position3.Zero;
      this.PositionInEntitySpace = Vector3.Zero;
      this.Power = 0.0f;
      this.Speed = Vector3.Zero;
      this.SpeedInEntitySpace = Vector3.Zero;
      this.Tick = 0L;
      this.SimulationTime = 0.0f;
      this.SynchronizationValue = 0U;
      ControlsManager controlsManager = Engine.SettingsManager.ControlsManager;
      this.Forward = input.KeyDown(ControlsManager.SpecialKey.Forward);
      this.Backward = input.KeyDown(ControlsManager.SpecialKey.Backward);
      this.StrafeLeft = input.KeyDown(ControlsManager.SpecialKey.StrafeLeft);
      this.StrafeRight = input.KeyDown(ControlsManager.SpecialKey.StrafeRight);
      this.Up = input.KeyDown(ControlsManager.SpecialKey.Up);
      this.Down = input.KeyDown(ControlsManager.SpecialKey.Down);
      this.Boost = input.KeyDown(ControlsManager.SpecialKey.Boost);
      this.RollLeft = input.KeyDown(ControlsManager.SpecialKey.RollLeft);
      this.RollRight = input.KeyDown(ControlsManager.SpecialKey.RollRight);
      this.Rotate = input.KeyDown(ControlsManager.SpecialKey.OrientateBlock);
      this.Use = input.KeyDown(ControlsManager.SpecialKey.Use);
      this.Edit = input.KeyDown(ControlsManager.SpecialKey.Edit);
      this.CharacterScreen = input.KeyDown(ControlsManager.SpecialKey.Inventory);
      this.MouseLeft = input.LeftDown;
      this.MouseMiddle = input.MiddleDown;
      this.MouseRight = input.RightDown;
    }

    public void Initialize()
    {
      this.ButtonsStates = 0;
      this.CenterOfMass = Vector3.Zero;
      this.ControlButtonsStates = (short) 0;
      this.Orientation = Quaternion.Identity;
      this.ActiveItemId = -1;
      this.Position = Position3.Zero;
      this.Speed = Vector3.Zero;
      this.Tick = 0L;
      this.SimulationTime = 0.0f;
      this.SynchronizationValue = 0U;
    }

    public void SetUpControlButtons(ControlBlock controlBlock, InputFrame input)
    {
      this.ControlButtonsStates = (short) 0;
      if (controlBlock == null || input == null)
        return;
      for (int index = 0; index < Math.Min(16, controlBlock.ColorKeyGroups.Count); ++index)
      {
        Keys key = controlBlock.ColorKeyGroups[index].Key;
        Color color = controlBlock.ColorKeyGroups[index].Color;
        this.ControlButtonsStates |= (short) ((input.KeyDown(key) ? 1 : 0) << index);
      }
    }

    public bool Forward
    {
      get => (this.ButtonsStates & 1) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 1;
        else
          this.ButtonsStates &= -2;
      }
    }

    public bool Backward
    {
      get => (this.ButtonsStates & 2) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 2;
        else
          this.ButtonsStates &= -3;
      }
    }

    public bool StrafeLeft
    {
      get => (this.ButtonsStates & 4) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 4;
        else
          this.ButtonsStates &= -5;
      }
    }

    public bool StrafeRight
    {
      get => (this.ButtonsStates & 8) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 8;
        else
          this.ButtonsStates &= -9;
      }
    }

    public bool Up
    {
      get => (this.ButtonsStates & 16) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 16;
        else
          this.ButtonsStates &= -17;
      }
    }

    public bool Down
    {
      get => (this.ButtonsStates & 32) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 32;
        else
          this.ButtonsStates &= -33;
      }
    }

    public bool Boost
    {
      get => (this.ButtonsStates & 64) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 64;
        else
          this.ButtonsStates &= -65;
      }
    }

    public bool RollLeft
    {
      get => (this.ButtonsStates & 128) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 128;
        else
          this.ButtonsStates &= -129;
      }
    }

    public bool RollRight
    {
      get => (this.ButtonsStates & 256) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 256;
        else
          this.ButtonsStates &= -257;
      }
    }

    public bool MouseLeft
    {
      get => (this.ButtonsStates & 512) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 512;
        else
          this.ButtonsStates &= -513;
      }
    }

    public bool MouseRight
    {
      get => (this.ButtonsStates & 1024) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 1024;
        else
          this.ButtonsStates &= -1025;
      }
    }

    public bool MouseMiddle
    {
      get => (this.ButtonsStates & 2048) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 2048;
        else
          this.ButtonsStates &= -2049;
      }
    }

    public bool Rotate
    {
      get => (this.ButtonsStates & 4096) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 4096;
        else
          this.ButtonsStates &= -4097;
      }
    }

    public bool Edit
    {
      get => (this.ButtonsStates & 8192) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 8192;
        else
          this.ButtonsStates &= -8193;
      }
    }

    public bool Use
    {
      get => (this.ButtonsStates & 16384) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 16384;
        else
          this.ButtonsStates &= -16385;
      }
    }

    public bool CharacterScreen
    {
      get => (this.ButtonsStates & 32768) > 0;
      set
      {
        if (value)
          this.ButtonsStates |= 32768;
        else
          this.ButtonsStates &= -32769;
      }
    }
  }
}
