﻿using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public enum CollapseTriggerType
{
   Header,
   Icon,
}

public enum CollapseExpandIconPosition
{
   Start,
   End,
}

[TemplatePart(CollapseTheme.ItemsPresenterPart, typeof(ItemsPresenter))]
public class Collapse : SelectingItemsControl
{
   #region 公共属性定义

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<Collapse, SizeType>(nameof(SizeType), SizeType.Middle);
   
   public static readonly StyledProperty<bool> IsGhostStyleProperty =
      AvaloniaProperty.Register<Collapse, bool>(nameof(IsGhostStyle), false);
   
   public static readonly StyledProperty<bool> IsBorderlessProperty =
      AvaloniaProperty.Register<Collapse, bool>(nameof(IsBorderless), false);
   
   public static readonly StyledProperty<bool> IsAccordionProperty =
      AvaloniaProperty.Register<Collapse, bool>(nameof(IsAccordion), false);
   
   public static readonly StyledProperty<CollapseTriggerType> TriggerTypeProperty =
      AvaloniaProperty.Register<Collapse, CollapseTriggerType>(nameof(TriggerType), CollapseTriggerType.Header);
   
   public static readonly StyledProperty<CollapseExpandIconPosition> ExpandIconPositionProperty =
      AvaloniaProperty.Register<Collapse, CollapseExpandIconPosition>(nameof(ExpandIconPosition), CollapseExpandIconPosition.Start);
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public bool IsGhostStyle
   {
      get => GetValue(IsGhostStyleProperty);
      set => SetValue(IsGhostStyleProperty, value);
   }
   
   public bool IsBorderless
   {
      get => GetValue(IsBorderlessProperty);
      set => SetValue(IsBorderlessProperty, value);
   }
   
   public bool IsAccordion
   {
      get => GetValue(IsAccordionProperty);
      set => SetValue(IsAccordionProperty, value);
   }
   
   public CollapseTriggerType TriggerType
   {
      get => GetValue(TriggerTypeProperty);
      set => SetValue(TriggerTypeProperty, value);
   }
   
   public CollapseExpandIconPosition ExpandIconPosition
   {
      get => GetValue(ExpandIconPositionProperty);
      set => SetValue(ExpandIconPositionProperty, value);
   }
   
   #endregion
   #region 内部属性定义

   internal static readonly DirectProperty<Collapse, Thickness> EffectiveBorderThicknessProperty =
      AvaloniaProperty.RegisterDirect<Collapse, Thickness>(nameof(EffectiveBorderThickness),
         o => o.EffectiveBorderThickness,
         (o, v) => o.EffectiveBorderThickness = v);

   private Thickness _effectiveBorderThickness;

   internal Thickness EffectiveBorderThickness
   {
      get => _effectiveBorderThickness;
      set => SetAndRaise(EffectiveBorderThicknessProperty, ref _effectiveBorderThickness, value);
   }
   
   internal ItemsPresenter? ItemsPresenterPart { get; private set; }

   private static readonly FuncTemplate<Panel?> DefaultPanel =
      new(() => new StackPanel
      {
         Orientation = Orientation.Vertical
      });

   #endregion

   static Collapse()
   {
      SelectionModeProperty.OverrideDefaultValue<Collapse>(SelectionMode.Multiple | SelectionMode.Toggle);
      ItemsPanelProperty.OverrideDefaultValue<Collapse>(DefaultPanel);
   }

   public Collapse()
   {
      SelectionChanged += HandleSelectionChanged;
   }

   private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs args)
   {
      if (VisualRoot is not null) {
         for (var i = 0; i < ItemCount; ++i) {
            if (Items[i] is CollapseItem collapseItem) {
               SetupCollapseBorderThickness(collapseItem, i);
            }
         }
      }
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateGlobalResourceBinding(this, BorderThicknessProperty, GlobalResourceKey.BorderThickness, 
                                                      BindingPriority.Template, new RenderScaleAwareThicknessConfigure(this));
      SetupEffectiveBorderThickness();
      SetupSelectionMode();
   }

   protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
   {
      return new CollapseItem();
   }

   protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
   {
      return NeedsContainer<CollapseItem>(item, out recycleKey);
   }

   protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
   {
      base.PrepareContainerForItemOverride(element, item, index);
      if (item is CollapseItem collapseItem) {
         BindUtils.RelayBind(this, SizeTypeProperty, collapseItem, CollapseItem.SizeTypeProperty);
         BindUtils.RelayBind(this, EffectiveBorderThicknessProperty, collapseItem, CollapseItem.BorderThicknessProperty);
         BindUtils.RelayBind(this, IsGhostStyleProperty, collapseItem, CollapseItem.IsGhostStyleProperty);
         BindUtils.RelayBind(this, TriggerTypeProperty, collapseItem, CollapseItem.TriggerTypeProperty);
         BindUtils.RelayBind(this, ExpandIconPositionProperty, collapseItem, CollapseItem.ExpandIconPositionProperty);
         SetupCollapseBorderThickness(collapseItem, index);
      }
   }

   private void SetupCollapseBorderThickness(CollapseItem collapseItem, int index)
   {
      var headerBorderBottom = BorderThickness.Bottom;
      if (index == ItemCount - 1 && !collapseItem.IsSelected) {
         headerBorderBottom = 0d;
      }
      collapseItem.HeaderBorderThickness = new Thickness(0, 0, 0, headerBorderBottom);
      
      var contentBorderBottom = BorderThickness.Bottom;
      if (index == ItemCount - 1 && collapseItem.IsSelected) {
         contentBorderBottom = 0d;
      }
      collapseItem.ContentBorderThickness = new Thickness(0, 0, 0, contentBorderBottom);
   }

   protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
   {
      base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
   }

   protected override void ClearContainerForItemOverride(Control element)
   {
      base.ClearContainerForItemOverride(element);
   }

   protected override void OnGotFocus(GotFocusEventArgs e)
   {
      base.OnGotFocus(e);

      if (e.NavigationMethod == NavigationMethod.Directional) {
         e.Handled = UpdateSelectionFromEventSource(e.Source);
      }
   }

   protected override void OnPointerPressed(PointerPressedEventArgs e)
   {
      base.OnPointerPressed(e);

      if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && e.Pointer.Type == PointerType.Mouse) {
         e.Handled = UpdateSelectionFromEventSource(e.Source);
      }
   }

   protected override void OnPointerReleased(PointerReleasedEventArgs e)
   {
      if (e.InitialPressMouseButton == MouseButton.Left && e.Pointer.Type != PointerType.Mouse) {
         var container = GetContainerFromEventSource(e.Source);
         if (container != null
             && container.GetVisualsAt(e.GetPosition(container))
                         .Any(c => container == c || container.IsVisualAncestorOf(c))) {
            e.Handled = UpdateSelectionFromEventSource(e.Source);
         }
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (VisualRoot is not null) {
         if (change.Property == IsBorderlessProperty) {
            SetupEffectiveBorderThickness();
         }
      }

      if (change.Property == IsAccordionProperty) {
         SetupSelectionMode();
      }
   }

   private void SetupEffectiveBorderThickness()
   {
      if (IsBorderless) {
         EffectiveBorderThickness = default;
      } else {
         EffectiveBorderThickness = BorderThickness;
      }
   }

   private void SetupSelectionMode()
   {
      if (IsAccordion) {
         SelectionMode = SelectionMode.Single | SelectionMode.Toggle;
      } else {
         SelectionMode = SelectionMode.Multiple | SelectionMode.Toggle;
      }
   }
}