// Decompiled with JetBrains decompiler
// Type: CornerSpace.InputReader
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace CornerSpace
{
  public class InputReader
  {
    private const int GWL_WNDPROC = -4;
    private const int WM_KEYDOWN = 256;
    private const int WM_KEYUP = 257;
    private const int WM_CHAR = 258;
    private const int WM_IME_SETCONTEXT = 641;
    private const int WM_INPUTLANGCHANGE = 81;
    private const int WM_GETDLGCODE = 135;
    private const int WM_IME_COMPOSITION = 271;
    private const int DLGC_WANTALLKEYS = 4;
    private int maxLength;
    private bool reading;
    private IntPtr wndProc;
    private InputReader.WndProc listenMessagesDelegate;
    private StringBuilder stringBuilder;

    [DllImport("user32.dll")]
    private static extern IntPtr CallWindowProc(
      IntPtr lpPrevWndFunc,
      IntPtr hWnd,
      uint Msg,
      IntPtr wParam,
      IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public InputReader(GameWindow gameWindow)
    {
      this.reading = false;
      this.maxLength = 100;
      this.stringBuilder = new StringBuilder(this.maxLength, this.maxLength);
      this.listenMessagesDelegate = new InputReader.WndProc(this.MessageRead);
      this.wndProc = SetWindowLongPtr(gameWindow.Handle, -4, Marshal.GetFunctionPointerForDelegate(this.listenMessagesDelegate));
    }

    public void Begin()
    {
      this.reading = true;
      this.stringBuilder.Remove(0, this.stringBuilder.Length);
    }

    public void End() => this.reading = false;

    public event Action<char> CharacterReadEvent;

    public string ReadString
    {
      set
      {
        this.stringBuilder.Remove(0, this.stringBuilder.Length);
        this.stringBuilder.Append(value);
      }
      get => this.stringBuilder.ToString();
    }

    private IntPtr MessageRead(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
      IntPtr num = InputReader.CallWindowProc(this.wndProc, hWnd, msg, wParam, lParam);
      switch (msg)
      {
        case 135:
          num = (IntPtr) (num.ToInt32() | 4);
          break;
        case 258:
          if (this.reading)
          {
            char ch = (char) (int) wParam;
            if (ch >= ' ' && ch <= '\u009A')
            {
              this.stringBuilder.Append((char) (int) wParam);
              if (this.CharacterReadEvent != null)
                this.CharacterReadEvent(ch);
              if (this.stringBuilder.Length >= this.maxLength)
                this.reading = false;
            }
            if (ch == '\b' && this.stringBuilder.Length > 0)
            {
              this.stringBuilder.Remove(this.stringBuilder.Length - 1, 1);
              break;
            }
            break;
          }
          break;
      }
      return num;
    }

    private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
  }
}
