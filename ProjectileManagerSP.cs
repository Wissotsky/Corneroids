// Decompiled with JetBrains decompiler
// Type: CornerSpace.ProjectileManagerSP
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using CornerSpace.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class ProjectileManagerSP : ProjectileManager
  {
    private const uint maxProjectileCount = 512;
    private const int maxProjectilesPerDrawCall = 50;
    private List<Projectile> projectiles;
    private bool updateRequired;
    private Vector3[] projectilePositions;
    private ushort[] indices;
    private ProjectileVertex[] vertices;
    private Effect effect;
    private DynamicIndexBuffer projectileIndexBuffer;
    private DynamicVertexBuffer projectileVertexBuffer;
    private int bufferWritePosition;
    private int bufferRenderPosition;

    public ProjectileManagerSP(
      SpaceEntityManager entityManager,
      EnvironmentManager environmentManager,
      LightingManager lightingManager)
      : base(entityManager, environmentManager, lightingManager)
    {
      this.projectiles = new List<Projectile>();
      this.updateRequired = false;
      this.projectilePositions = new Vector3[50];
      this.effect = Engine.ShaderPool[5];
      this.indices = new ushort[18432];
      this.vertices = new ProjectileVertex[4096];
      try
      {
      this.projectileIndexBuffer = new DynamicIndexBuffer(Engine.GraphicsDevice, typeof(ushort), 147456, BufferUsage.WriteOnly);
      this.projectileVertexBuffer = new DynamicVertexBuffer(Engine.GraphicsDevice, typeof(ProjectileVertex), 4096, BufferUsage.WriteOnly);
      }
      catch (Exception ex)
      {
      Engine.Console.WriteErrorLine("Failed to create buffers for particles: " + ex.Message);
      if (this.projectileIndexBuffer != null)
        this.projectileIndexBuffer.Dispose();
      if (this.projectileVertexBuffer != null)
        this.projectileVertexBuffer.Dispose();
      this.projectileIndexBuffer = null;
      this.projectileVertexBuffer = null;
      }
    }

    public override bool AddProjectile(Projectile projectile)
    {
      if (this.projectiles.Count >= 512)
        return false;
      int vIndex;
      int iIndex;
      this.GetIndexValues(this.projectiles.Count, out vIndex, out iIndex);
      int index = this.projectiles.Count % 50;
      this.CreateProjectileStructure(projectile, vIndex, iIndex, index);
      this.projectiles.Add(projectile);
      this.updateRequired = true;
      return true;
    }

    public override void Dispose()
    {
      if (this.projectileIndexBuffer != null && !this.projectileIndexBuffer.IsDisposed)
        this.projectileIndexBuffer.Dispose();
      if (this.projectileVertexBuffer == null || this.projectileVertexBuffer.IsDisposed)
        return;
      this.projectileVertexBuffer.Dispose();
    }

    public override void Render(IRenderCamera camera)
    {
      int num1 = this.projectiles.Count * 8;
      int elementCount = this.projectiles.Count * 36;
      if (num1 <= 0 || elementCount <= 0)
        return;
      if (this.updateRequired)
      {
        try
        {
          if (this.bufferWritePosition == 0 || this.bufferWritePosition + this.projectiles.Count >= 2048)
          {
            this.projectileVertexBuffer.SetData<ProjectileVertex>(this.vertices, 0, num1, SetDataOptions.Discard);
            this.projectileIndexBuffer.SetData<ushort>(this.indices, 0, elementCount, SetDataOptions.Discard);
            this.bufferWritePosition = this.projectiles.Count;
            this.bufferRenderPosition = 0;
          }
          else
          {
            this.projectileVertexBuffer.SetData<ProjectileVertex>(this.bufferWritePosition * 8 * (int)ProjectileVertex.SizeInBytes, this.vertices, 0, num1, (int)ProjectileVertex.SizeInBytes, SetDataOptions.NoOverwrite);
            this.projectileIndexBuffer.SetData<ushort>(this.bufferWritePosition * 36 * 2, this.indices, 0, elementCount, SetDataOptions.NoOverwrite);
            this.bufferRenderPosition = this.bufferWritePosition;
            this.bufferWritePosition += this.projectiles.Count;
          }
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to set data to projectile buffers: " + ex.Message);
        }
        this.updateRequired = false;
      }
      GraphicsDevice graphicsDevice = Engine.GraphicsDevice;
      //graphicsDevice.VertexDeclaration = Engine.VertexDeclarationPool[5];
      //graphicsDevice.Vertices[0].SetSource((VertexBuffer) this.projectileVertexBuffer, 0, (int) ProjectileVertex.SizeInBytes);
      graphicsDevice.SetVertexBuffer(this.projectileVertexBuffer);
      graphicsDevice.Indices = this.projectileIndexBuffer;
      graphicsDevice.DepthStencilState = DepthStencilState.Default;
      graphicsDevice.RasterizerState = RasterizerState.CullNone;
      graphicsDevice.BlendState = BlendState.Opaque;
      this.effect.Parameters["View"].SetValue(camera.ViewMatrix);
      this.effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
      double num2 = Math.Ceiling((double)this.projectiles.Count / 50.0);
      for (int index1 = 0; (double)index1 < num2; ++index1)
      {
        int num3 = index1 * 50;
        int num4 = Math.Min((index1 + 1) * 50, this.projectiles.Count) - num3;
        for (int index2 = 0; index2 < num4; ++index2)
          this.projectilePositions[index2] = camera.GetPositionRelativeToCamera(this.projectiles[num3 + index2].Position);
        this.effect.Parameters["TransformVectors"].SetValue(this.projectilePositions);
        try
        {
          this.effect.CurrentTechnique.Passes[0].Apply();
          graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, this.bufferRenderPosition * 8, 0, num1, 1800 * index1 + this.bufferRenderPosition * 36, num4 * 12);
        }
        catch (Exception ex)
        {
          Engine.Console.WriteErrorLine("Failed to render projectiles: " + ex.Message);
        }
      }
    }

    public override void Update()
    {
      long deltaTimeMs = Engine.FrameCounter.DeltaTimeMS;
      for (int index = this.projectiles.Count - 1; index >= 0; --index)
      {
        bool flag = false;
        Projectile projectile = this.projectiles[index];
        projectile.Update();
        Position3? nullable1 = new Position3?();
        foreach (SpaceEntity closeEntity in this.entityManager.GetCloseEntities(projectile.Position, 5f))
        {
          Position3? nullable2 = CollisionManager.Instance.CheckEntityProjectileCollision(closeEntity, projectile);
          if (nullable2.HasValue)
          {
            BlockCell blockCell = closeEntity.GetBlockCell(nullable2.Value);
            if (blockCell != null)
            {
              int num = (int) Math.Min(blockCell.Block.HealthPoints, projectile.DamageLeft);
              projectile.DamageLeft -= (byte) num;
              blockCell.Block.HealthPoints -= (byte) num;
              if (blockCell.Block.HealthPoints == (byte) 0)
                nullable1 = nullable2;
              if (projectile.DamageLeft == (byte) 0 || blockCell.Block.HealthPoints == (byte) 0)
                this.environmentManager.AddExplosion(projectile.Position - projectile.Speed * Engine.FrameCounter.DeltaTime, projectile.ProjectileType);
              if (projectile.DamageLeft == (byte) 0)
              {
                this.RemoveProjectile(index);
                flag = true;
                break;
              }
            }
          }
        }
        if (nullable1.HasValue)
          this.entityManager.DestroyBlock(nullable1.Value);
        if (!flag)
        {
          if (this.characterManager != null)
          {
            foreach (CharacterManager.CharacterContainer character1 in this.characterManager.Characters)
            {
              AICharacter character2 = character1.Character;
              Position3? nullable3 = CollisionManager.Instance.CheckCharacterProjectileCollision((Character) character2, this.projectiles[index]);
              if (nullable3.HasValue)
              {
                this.CollisionResponseWithCharacter((Character) character2, projectile, nullable3.Value);
                character2.ShotBy(projectile);
              }
              if (projectile.DamageLeft <= (byte) 0)
                break;
            }
            foreach (Player player in this.characterManager.Players)
            {
              if (player.Astronaut.Alive)
              {
                Position3? nullable4 = CollisionManager.Instance.CheckCharacterProjectileCollision((Character) player.Astronaut, projectile);
                if (nullable4.HasValue)
                {
                  this.CollisionResponseWithCharacter((Character) player.Astronaut, projectile, nullable4.Value);
                  player.DamageTaken(projectile.Position);
                  break;
                }
              }
            }
          }
          if (this.projectiles[index].Lifetime <= 0)
            this.RemoveProjectile(index);
        }
      }
    }

    protected virtual void CollisionResponseWithCharacter(
      Character character,
      Projectile projectile,
      Position3 position)
    {
      int num = (int) Math.Min(character.Health, (short) projectile.DamageLeft);
      character.ReduceHealth(num);
      projectile.DamageLeft -= (byte) num;
      this.environmentManager.AddBlood(position);
      if (projectile.DamageLeft > (byte) 0)
        return;
      this.environmentManager.AddExplosion(position, projectile.ProjectileType);
      projectile.Lifetime = 0;
    }

    private void CreateProjectileStructure(
      Projectile projectile,
      int vOffset,
      int iOffset,
      int index)
    {
      if (vOffset < 0 || vOffset + 8 >= this.vertices.Length || iOffset < 0 || iOffset + 36 >= this.indices.Length)
        return;
      ProjectileType projectileType = projectile.ProjectileType;
      Array.Copy((Array) projectileType.PremadeVertices, 0, (Array) this.vertices, vOffset, projectileType.PremadeVertices.Length);
      Array.Copy((Array) projectileType.PremadeIndices, 0, (Array) this.indices, iOffset, projectileType.PremadeIndices.Length);
      Vector3 vector3 = (double) projectile.Speed.LengthSquared() > 0.0 ? Vector3.Normalize(projectile.Speed) : Vector3.Up;
      Vector3 axis = Vector3.Cross(Vector3.Up, vector3);
      axis = (double) axis.LengthSquared() > 0.0 ? Vector3.Normalize(axis) : Vector3.Zero;
      float angle = (float) Math.Acos((double) Vector3.Dot(vector3, Vector3.Up));
      Matrix matrix = axis != Vector3.Zero ? Matrix.CreateFromAxisAngle(axis, angle) : Matrix.Identity;
      for (int index1 = vOffset; index1 < vOffset + 8; ++index1)
      {
        this.vertices[index1].Index = (float) index;
        this.vertices[index1].Color = projectileType.Color;
        this.vertices[index1].Position = Vector3.Transform(this.vertices[index1].Position, matrix);
      }
      for (int index2 = iOffset; index2 < iOffset + 36; ++index2)
        this.indices[index2] = (ushort) ((uint) this.indices[index2] + (uint) vOffset);
    }

    private void GetIndexValues(int projectileIndex, out int vIndex, out int iIndex)
    {
      vIndex = projectileIndex * 8;
      iIndex = projectileIndex * 36;
    }

    private void RemoveProjectile(int index)
    {
      if (index == this.projectiles.Count - 1)
      {
        this.projectiles.RemoveAt(index);
      }
      else
      {
        int num1 = index * 8;
        int num2 = index * 36;
        int num3 = (this.projectiles.Count - 1) * 8;
        int num4 = (this.projectiles.Count - 1) * 36;
        for (int index1 = 0; index1 < 8; ++index1)
        {
          this.vertices[num1 + index1] = this.vertices[num3 + index1];
          this.vertices[num1 + index1].Index = (float) (index % 50);
        }
        int num5 = (this.projectiles.Count - 1 - index) * 8;
        for (int index2 = 0; index2 < 36; ++index2)
          this.indices[num2 + index2] = (ushort) ((uint) this.indices[num4 + index2] - (uint) num5);
        this.projectiles[index] = this.projectiles[this.projectiles.Count - 1];
        this.projectiles.RemoveAt(this.projectiles.Count - 1);
      }
      this.updateRequired = true;
    }
  }
}
