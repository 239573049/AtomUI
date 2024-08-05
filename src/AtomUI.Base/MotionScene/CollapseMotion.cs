﻿using Avalonia.Animation.Easings;
using Avalonia.Controls;

namespace AtomUI.MotionScene;

public class CollapseMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? HeightConfig => GetMotionConfig(MotionHeightProperty);

   public void ConfigureHeight(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
      var config = new MotionConfig(MotionHeightProperty)
      {
         TransitionKind = TransitionKind.Double,
         EndValue = 0,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureOpacity(TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = 1d,
         EndValue = 0d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
   {
      base.NotifyPreBuildTransition(config, motionTarget);
      if (config.Property == MotionHeightProperty) {
         config.StartValue = motionTarget.DesiredSize.Height;
      }
   }
}

public class ExpandMotion : AbstractMotion
{
   public MotionConfig? OpacityConfig => GetMotionConfig(MotionOpacityProperty);
   public MotionConfig? HeightConfig => GetMotionConfig(MotionHeightProperty);

   public void ConfigureHeight(double originHeight, TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
      var config = new MotionConfig(MotionHeightProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = 0,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }

   public void ConfigureOpacity(double originOpacity, TimeSpan duration, Easing? easing = null)
   {
      easing ??= new CubicEaseInOut();
      var config = new MotionConfig(MotionOpacityProperty)
      {
         TransitionKind = TransitionKind.Double,
         StartValue = 0d,
         EndValue = 1d,
         MotionDuration = duration,
         MotionEasing = easing
      };
      AddMotionConfig(config);
   }
   
   protected override void NotifyPreBuildTransition(MotionConfig config, Control motionTarget)
   {
      base.NotifyPreBuildTransition(config, motionTarget);
      if (config.Property == MotionHeightProperty) {
         config.EndValue = motionTarget.DesiredSize.Height;
      }
   }
}