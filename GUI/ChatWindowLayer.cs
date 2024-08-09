// Decompiled with JetBrains decompiler
// Type: CornerSpace.GUI.ChatWindowLayer
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace.GUI
{
  public class ChatWindowLayer : Layer
  {
    private const int cMaxLogSize = 200;
    private const int cMessageMaxLength = 64;
    private const int cNickMaxLength = 20;
    private List<ChatWindowLayer.ChatMessage> chatMessages;
    private int maxLines;
    private string messageBeingWritten;
    private Texture2D pointerTexture;
    private bool readingMessage;

    public ChatWindowLayer(Rectangle positionAndSize, int maxLineCount)
      : base(positionAndSize)
    {
      this.chatMessages = new List<ChatWindowLayer.ChatMessage>();
      this.maxLines = Math.Max(1, maxLineCount);
      this.readingMessage = false;
      this.pointerTexture = Engine.ContentManager.Load<Texture2D>("Textures/Sprites/blank");
    }

    public void AddChatMessage(string sender, string message, Color color)
    {
      if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(message))
        return;
      if (sender.Length > 20)
        sender = sender.Substring(0, 20);
      if (message.Length > 64)
        message = message.Substring(0, 64);
      if (this.chatMessages.Count > 200)
        this.chatMessages.RemoveAt(0);
      sender = ">> " + sender + ":";
      this.chatMessages.Add(new ChatWindowLayer.ChatMessage()
      {
        Name = sender,
        Message = message,
        Color = color
      });
    }

    public void BeginListeningInput(IInput input)
    {
      this.messageBeingWritten = string.Empty;
      this.readingMessage = true;
      input.InputReader.Begin();
    }

    public string EndListeningInput(IInput input, string sender)
    {
      this.readingMessage = false;
      input.InputReader.End();
      if (!string.IsNullOrEmpty(sender))
      {
        string readString = input.InputReader.ReadString;
        if (!string.IsNullOrEmpty(readString))
        {
          this.AddChatMessage(sender, readString, Color.White);
          return readString;
        }
      }
      return (string) null;
    }

    public override void Render()
    {
      SpriteBatch spriteBatch = Engine.SpriteBatch;
      spriteBatch.Begin();
      Vector2 position1 = this.Position + Vector2.UnitY * (float) this.maxLines * 20f;
      int num = 0;
      for (int index = this.chatMessages.Count - 1; index >= Math.Max(0, this.chatMessages.Count - this.maxLines); --index)
      {
        ChatWindowLayer.ChatMessage chatMessage = this.chatMessages[index];
        Vector2 vector2 = Engine.Font.MeasureString(chatMessage.Name);
        spriteBatch.DrawString(Engine.Font, chatMessage.Name, position1, chatMessage.Color);
        spriteBatch.DrawString(Engine.Font, chatMessage.Message, position1 + Vector2.UnitX * (vector2.X + 10f), chatMessage.Color);
        position1 -= Vector2.UnitY * 20f;
        ++num;
        if (num >= this.maxLines)
          break;
      }
      if (this.readingMessage)
      {
        Vector2 position2 = this.Position + Vector2.UnitY * (float) (this.maxLines + 1) * 20f;
        string text = ">> Chat: " + this.messageBeingWritten;
        spriteBatch.DrawString(Engine.Font, text, position2, Color.White);
        Vector2 vector2 = Engine.Font.MeasureString(text);
        spriteBatch.Draw(this.pointerTexture, new Rectangle((int) ((double) position2.X + (double) vector2.X), (int) position2.Y, 3, (int) vector2.Y), Color.White);
      }
      spriteBatch.End();
    }

    public void Update(IInput input)
    {
      if (input == null || !this.readingMessage)
        return;
      this.messageBeingWritten = input.InputReader.ReadString;
    }

    private class ChatMessage
    {
      public Color Color;
      public string Name;
      public string Message;
    }
  }
}
