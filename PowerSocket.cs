// Decompiled with JetBrains decompiler
// Type: CornerSpace.PowerSocket
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

#nullable disable
namespace CornerSpace
{
  public class PowerSocket
  {
    private Vector3int position;
    private PowerNetwork ownerNetwork;
    private PowerSocket[] connectedSockets;

    public PowerSocket(PowerNetwork owner, Vector3int position)
    {
      this.connectedSockets = new PowerSocket[3];
      this.ownerNetwork = owner;
      this.position = position;
    }

    public void ChangeOwner(PowerNetwork newOwner) => this.ownerNetwork = newOwner;

    public bool ConnectTo(PowerSocket socket)
    {
      if (socket == null || socket == this || !this.EligibleForConnection(socket) || this.ConnectedTo(socket) || !this.SpaceForNewConnection() || !socket.SpaceForNewConnection())
        return false;
      this.AddConnection(socket);
      socket.AddConnection(this);
      return true;
    }

    public void Disconnect(PowerSocket socket)
    {
      if (!this.ConnectedTo(socket))
        return;
      socket.RemoveConnection(this);
      this.RemoveConnection(socket);
    }

    public void DisconnectAll()
    {
      for (int index = 0; index < this.connectedSockets.Length; ++index)
      {
        if (this.connectedSockets[index] != null)
        {
          this.connectedSockets[index].RemoveConnection(this);
          this.RemoveConnection(this.connectedSockets[index]);
        }
      }
    }

    public void DisconnectFrom(PowerNetwork network)
    {
      if (network == null)
        return;
      for (int index = 0; index < this.connectedSockets.Length; ++index)
      {
        if (this.connectedSockets[index] != null && this.connectedSockets[index].ownerNetwork == network)
        {
          this.connectedSockets[index].RemoveConnection(this);
          this.RemoveConnection(this.connectedSockets[index]);
        }
      }
    }

    public PowerNetwork[] ConnectedNetworks
    {
      get
      {
        PowerNetwork[] connectedNetworks = new PowerNetwork[this.NumberOfConnections()];
        int num = 0;
        for (int index = 0; index < this.connectedSockets.Length; ++index)
        {
          if (this.connectedSockets[index] != null)
            connectedNetworks[num++] = this.connectedSockets[index].ownerNetwork;
        }
        return connectedNetworks;
      }
    }

    public PowerNetwork OwnerNetwork => this.ownerNetwork;

    public Vector3int Position
    {
      get => this.position;
      set => this.position = value;
    }

    private bool AddConnection(PowerSocket socket)
    {
      for (int index = 0; index < this.connectedSockets.Length; ++index)
      {
        if (this.connectedSockets[index] == null)
        {
          this.connectedSockets[index] = socket;
          return true;
        }
      }
      return false;
    }

    private bool ConnectedTo(PowerSocket socket)
    {
      for (int index = 0; index < this.connectedSockets.Length; ++index)
      {
        if (this.connectedSockets[index] == socket)
          return true;
      }
      return false;
    }

    private bool EligibleForConnection(PowerSocket socket)
    {
      return this.position.DistanceSquared(socket.position) == 1;
    }

    private int NumberOfConnections()
    {
      int num = 0;
      for (int index = 0; index < this.connectedSockets.Length; ++index)
        num += this.connectedSockets[index] != null ? 1 : 0;
      return num;
    }

    private void RemoveConnection(PowerSocket socket)
    {
      for (int index = 0; index < this.connectedSockets.Length; ++index)
      {
        if (this.connectedSockets[index] == socket)
          this.connectedSockets[index] = (PowerSocket) null;
      }
    }

    private bool SpaceForNewConnection()
    {
      for (int index = 0; index < this.connectedSockets.Length; ++index)
      {
        if (this.connectedSockets[index] == null)
          return true;
      }
      return false;
    }
  }
}
