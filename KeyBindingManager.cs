// Decompiled with JetBrains decompiler
// Type: CornerSpace.KeyBindingManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class KeyBindingManager : IDisposable
  {
    private HashSet<KeyBinding> keyBindings;
    private HashSet<ControlKey> controlKeys;

    public KeyBindingManager()
    {
      this.keyBindings = new HashSet<KeyBinding>();
      this.controlKeys = new HashSet<ControlKey>();
    }

    public void Dispose()
    {
      foreach (KeyBinding keyBinding in this.keyBindings)
        keyBinding.Dispose();
      this.keyBindings.Clear();
      this.controlKeys.Clear();
    }

    public KeyBinding GetKey(Keys key)
    {
      foreach (KeyBinding keyBinding in this.keyBindings)
      {
        if (keyBinding.ControlKey.Equals(key))
          return keyBinding;
      }
      return (KeyBinding) null;
    }

    public KeyBinding NewKey(Keys key)
    {
      KeyBinding key1 = this.GetKey(key);
      if (key1 != null)
        return key1;
      KeyboardControlKey key2 = new KeyboardControlKey(key);
      KeyBinding keyBinding = new KeyBinding((ControlKey) key2);
      this.controlKeys.Add((ControlKey) key2);
      this.keyBindings.Add(keyBinding);
      return keyBinding;
    }

    public bool RemoveKey(Keys key)
    {
      KeyBinding key1 = this.GetKey(key);
      if (key1 == null)
        return false;
      this.keyBindings.Remove(key1);
      this.controlKeys.Remove(key1.ControlKey);
      return true;
    }

    public void RemoveKeyBindingsToBlock(TriggerBlock block)
    {
      foreach (KeyBinding keyBinding in this.keyBindings)
      {
        if (keyBinding.IsBlockBound(block))
          keyBinding.UnbindBlock(block);
      }
    }

    public void Update(IKeyboardInterface keyboard, IController<Astronaut> controller)
    {
      foreach (ControlKey controlKey in this.controlKeys)
        controlKey.UpdateKeyState(keyboard);
      foreach (KeyBinding keyBinding in this.keyBindings)
        keyBinding.Activate(controller);
    }
  }
}
