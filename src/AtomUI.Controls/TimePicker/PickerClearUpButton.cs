﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class PickerClearUpButton : TemplatedControl
{
   public event EventHandler? ClearRequest;

   public static readonly StyledProperty<bool> IsInClearModeProperty =
      AvaloniaProperty.Register<PickerClearUpButton, bool>(nameof(IsInClearMode));

   public bool IsInClearMode
   {
      get => GetValue(IsInClearModeProperty);
      set => SetValue(IsInClearModeProperty, value);
   }

   private IconButton? _clearButton;

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _clearButton = e.NameScope.Get<IconButton>(PickerClearUpButtonTheme.ClearButtonPart);
      if (_clearButton is not null) {
         _clearButton.Click += (sender, args) =>
         {
            ClearRequest?.Invoke(this, EventArgs.Empty);
         };
      }
   }
}