﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Styling;

namespace AtomUI;

using AvaloniaControlTheme = Avalonia.Styling.ControlTheme;

public class ControlTheme : AvaloniaControlTheme
{
   public ControlTheme() {}
   public ControlTheme(Type targetType) : base(targetType) {}
   
   public void Build()
   {
      NotifyPreBuild();
      BuildStyles();
      var template = BuildControlTemplate();
      if (template is not null) {
         Add(new Setter(TemplatedControl.TemplateProperty, template));
      }
      NotifyBuildCompleted();
   }

   public virtual string? ThemeResourceKey()
   {
      return default;
   }

   protected virtual IControlTemplate? BuildControlTemplate() { return default; }
   protected virtual void BuildStyles() {}
   protected virtual void NotifyPreBuild() {}
   protected virtual void NotifyBuildCompleted() {}

   protected static IDisposable CreateTemplateParentBinding(Control control, AvaloniaProperty property, string templateParentPath)
   {
      return control.Bind(property, new Binding(templateParentPath)
      {
         RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent)
      });
   }
   
   protected static IDisposable CreateTemplateParentBinding(Control control, AvaloniaProperty property, AvaloniaProperty templateParentProperty)
   {
      return CreateTemplateParentBinding(control, property, templateParentProperty.Name);
   }
}