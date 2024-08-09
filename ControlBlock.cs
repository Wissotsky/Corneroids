// Decompiled with JetBrains decompiler
// Type: CornerSpace.ControlBlock
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.GUI;
using CornerSpace.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class ControlBlock : PowerBlock, IMountable<Astronaut>
  {
    private short activatedButtons;
    private CameraBlock boundCamera;
    private List<ColorKeyGroup> colorKeyGroups;
    private int blockCount;
    private IController<Astronaut> mountedController;
    private Vector3 positionInEntitySpace;
    private BlockSector ownerSector;
    protected Dictionary<Color, HashSet<TriggerBlock>> controlledTriggerBlocks;

    public ControlBlock(ControlBlockType creationParameters)
      : base((PowerBlockType) creationParameters)
    {
      this.colorKeyGroups = new List<ColorKeyGroup>();
      this.controlledTriggerBlocks = new Dictionary<Color, HashSet<TriggerBlock>>();
      this.blockCount = 0;
      this.SetDefaultControlColors((int) creationParameters.NumberOfKeys);
    }

    public override void Added(
      SpaceEntity entity,
      BlockSector sector,
      Vector3 positionInEntitySpace)
    {
      this.positionInEntitySpace = positionInEntitySpace;
      this.ownerSector = sector;
      sector.ControlBlocks.Add(this);
      entity.ControlBlocks.Add(this);
      entity.OnControlBlockAdded(this);
    }

    public override void EditButtonClicked(Player player)
    {
      Astronaut astronaut = player.Astronaut;
      if (player is LocalPlayer && astronaut.ActiveItem != (Item) null && astronaut.ActiveItem is BindTriggerToControlTool && (astronaut.ActiveItem as BindTriggerToControlTool).ControlBlock == this)
      {
        Engine.LoadNewScreen((GameScreen) new ControlBlockKeygroupsScreen(this, player));
      }
      else
      {
        astronaut.ActiveItem = (Item) new BindTriggerToControlTool(this);
        player.Toolbar.SelectedIndex = (sbyte) -1;
      }
    }

    public virtual void BindBlock(TriggerBlock block, Color color, bool triggerEvents)
    {
      if (block == null || this.IsBlockBound(block))
        return;
      block.Controller = this;
      ++this.blockCount;
      if (this.controlledTriggerBlocks.ContainsKey(color))
        this.controlledTriggerBlocks[color].Add(block);
      else
        this.controlledTriggerBlocks.Add(color, new HashSet<TriggerBlock>()
        {
          block
        });
      if (this.NewBlockBound != null && triggerEvents)
        this.NewBlockBound(this, block, color);
      if (this.DataChanged == null)
        return;
      this.DataChanged();
    }

    public void BindCamera(CameraBlock camera, bool triggerEvents)
    {
      if (camera == null || this.boundCamera == camera)
        return;
      this.BindBlock((TriggerBlock) camera, Color.Magenta, triggerEvents);
      this.boundCamera = camera;
    }

    public override void Deactivate()
    {
      base.Deactivate();
      this.UnMount();
    }

    public Color? GetBlockColor(TriggerBlock block)
    {
      if (this.IsBlockBound(block))
      {
        foreach (Color key in this.controlledTriggerBlocks.Keys)
        {
          if (this.controlledTriggerBlocks[key].Contains(block))
            return new Color?(key);
        }
      }
      return new Color?();
    }

    public bool IsBlockBound(TriggerBlock block)
    {
      foreach (HashSet<TriggerBlock> triggerBlockSet in this.controlledTriggerBlocks.Values)
      {
        if (triggerBlockSet.Contains(block))
          return true;
      }
      return false;
    }

    public bool Mount(IController<Astronaut> controller)
    {
      if (this.mountedController != null)
        return false;
      this.mountedController = controller;
      controller.MountedTo = (IMountable<Astronaut>) this;
      controller.Controller.BoardedEntity = this.ownerSector.Owner;
      return true;
    }

    public void RebindBlock(TriggerBlock block, Color color, bool triggerEvents)
    {
      this.UnbindBlock(block, triggerEvents);
      this.BindBlock(block, color, triggerEvents);
    }

    public override void Removed(
      SpaceEntity entity,
      BlockSector sector,
      Vector3 positionInEntitySpace)
    {
      base.Removed(entity, sector, positionInEntitySpace);
      this.UnMount();
      sector.ControlBlocks.Add(this);
      entity.ControlBlocks.Remove(this);
      entity.OnControlBlockRemoved(this);
    }

    public void SetKeyForColor(Color color, Keys newKey, bool triggerEvents)
    {
      ColorKeyGroup colorKeyGroup = this.colorKeyGroups.Find((Predicate<ColorKeyGroup>) (g => g.Color == color));
      if (colorKeyGroup == null)
        return;
      bool flag = colorKeyGroup.Key != newKey;
      colorKeyGroup.Key = newKey;
      if (flag && this.KeyChanged != null && triggerEvents)
        this.KeyChanged(this, colorKeyGroup);
      if (this.DataChanged == null)
        return;
      this.DataChanged();
    }

    public bool UnbindBlock(TriggerBlock block, bool triggerEvents)
    {
      if (!this.IsBlockBound(block))
        return false;
      foreach (HashSet<TriggerBlock> triggerBlockSet in this.controlledTriggerBlocks.Values)
      {
        if (triggerBlockSet.Remove(block))
          --this.blockCount;
      }
      block.Controller = (ControlBlock) null;
      if (this.BlockUnbound != null && triggerEvents)
        this.BlockUnbound(this, block);
      if (this.DataChanged != null)
        this.DataChanged();
      return true;
    }

    public void UnbindCamera(bool triggerEvents)
    {
      if (this.boundCamera == null)
        return;
      this.UnbindBlock((TriggerBlock) this.boundCamera, triggerEvents);
      this.boundCamera = (CameraBlock) null;
    }

    public void UnMount()
    {
      if (this.mountedController != null)
      {
        this.mountedController.MountedTo = (IMountable<Astronaut>) null;
        this.mountedController = (IController<Astronaut>) null;
      }
      foreach (HashSet<TriggerBlock> triggerBlockSet in this.controlledTriggerBlocks.Values)
      {
        foreach (TriggerBlock triggerBlock in triggerBlockSet)
          triggerBlock.UnTrigger();
      }
    }

    public virtual void Update(InputFrame input, Astronaut updater)
    {
      if (this.boundCamera != null)
        this.boundCamera.UpdateCameraPosition();
      for (int index = 0; index < this.colorKeyGroups.Count; ++index)
      {
        Keys key = this.colorKeyGroups[index].Key;
        Color color = this.colorKeyGroups[index].Color;
        if (((input != null ? (input.KeyDown(key) ? 1 : 0) : 0) | ((1 << index & (int) this.activatedButtons) != 0 ? 1 : 0)) != 0)
        {
          this.activatedButtons |= (short) (1 << index);
          if (this.controlledTriggerBlocks.ContainsKey(color))
          {
            foreach (TriggerBlock triggerBlock in this.controlledTriggerBlocks[color])
              triggerBlock.Trigger();
          }
        }
        else if (this.controlledTriggerBlocks.ContainsKey(color))
        {
          foreach (TriggerBlock triggerBlock in this.controlledTriggerBlocks[color])
            triggerBlock.UnTrigger();
        }
      }
    }

    public override void UseButtonClicked(Player player)
    {
      this.Mount((IController<Astronaut>) player.Astronaut);
      player.Astronaut.Speed = Vector3.Zero;
      player.Astronaut.ActiveItem = (Item) null;
    }

    public event System.Action DataChanged;

    public event Action<ControlBlock, ColorKeyGroup> KeyChanged;

    public event Action<ControlBlock, TriggerBlock, Color> NewBlockBound;

    public event Action<ControlBlock, TriggerBlock> BlockUnbound;

    public short ActiveButtons
    {
      get => this.activatedButtons;
      set => this.activatedButtons = value;
    }

    public int BlockCount => this.blockCount;

    public CameraBlock CameraBlock => this.boundCamera;

    public override bool CanBeUsed => true;

    public SpriteImage Crosshair
    {
      get
      {
        return new SpriteImage(Engine.ContentManager.Load<Texture2D>("Textures/Sprites/Crosshairs/gunConsole"));
      }
    }

    public Dictionary<Color, HashSet<TriggerBlock>> ControlledBlocks
    {
      get => this.controlledTriggerBlocks;
    }

    public List<ColorKeyGroup> ColorKeyGroups => this.colorKeyGroups;

    public IController<Astronaut> MountedController => this.mountedController;

    public BlockSector OwnerSector
    {
      get => this.ownerSector;
      set => this.ownerSector = value;
    }

    public Vector3 PositionInEntitySpace
    {
      get => this.positionInEntitySpace;
      set => this.positionInEntitySpace = value;
    }

    public Vector3 Speed => this.ownerSector.Owner.Speed;

    protected virtual void SetDefaultControlColors(int number)
    {
      Color[] colorArray = new Color[10]
      {
        Color.Red,
        Color.Green,
        Color.LightBlue,
        Color.Yellow,
        Color.Violet,
        Color.Orange,
        Color.Brown,
        Color.Turquoise,
        Color.Pink,
        Color.DarkBlue
      };
      for (int index = 0; index < Math.Min(number, 10); ++index)
        this.colorKeyGroups.Add(new ColorKeyGroup(Keys.None, colorArray[index]));
    }
  }
}
