// Decompiled with JetBrains decompiler
// Type: CornerSpace.ProjectileType
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

#nullable disable
namespace CornerSpace
{
  public class ProjectileType
  {
    private byte id;
    private Color color;
    private byte damage;
    private int lifetime;
    private float ricochetAngle;
    private float speed;
    private Vector3 size;
    public ProjectileVertex[] PremadeVertices;
    public ushort[] PremadeIndices;

    public ProjectileType()
    {
      this.id = (byte) 0;
      this.damage = (byte) 0;
      this.lifetime = 300;
      this.size = new Vector3(0.2f, 0.5f, 0.2f);
      this.speed = 0.2f;
    }

    public void LoadFromXml(XElement element)
    {
      XmlReader instance = XmlReader.Instance;
      this.id = instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "projectileId", (byte) 0, (string[]) null);
      this.damage = instance.ReadElementValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), (byte) 0, "damage");
      this.lifetime = instance.ReadElementValue<int>(element, new XmlReader.ConvertValue<int>(instance.ReadInt), 0, "lifetime");
      this.ricochetAngle = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 0.0f, "ricochetAngle");
      this.speed = instance.ReadElementValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), 1f, "speed");
      this.size = new Vector3(instance.ReadAttributeValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), "w", 0.1f, "size"), instance.ReadAttributeValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), "h", 0.1f, "size"), instance.ReadAttributeValue<float>(element, new XmlReader.ConvertValue<float>(instance.ReadFloat), "d", 0.1f, "size"));
      this.color = new Color(instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "r", byte.MaxValue, "color"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "g", (byte) 0, "color"), instance.ReadAttributeValue<byte>(element, new XmlReader.ConvertValue<byte>(instance.ReadByte), "b", byte.MaxValue, "color"), byte.MaxValue);
      this.CreateProjectileStructure(this.size);
    }

    public Color Color
    {
      get => this.color;
      set => this.color = value;
    }

    public byte Damage
    {
      get => this.damage;
      set => this.damage = value;
    }

    public byte Id => this.id;

    public int Lifetime
    {
      get => this.lifetime;
      set => this.lifetime = value;
    }

    public float RicochetAngle
    {
      get => this.ricochetAngle;
      set => this.ricochetAngle = value;
    }

    public Vector3 Size
    {
      get => this.size;
      set => this.size = value;
    }

    public float Speed
    {
      get => this.speed;
      set => this.speed = value;
    }

    public void CreateProjectileStructure(Vector3 size)
    {
      Color white = Color.White;
      this.PremadeVertices = new ProjectileVertex[8];
      this.PremadeIndices = new ushort[36];
      float x = size.X * 0.5f;
      float y = size.Y * 0.5f;
      float z = size.Z * 0.5f;
      Vector3 position1 = new Vector3(-x, -y, -z);
      Vector3 position2 = new Vector3(x, -y, -z);
      Vector3 position3 = new Vector3(x, -y, z);
      Vector3 position4 = new Vector3(-x, -y, z);
      Vector3 position5 = new Vector3(-x, y, -z);
      Vector3 position6 = new Vector3(x, y, -z);
      Vector3 position7 = new Vector3(x, y, z);
      Vector3 position8 = new Vector3(-x, y, z);
      this.PremadeVertices[0] = new ProjectileVertex(position1, white);
      this.PremadeVertices[1] = new ProjectileVertex(position2, white);
      this.PremadeVertices[2] = new ProjectileVertex(position3, white);
      this.PremadeVertices[3] = new ProjectileVertex(position4, white);
      this.PremadeVertices[4] = new ProjectileVertex(position5, white);
      this.PremadeVertices[5] = new ProjectileVertex(position6, white);
      this.PremadeVertices[6] = new ProjectileVertex(position7, white);
      this.PremadeVertices[7] = new ProjectileVertex(position8, white);
      this.PremadeIndices[0] = (ushort) 0;
      this.PremadeIndices[1] = (ushort) 2;
      this.PremadeIndices[2] = (ushort) 1;
      this.PremadeIndices[3] = (ushort) 0;
      this.PremadeIndices[4] = (ushort) 3;
      this.PremadeIndices[5] = (ushort) 2;
      this.PremadeIndices[6] = (ushort) 4;
      this.PremadeIndices[7] = (ushort) 5;
      this.PremadeIndices[8] = (ushort) 6;
      this.PremadeIndices[9] = (ushort) 4;
      this.PremadeIndices[10] = (ushort) 6;
      this.PremadeIndices[11] = (ushort) 7;
      this.PremadeIndices[12] = (ushort) 0;
      this.PremadeIndices[13] = (ushort) 1;
      this.PremadeIndices[14] = (ushort) 5;
      this.PremadeIndices[15] = (ushort) 0;
      this.PremadeIndices[16] = (ushort) 5;
      this.PremadeIndices[17] = (ushort) 4;
      this.PremadeIndices[18] = (ushort) 2;
      this.PremadeIndices[19] = (ushort) 3;
      this.PremadeIndices[20] = (ushort) 7;
      this.PremadeIndices[21] = (ushort) 2;
      this.PremadeIndices[22] = (ushort) 7;
      this.PremadeIndices[23] = (ushort) 6;
      this.PremadeIndices[24] = (ushort) 1;
      this.PremadeIndices[25] = (ushort) 2;
      this.PremadeIndices[26] = (ushort) 6;
      this.PremadeIndices[27] = (ushort) 1;
      this.PremadeIndices[28] = (ushort) 6;
      this.PremadeIndices[29] = (ushort) 5;
      this.PremadeIndices[30] = (ushort) 3;
      this.PremadeIndices[31] = (ushort) 0;
      this.PremadeIndices[32] = (ushort) 4;
      this.PremadeIndices[33] = (ushort) 3;
      this.PremadeIndices[34] = (ushort) 4;
      this.PremadeIndices[35] = (ushort) 7;
    }
  }
}
