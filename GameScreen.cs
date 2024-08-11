// Decompiled with JetBrains decompiler
// Type: CornerSpace.GameScreen
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

#nullable disable
namespace CornerSpace
{
  public abstract class GameScreen : IDisposable
  {
    protected bool pausable;
    protected Color backgroundColor;
    private MouseDevice.Behavior mouseBehavior;
    private GameScreen exitThisScreen;
    private GameScreen.Type screenType;
    private IInput inputManager;
    private MouseDevice mouse;
    private KeyboardDevice keyboard;
    private InputFrame inputFrame;

    public GameScreen(
      GameScreen.Type screenType,
      bool pausable,
      MouseDevice.Behavior mouseBehavior)
    {
      this.pausable = pausable;
      this.mouseBehavior = mouseBehavior;
      this.screenType = screenType;
      this.backgroundColor = Color.Black;
    }

    public void CloseScreen()
    {
      this.exitThisScreen = this;
      this.Closing();
    }

    protected virtual void Closing()
    {
      if (this.ScreenClosing == null)
        return;
      this.ScreenClosing();
    }

    public virtual void Dispose()
    {
    }

    public void Initialize(IInput inputManager)
    {
      this.inputManager = inputManager;
      this.mouse = inputManager.GetMouseListener();
      this.keyboard = inputManager.GetKeyboardListener();
      this.inputFrame = inputManager.InputFrame;
    }

    public virtual void Load()
    {
    }

    public virtual void UpdateInput()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Render()
    {
    }

    public event System.Action ScreenClosing;

    public MouseDevice.Behavior MouseBehavior
    {
      get => this.mouseBehavior;
      set => this.mouseBehavior = value;
    }

    public bool Pausable => this.pausable;

    public GameScreen ExitScreen => this.exitThisScreen;

    public GameScreen.Type ScreenType => this.screenType;

    protected IInput InputManager => this.inputManager;

    protected MouseDevice Mouse => this.mouse;

    protected KeyboardDevice Keyboard => this.keyboard;

    protected InputFrame InputFrame => this.inputFrame;

    public enum Type
    {
      Popup,
      Fullscreen,
    }
  }
}
