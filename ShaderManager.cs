// Decompiled with JetBrains decompiler
// Type: CornerSpace.ShaderManager
// Assembly: Corneroids, Version=1.0.0.0, Culture=neutral, PublicKeyToken=926c18641d5253e1
// MVID: 16145FCE-2BFC-487E-A607-E2168F0C1632
// Assembly location: C:\Users\Wissotsky\Desktop\camcompute\Corneroids\Corneroids.exe

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#nullable disable
namespace CornerSpace
{
  public class ShaderManager
  {
    private Dictionary<string, Effect> shaders;

    public bool Load(ContentManager content)
    {
      if (content == null)
        return false;
      try
      {
        this.shaders = new Dictionary<string, Effect>()
        {
          {
            "block_shader",
            content.Load<Effect>("Shaders/BlockShader")
          },
          {
            "dynamic_block_shader",
            content.Load<Effect>("Shaders/DynamicBlockShader")
          },
          {
            "pick_block_shader",
            content.Load<Effect>("Shaders/selectorEffect")
          },
          {
            "tool_block_shader",
            content.Load<Effect>("Shaders/ToolBlockShader")
          },
          {
            "bind_tool_shader",
            content.Load<Effect>("Shaders/ToolBlockShader")
          },
          {
            "projectile_shader",
            content.Load<Effect>("Shaders/ProjectileShader")
          },
          {
            "billboard_shader",
            content.Load<Effect>("Shaders/billboardShader")
          },
          {
            "thruster_shader",
            content.Load<Effect>("Shaders/ThrusterShader")
          },
          {
            "item_shader",
            content.Load<Effect>("Shaders/ItemShader")
          },
          {
            "rotate_block_shader",
            content.Load<Effect>("Shaders/RotateBlockShader")
          },
          {
            "entity_line_shader",
            content.Load<Effect>("Shaders/EntityLineShader")
          },
          {
            "radar_shader",
            content.Load<Effect>("Shaders/RadarShader")
          },
          {
            "block_shader_reduced",
            content.Load<Effect>("Shaders/BlockShaderReducedQuality")
          },
          {
            "space_model_shader",
            content.Load<Effect>("Shaders/SpaceModelShader")
          },
          {
            "deferred_combine_shader",
            content.Load<Effect>("Shaders/DeferredCombineShader")
          },
          {
            "ssao_shader",
            content.Load<Effect>("Shaders/ssaoShader")
          },
          {
            "ssao_combine_shader",
            content.Load<Effect>("Shaders/DeferredSSAOCombineShader")
          }
        };
        return true;
      }
      catch (Exception ex)
      {
        Engine.Console.WriteErrorLine("Failed to load shader files: " + ex.Message);
        return false;
      }
    }

    public void UpdateShaderVariable<T>(string variableName, T value)
    {
      if (this.shaders == null)
        throw new Exception("Shader manager has to be initialized first before calling any functions!");
      if (string.IsNullOrEmpty(variableName))
        return;
      try
      {
        foreach (Effect effect in this.shaders.Values)
        {
          if (effect != null)
          {
            EffectParameter parameter = effect.Parameters[variableName];
          }
        }
      }
      catch
      {
      }
    }
  }
}
