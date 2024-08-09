// Decompiled with JetBrains decompiler
// Type: CornerSpace.Block
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace CornerSpace
{
  public abstract class Block
  {
    protected byte blockType;
    protected byte healthPoints;
    protected byte orientation;
    protected byte visibility;

    public Block(BlockType creationParameters)
    {
      this.healthPoints = creationParameters.MaximumHealth;
      this.blockType = creationParameters.BlockId;
      this.orientation = (byte) 0;
    }

    public virtual void Added(
      SpaceEntity entity,
      BlockSector sector,
      Vector3 positionInEntitySpace)
    {
    }

    public virtual void CollidingWithObject(Vector3 collisionNormal)
    {
    }

    public virtual void CreateBlock(
      Byte3 position,
      out BlockVertex[] vertices,
      out short[] indices)
    {
      BlockBuilder.Factory.ConstructBlock(this, position, out vertices, out indices);
    }

    public virtual void Destroyed()
    {
    }

    public virtual void EditButtonClicked(Player player)
    {
    }

    public virtual BlockType GetBlockType()
    {
      return Engine.LoadedWorld.Itemset.BlockTypes[(int) this.blockType];
    }

    public abstract BlockTextureCoordinates GetTextureCoordinates();

    public virtual void Removed(
      SpaceEntity entity,
      BlockSector sector,
      Vector3 positionInEntitySpace)
    {
    }

    public virtual void Rotate(Vector3 rotation)
    {
      if (rotation == Vector3.Zero)
        return;
      float num1 = Math.Abs(rotation.X);
      float num2 = Math.Abs(rotation.Y);
      float num3 = Math.Abs(rotation.Z);
      if ((double) num1 > (double) num2 && (double) num1 > (double) num3)
      {
        if ((double) rotation.X > 0.0)
        {
          if ((Vector3) new Byte3((byte) 0, (byte) 0, (byte) 3) == (Vector3) this.OrientationYawPitchRoll)
            this.OrientationYawPitchRoll = new Byte3((byte) 0, (byte) 1, (byte) 3);
          else
            this.OrientationYawPitchRoll = new Byte3((byte) 0, (byte) 0, (byte) 3);
        }
        else if ((Vector3) new Byte3((byte) 0, (byte) 0, (byte) 1) == (Vector3) this.OrientationYawPitchRoll)
          this.OrientationYawPitchRoll = new Byte3((byte) 0, (byte) 1, (byte) 1);
        else
          this.OrientationYawPitchRoll = new Byte3((byte) 0, (byte) 0, (byte) 1);
      }
      else if ((double) num2 > (double) num1 && (double) num2 > (double) num3)
      {
        if ((double) rotation.Y > 0.0)
          this.OrientationYawPitchRoll = !((Vector3) new Byte3((byte) 0, (byte) 0, (byte) 0) == (Vector3) this.OrientationYawPitchRoll) ? new Byte3((byte) 0, (byte) 0, (byte) 0) : new Byte3((byte) 1, (byte) 0, (byte) 0);
        if ((double) rotation.Y >= 0.0)
          return;
        if ((Vector3) new Byte3((byte) 0, (byte) 0, (byte) 2) == (Vector3) this.OrientationYawPitchRoll)
          this.OrientationYawPitchRoll = new Byte3((byte) 1, (byte) 0, (byte) 2);
        else
          this.OrientationYawPitchRoll = new Byte3((byte) 0, (byte) 0, (byte) 2);
      }
      else
      {
        if ((double) num3 <= (double) num1 || (double) num3 <= (double) num2)
          return;
        if ((double) rotation.Z > 0.0)
        {
          if ((Vector3) new Byte3((byte) 0, (byte) 1, (byte) 0) == (Vector3) this.OrientationYawPitchRoll)
            this.OrientationYawPitchRoll = new Byte3((byte) 1, (byte) 0, (byte) 1);
          else
            this.OrientationYawPitchRoll = new Byte3((byte) 0, (byte) 1, (byte) 0);
        }
        else if ((Vector3) new Byte3((byte) 0, (byte) 3, (byte) 0) == (Vector3) this.OrientationYawPitchRoll)
          this.OrientationYawPitchRoll = new Byte3((byte) 3, (byte) 0, (byte) 1);
        else
          this.OrientationYawPitchRoll = new Byte3((byte) 0, (byte) 3, (byte) 0);
      }
    }

    public Byte3 RotateSizeVector(Byte3 size)
    {
      Matrix orientationMatrix = this.OrientationMatrix;
      Vector3 vector3 = Vector3.Transform((Vector3) size, orientationMatrix);
      return new Byte3((byte) Math.Round((double) Math.Abs(vector3.X)), (byte) Math.Round((double) Math.Abs(vector3.Y)), (byte) Math.Round((double) Math.Abs(vector3.Z)));
    }

    public virtual void UseButtonClicked(Player player)
    {
    }

    public virtual bool CanBeEdited => false;

    public virtual bool CanBeUsed => false;

    public virtual bool HasDynamicBehavior => false;

    public byte Id => this.blockType;

    public float Mass => this.GetBlockType().Mass;

    public short MaxHealthPoints => (short) this.GetBlockType().MaximumHealth;

    public virtual Vector3 ModelPlacement
    {
      get => Vector3.Transform(this.GetBlockType().ModelPlacement, this.OrientationQuaternion);
    }

    public Byte3 ModelSize
    {
      get
      {
        Byte3 modelSize = this.GetBlockType().ModelSize;
        byte x = modelSize.X;
        byte y = modelSize.Y;
        byte z = modelSize.Z;
        if (x == (byte) 1 && y == (byte) 1 && z == (byte) 1)
          return new Byte3((byte) 1, (byte) 1, (byte) 1);
        Matrix orientationMatrix = this.OrientationMatrix;
        Vector3 vector3 = Vector3.Transform(new Vector3((float) x, (float) y, (float) z), orientationMatrix);
        return new Byte3((byte) Math.Round((double) Math.Abs(vector3.X)), (byte) Math.Round((double) Math.Abs(vector3.Y)), (byte) Math.Round((double) Math.Abs(vector3.Z)));
      }
    }

    public virtual bool HasCollision => true;

    public byte HealthPoints
    {
      get => this.healthPoints;
      set => this.healthPoints = value;
    }

    public virtual byte Orientation
    {
      get => this.orientation;
      set => this.orientation = value;
    }

    public Matrix OrientationMatrix
    {
      get
      {
        Byte3 orientationYawPitchRoll = this.OrientationYawPitchRoll;
        return Matrix.CreateFromYawPitchRoll((float) orientationYawPitchRoll.X * 1.57079637f, (float) orientationYawPitchRoll.Y * 1.57079637f, (float) orientationYawPitchRoll.Z * 1.57079637f);
      }
    }

    public Quaternion OrientationQuaternion
    {
      get
      {
        Byte3 orientationYawPitchRoll = this.OrientationYawPitchRoll;
        return Quaternion.CreateFromYawPitchRoll((float) orientationYawPitchRoll.X * 1.57079637f, (float) orientationYawPitchRoll.Y * 1.57079637f, (float) orientationYawPitchRoll.Z * 1.57079637f);
      }
    }

    public Byte3 OrientationYawPitchRoll
    {
      get
      {
        return new Byte3((byte) ((uint) this.orientation & 3U), (byte) ((int) this.orientation >> 2 & 3), (byte) ((int) this.orientation >> 4 & 3));
      }
      set
      {
        this.orientation = (byte) ((int) value.X % 4 + ((int) value.Y % 4 << 2) + ((int) value.Z % 4 << 4));
      }
    }

    public Byte3 Size
    {
      get
      {
        Byte3 size = this.GetBlockType().Size;
        byte x = size.X;
        byte y = size.Y;
        byte z = size.Z;
        return x == (byte) 1 && y == (byte) 1 && z == (byte) 1 ? new Byte3((byte) 1, (byte) 1, (byte) 1) : this.RotateSizeVector(size);
      }
    }

    public virtual bool Transparent => this.GetBlockType().Transparent;

    public virtual byte Visibility
    {
      get => this.visibility;
      set => this.visibility = value;
    }

    public enum Face : byte
    {
      noneVisible = 0,
      xMinus = 1,
      xPlus = 2,
      yMinus = 4,
      yPlus = 8,
      zMinus = 16, // 0x10
      zPlus = 32, // 0x20
      allVisible = 63, // 0x3F
    }
  }
}
