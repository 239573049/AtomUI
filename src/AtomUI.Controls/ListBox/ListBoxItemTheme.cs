﻿using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia.Animation;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ListBoxItemTheme : BaseControlTheme
{
   public const string ContentPresenterPart = "PART_ContentPresenter";
   
   public ListBoxItemTheme() : this(typeof(ListBoxItem)) {}
   protected ListBoxItemTheme(Type targetType) : base(targetType) {}
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<ListBoxItem>((listBoxItem, scope) =>
      {
         var contentPresenter = new ContentPresenter()
         {
            Name = ContentPresenterPart,
         };

         contentPresenter.Transitions = new Transitions()
         {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.BackgroundProperty)
         };

         CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty, ListBoxItem.CornerRadiusProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, ListBoxItem.ContentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, ListBoxItem.ContentTemplateProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.MinHeightProperty, ListBoxItem.MinHeightProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty, ListBoxItem.PaddingProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty, ListBoxItem.HorizontalContentAlignmentProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty, ListBoxItem.VerticalContentAlignmentProperty);
         return contentPresenter;
      });
   }
   
   protected override void BuildStyles()
   {
      BuildCommonStyle();
      BuildSizeTypeStyle();
      BuildDisabledStyle();
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(ListBoxItem.MarginProperty, ListBoxTokenResourceKey.ItemMargin);
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, ListBoxTokenResourceKey.ItemColor);
         contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, ListBoxTokenResourceKey.ItemBgColor);
         commonStyle.Add(contentPresenterStyle);
      }

      var disabledItemHoverStyle = new Style(selector => selector.Nesting().PropertyEquals(ListBoxItem.DisabledItemHoverEffectProperty, false));
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart).Class(StdPseudoClass.PointerOver));
         contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, ListBoxTokenResourceKey.ItemHoverColor);
         contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, ListBoxTokenResourceKey.ItemHoverBgColor);
         disabledItemHoverStyle.Add(contentPresenterStyle);
      }
      commonStyle.Add(disabledItemHoverStyle);

      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      {
         var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, ListBoxTokenResourceKey.ItemSelectedColor);
         contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, ListBoxTokenResourceKey.ItemSelectedBgColor);
         selectedStyle.Add(contentPresenterStyle);
      }
      commonStyle.Add(selectedStyle);
      Add(commonStyle);
   }

   private void BuildSizeTypeStyle()
   {
      var largeStyle = new Style(selector => selector.Nesting().PropertyEquals(ListBoxItem.SizeTypeProperty, SizeType.Large));
      largeStyle.Add(ListBoxItem.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      largeStyle.Add(ListBoxItem.MinHeightProperty, GlobalTokenResourceKey.ControlHeightLG);
      largeStyle.Add(ListBoxItem.PaddingProperty, ListBoxTokenResourceKey.ItemPaddingLG);

      Add(largeStyle);
      
      var middleStyle = new Style(selector => selector.Nesting().PropertyEquals(ListBoxItem.SizeTypeProperty, SizeType.Middle));
      middleStyle.Add(ListBoxItem.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
      middleStyle.Add(ListBoxItem.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
      middleStyle.Add(ListBoxItem.PaddingProperty, ListBoxTokenResourceKey.ItemPadding);

      Add(middleStyle);
      
      var smallStyle = new Style(selector => selector.Nesting().PropertyEquals(ListBoxItem.SizeTypeProperty, SizeType.Small));
      smallStyle.Add(ListBoxItem.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusXS);
      smallStyle.Add(ListBoxItem.MinHeightProperty, GlobalTokenResourceKey.ControlHeightSM);
      smallStyle.Add(ListBoxItem.PaddingProperty, ListBoxTokenResourceKey.ItemPaddingSM);
      Add(smallStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
      contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
      disabledStyle.Add(contentPresenterStyle);
      Add(disabledStyle);
   }
}