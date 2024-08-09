// Decompiled with JetBrains decompiler
// Type: CornerSpace.Networking.ServerSendWorldDataHandler
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Text;

#nullable disable
namespace CornerSpace.Networking
{
  public class ServerSendWorldDataHandler : SequencedEventHandler<NetworkServer>
  {
    private Action<bool> resultDelegate;
    private Player player;
    private World world;

    public ServerSendWorldDataHandler(
      NetConnection connection,
      NetworkServer server,
      World world,
      Player player,
      Action<bool> result)
      : base(connection, server)
    {
      ServerSendWorldDataHandler worldDataHandler = this;
      this.player = player;
      this.world = world;
      this.resultDelegate = result;
      this.SendItemset();
      this.AddNewSequenceFunction("Wait client to ack sent itemset", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 11)
          return;
        if (message.ReadBoolean())
        {
          worldDataHandler.SendPlayer();
          worldDataHandler.MoveToNextState();
        }
        else
        {
          result(false);
          worldDataHandler.Unregister();
        }
      })).AddNewSequenceFunction("Wait client to ack sent player data", NetIncomingMessageType.Data, (Action<NetIncomingMessage>) (message =>
      {
        if (message.ReadByte() != (byte) 11)
          return;
        if (message.ReadBoolean())
          worldDataHandler.resultDelegate(true);
        else
          result(false);
        worldDataHandler.Unregister();
      }));
    }

    public override void ErrorCaught(Exception e)
    {
    }

    private void SendItemset()
    {
      try
      {
        Itemset itemset = this.world.Itemset;
        BlockTextureAtlas blockTextureAtlas = this.world.BlockTextureAtlas;
        SpriteTextureAtlas spriteTextureAtlas = this.world.SpriteTextureAtlas;
        byte[] bytes = Encoding.UTF8.GetBytes(itemset.ItemsetXML.ToString());
        byte[] numArray1 = new byte[blockTextureAtlas.Texture.Width * blockTextureAtlas.Texture.Height * 4];
        byte[] numArray2 = new byte[spriteTextureAtlas.Texture.Width * spriteTextureAtlas.Texture.Height * 4];
        byte[] numArray3 = (byte[]) null;
        blockTextureAtlas.Texture.GetData<byte>(numArray1);
        spriteTextureAtlas.Texture.GetData<byte>(numArray2);
        if (blockTextureAtlas.LightValuesTexture != null)
        {
          numArray3 = new byte[blockTextureAtlas.LightValuesTexture.Width * blockTextureAtlas.Texture.Height * 4];
          blockTextureAtlas.LightValuesTexture.GetData<byte>(numArray3);
        }
        byte[] source1 = Gzip.Instance.Compress(bytes);
        byte[] source2 = Gzip.Instance.Compress(numArray1);
        byte[] source3 = Gzip.Instance.Compress(numArray2);
        byte[] source4 = Gzip.Instance.Compress(numArray3);
        NetOutgoingMessage message = this.networkInterface.CreateMessage();
        message.Write((byte) 10);
        message.Write(bytes.Length);
        message.Write(source1.Length);
        message.Write(source1);
        message.Write(spriteTextureAtlas.TextureUnitSizePixels);
        message.Write(blockTextureAtlas.TextureUnitSizePixels);
        message.Write(spriteTextureAtlas.Texture.Width);
        message.Write(spriteTextureAtlas.Texture.Height);
        message.Write(source3.Length);
        message.Write(source3);
        message.Write(blockTextureAtlas.Texture.Width);
        message.Write(blockTextureAtlas.Texture.Height);
        message.Write(source2.Length);
        message.Write(source2);
        message.Write(source4.Length);
        message.Write(source4);
        int num = (int) this.Key.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 2);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Error while compressing itemset data for sending: " + ex.Message);
        this.networkInterface.DisconnectUser(this.Key, "Server error: itemset compression failed");
      }
    }

    private void SendPlayer()
    {
      try
      {
        Position3 position = this.player.Astronaut.Position;
        Quaternion orientation = this.player.Astronaut.Orientation;
        byte[] source1 = this.player.Inventory.Serialize();
        byte[] source2 = this.player.Toolbar.Items.Serialize();
        byte[] source3 = this.player.Astronaut.Suit.Serialize();
        NetOutgoingMessage messageTemplate = this.networkInterface.CreateMessageTemplate(NetworkPeer.MessageID.WorldData);
        messageTemplate.Write(position.X);
        messageTemplate.Write(position.Y);
        messageTemplate.Write(position.Z);
        messageTemplate.Write(orientation.X);
        messageTemplate.Write(orientation.Y);
        messageTemplate.Write(orientation.Z);
        messageTemplate.Write(orientation.W);
        messageTemplate.Write(source1.Length);
        if (source1.Length > 0)
          messageTemplate.Write(source1);
        messageTemplate.Write(source2.Length);
        if (source2.Length > 0)
          messageTemplate.Write(source2);
        messageTemplate.Write(source3.Length);
        if (source3.Length > 0)
          messageTemplate.Write(source3);
        int num = (int) this.Key.SendMessage(messageTemplate, NetDeliveryMethod.ReliableOrdered, 2);
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Error while compressing player data for sending: " + ex.Message);
        this.networkInterface.DisconnectUser(this.Key, "Server error: player data compression failed");
      }
    }
  }
}
