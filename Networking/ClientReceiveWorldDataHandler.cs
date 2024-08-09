// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ClientReceiveWorldDataHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace.Networking
{
  public class ClientReceiveWorldDataHandler : SequencedEventHandler<NetworkClient>
  {
    private NetworkClientManager client;
    private Itemset itemset;
    private BlockTextureAtlas blockTextureAtlas;
    private SpriteTextureAtlas spriteTextureAtlas;
    private PlayerSerializedData playerData;

    public ClientReceiveWorldDataHandler(NetConnection connection, NetworkClientManager client)
      : base(connection, (NetworkClient) client)
    {
      this.client = client;
      this.AddNewSequenceFunction("Wait for itemset data", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 10)
          return;
        bool result = this.ReadItemsetData(message);
        if (result)
        {
          this.MoveToNextState();
        }
        else
        {
          this.client.Disconnect("Failed to recieve itemset!");
          this.Unregister();
        }
        this.Ack(result);
      })).AddNewSequenceFunction("wait for player data", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 10)
          return;
        bool result = this.ReadPlayerData(message);
        this.Ack(result);
        if (result)
          this.client.WorldDataLoaded(this.itemset, this.spriteTextureAtlas, this.blockTextureAtlas, this.playerData);
        else
          this.networkInterface.Disconnect("Failed to parse player data");
        this.Unregister();
      }));
    }

    public override void ErrorCaught(Exception e)
    {
    }

    private void Ack(bool result)
    {
      NetOutgoingMessage messageTemplate = this.client.CreateMessageTemplate(NetworkPeer.MessageID.WorldDataResponse);
      messageTemplate.Write(result);
      int num = (int) this.Key.SendMessage(messageTemplate, NetDeliveryMethod.ReliableOrdered, 2);
    }

    private bool ReadItemsetData(NetIncomingMessage message)
    {
      Texture2D blockTexture = (Texture2D) null;
      Texture2D texture = (Texture2D) null;
      Texture2D lightTexture = (Texture2D) null;
      try
      {
        int unzippedByteCount = message.ReadInt32();
        int numberOfBytes1 = message.ReadInt32();
        byte[] bytesToDecompress1 = message.ReadBytes(numberOfBytes1);
        int textureUnitSizePixels1 = message.ReadInt32();
        int textureUnitSizePixels2 = message.ReadInt32();
        int width1 = message.ReadInt32();
        int height1 = message.ReadInt32();
        int numberOfBytes2 = message.ReadInt32();
        byte[] bytesToDecompress2 = message.ReadBytes(numberOfBytes2);
        int width2 = message.ReadInt32();
        int height2 = message.ReadInt32();
        int numberOfBytes3 = message.ReadInt32();
        byte[] bytesToDecompress3 = message.ReadBytes(numberOfBytes3);
        int numberOfBytes4 = message.ReadInt32();
        byte[] bytesToDecompress4 = message.ReadBytes(numberOfBytes4);
        byte[] bytes = Gzip.Instance.Decompress(bytesToDecompress1, unzippedByteCount);
        byte[] data1 = Gzip.Instance.Decompress(bytesToDecompress3, width2 * height2 * 4);
        byte[] data2 = Gzip.Instance.Decompress(bytesToDecompress2, width1 * height1 * 4);
        byte[] data3 = (byte[]) null;
        if (numberOfBytes4 > 0)
          data3 = Gzip.Instance.Decompress(bytesToDecompress4, width2 * height2 * 4);
        this.itemset = new Itemset();
        if (!this.itemset.ParseFromXml(XDocument.Parse(Encoding.UTF8.GetString(bytes))))
          throw new Exception("The itemset could not be parsed.");
        texture = new Texture2D(Engine.GraphicsDevice, width1, height1, 1, TextureUsage.AutoGenerateMipMap, SurfaceFormat.Color);
        blockTexture = new Texture2D(Engine.GraphicsDevice, width2, height2, 0, TextureUsage.AutoGenerateMipMap, SurfaceFormat.Color);
        texture.SetData<byte>(data2);
        blockTexture.SetData<byte>(data1);
        for (int index = 0; index < data3.Length; ++index)
        {
          int num = (int) data3[index];
        }
        if (data3 != null)
        {
          lightTexture = new Texture2D(Engine.GraphicsDevice, width2, height2);
          lightTexture.SetData<byte>(data3);
        }
        this.blockTextureAtlas = new BlockTextureAtlas(blockTexture, lightTexture, textureUnitSizePixels2);
        this.spriteTextureAtlas = new SpriteTextureAtlas(texture, textureUnitSizePixels1);
        return true;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("World data received but data was invalid. " + ex.Message);
        lightTexture?.Dispose();
        blockTexture?.Dispose();
        texture?.Dispose();
        return false;
      }
    }

    private bool ReadPlayerData(NetIncomingMessage message)
    {
      try
      {
        this.playerData = new PlayerSerializedData();
        this.playerData.Position = new Position3(message.ReadInt64(), message.ReadInt64(), message.ReadInt64());
        this.playerData.Orientation = new Quaternion(message.ReadFloat(), message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
        int numberOfBytes1 = message.ReadInt32();
        if (numberOfBytes1 > 0)
          this.playerData.InventoryItems = message.ReadBytes(numberOfBytes1);
        int numberOfBytes2 = message.ReadInt32();
        if (numberOfBytes2 > 0)
          this.playerData.ToolbarItems = message.ReadBytes(numberOfBytes2);
        int numberOfBytes3 = message.ReadInt32();
        if (numberOfBytes3 > 0)
          this.playerData.SuitItems = message.ReadBytes(numberOfBytes3);
        return true;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to parse player data: " + ex.Message);
        return false;
      }
    }
  }
}
