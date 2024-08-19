﻿using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class AddOnDecoratedInnerBoxTheme : BaseControlTheme
{
   public const string MainLayoutPart = "PART_MainLayout";
   public const string ContentPresenterPart = "PART_ContentPresenter";
   public const string LeftAddOnPart = "PART_LeftAddOn";
   public const string RightAddOnPart = "PART_RightAddOn";
   public const string LeftAddOnLayoutPart = "PART_LeftAddOnLayout";
   public const string RightAddOnLayoutPart = "PART_RightAddOnLayout";
   public const string ClearButtonPart = "PART_ClearButton";
   public const string InnerBoxDecoratorPart = "PART_InnerBoxDecorator";

   public AddOnDecoratedInnerBoxTheme() : base(typeof(AddOnDecoratedInnerBox)) {}
   protected AddOnDecoratedInnerBoxTheme(Type targetType) : base(targetType) { }

   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<AddOnDecoratedInnerBox>((decoratedBox, scope) =>
      {
         var frameLayout = new Panel();
         BuildFrameDecorator(frameLayout, decoratedBox, scope);
         NotifyBuildExtraChild(frameLayout, decoratedBox, scope);
         return frameLayout;
      });
   }

   protected virtual void NotifyBuildExtraChild(Panel layout, AddOnDecoratedInnerBox decoratedBox, INameScope scope)
   {
   }

   protected virtual void BuildFrameDecorator(Panel layout, AddOnDecoratedInnerBox decoratedBox, INameScope scope)
   {
      var innerBoxDecorator = new Border()
      {
         Name = InnerBoxDecoratorPart,
         Transitions = new Transitions()
         {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BorderBrushProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
         }
      };
      
      innerBoxDecorator.RegisterInNameScope(scope);
      CreateTemplateParentBinding(innerBoxDecorator, Border.PaddingProperty, AddOnDecoratedInnerBox.EffectiveInnerBoxPaddingProperty);
      CreateTemplateParentBinding(innerBoxDecorator, Border.BorderThicknessProperty, AddOnDecoratedInnerBox.BorderThicknessProperty);
      CreateTemplateParentBinding(innerBoxDecorator, Border.CornerRadiusProperty, AddOnDecoratedInnerBox.CornerRadiusProperty);
      
      var mainLayout = BuildBoxMainLayout(decoratedBox, scope);
      innerBoxDecorator.Child = mainLayout;
      
      layout.Children.Add(innerBoxDecorator);
   }

   protected virtual Panel BuildBoxMainLayout(AddOnDecoratedInnerBox decoratedBox, INameScope scope)
   {
      var mainLayout = new Grid()
      {
         Name = MainLayoutPart,
         ColumnDefinitions = new ColumnDefinitions()
         {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Auto)
         }
      };
      BuildGridChildren(decoratedBox, mainLayout, scope);
      return mainLayout;
   }

   protected virtual void BuildGridChildren(AddOnDecoratedInnerBox decoratedBox, Grid mainLayout, INameScope scope)
   {
      BuildLeftAddOn(mainLayout, scope);
      BuildContent(decoratedBox, mainLayout, scope);
      BuildRightAddOn(mainLayout, scope);
   }

   protected virtual void BuildLeftAddOn(Grid layout, INameScope scope)
   {
      // 理论上可以支持多个，暂时先支持一个
      var addLayout = new StackPanel()
      {
         Name = LeftAddOnLayoutPart,
         Orientation = Orientation.Horizontal,
      };
      TokenResourceBinder.CreateGlobalTokenBinding(addLayout, StackPanel.SpacingProperty, GlobalResourceKey.PaddingXXS);
      addLayout.RegisterInNameScope(scope);
      
      var leftAddOnContentPresenter = new ContentPresenter()
      {
         Name = LeftAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Left,
         Focusable = false,
      };

      CreateTemplateParentBinding(leftAddOnContentPresenter, ContentPresenter.ContentProperty,
                                  AddOnDecoratedInnerBox.LeftAddOnContentProperty);
      leftAddOnContentPresenter.RegisterInNameScope(scope);

      addLayout.Children.Add(leftAddOnContentPresenter);

      Grid.SetColumn(addLayout, 0);
      layout.Children.Add(addLayout);
   }

   protected virtual void BuildContent(AddOnDecoratedInnerBox decoratedBox, Grid layout, INameScope scope)
   {
      var innerBox = new ContentPresenter()
      {
         Name = ContentPresenterPart,
      };
      innerBox.RegisterInNameScope(scope);

      CreateTemplateParentBinding(innerBox, ContentPresenter.MarginProperty, AddOnDecoratedInnerBox.ContentPresenterMarginProperty);
      CreateTemplateParentBinding(innerBox, ContentPresenter.ContentProperty, AddOnDecoratedInnerBox.ContentProperty);
      CreateTemplateParentBinding(innerBox, ContentPresenter.ContentTemplateProperty,
                                  AddOnDecoratedInnerBox.ContentTemplateProperty);

      layout.Children.Add(innerBox);
      Grid.SetColumn(innerBox, 1);
   }

   private void BuildRightAddOn(Grid layout, INameScope scope)
   {
      var addLayout = new StackPanel()
      {
         Name = RightAddOnLayoutPart,
         Orientation = Orientation.Horizontal
      };
      TokenResourceBinder.CreateGlobalTokenBinding(addLayout, StackPanel.SpacingProperty, GlobalResourceKey.PaddingXXS);
      addLayout.RegisterInNameScope(scope);
      var rightAddOnContentPresenter = new ContentPresenter()
      {
         Name = RightAddOnPart,
         VerticalAlignment = VerticalAlignment.Stretch,
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Right,
         Focusable = false
      };
      CreateTemplateParentBinding(rightAddOnContentPresenter, ContentPresenter.ContentProperty,
                                  AddOnDecoratedInnerBox.RightAddOnContentProperty);

      rightAddOnContentPresenter.RegisterInNameScope(scope);
      addLayout.Children.Add(rightAddOnContentPresenter);
      
      BuildRightAddOnItems(addLayout, scope);

      layout.Children.Add(addLayout);
      Grid.SetColumn(addLayout, 2);
   }

   protected virtual void BuildRightAddOnItems(StackPanel layout, INameScope scope)
   {
      BuildClearButton(layout, scope);
   }

   protected virtual void BuildClearButton(StackPanel addOnLayout, INameScope scope)
   {
      var closeIcon = new PathIcon()
      {
         Kind = "CloseCircleFilled"
      };
      var clearButton = new IconButton()
      {
         Name = ClearButtonPart,
         Icon = closeIcon
      };

      TokenResourceBinder.CreateGlobalTokenBinding(clearButton, IconButton.IconHeightProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(clearButton, IconButton.IconWidthProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.NormalFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextQuaternary);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.ActiveFilledBrushProperty,
                                                   GlobalResourceKey.ColorTextTertiary);
      TokenResourceBinder.CreateGlobalTokenBinding(closeIcon, PathIcon.SelectedFilledBrushProperty,
                                                   GlobalResourceKey.ColorText);

      clearButton.RegisterInNameScope(scope);
      CreateTemplateParentBinding(clearButton, IconButton.IsVisibleProperty,
                                  AddOnDecoratedInnerBox.IsClearButtonVisibleProperty);
      addOnLayout.Children.Add(clearButton);
   }

   protected override void BuildStyles()
   {
      BuildCommonStyle();
      BuildDisabledStyle();
      BuildOutLineStyle();
      BuildFilledStyle();
      BuildAddOnStyle();
   }
   
   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      
      var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
      decoratorStyle.Add(Border.ZIndexProperty, AddOnDecoratedBoxTheme.NormalZIndex);
      commonStyle.Add(decoratorStyle);
      
      var largeStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Large));
      largeStyle.Add(AddOnDecoratedInnerBox.InnerBoxPaddingProperty, AddOnDecoratedBoxResourceKey.PaddingLG);
      
      {
         var innerBoxContentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         innerBoxContentStyle.Add(TextBlock.LineHeightProperty, GlobalResourceKey.FontHeightLG);
         innerBoxContentStyle.Add(Border.MinHeightProperty, GlobalResourceKey.FontHeightLG);
         largeStyle.Add(innerBoxContentStyle);
      }
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<PathIcon>());
         iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeLG);
         iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeLG);
         largeStyle.Add(iconStyle);
      }
      commonStyle.Add(largeStyle);

      var middleStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Middle));
      middleStyle.Add(AddOnDecoratedInnerBox.InnerBoxPaddingProperty, AddOnDecoratedBoxResourceKey.Padding);
      {
         var innerBoxContentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         innerBoxContentStyle.Add(TextBlock.LineHeightProperty, GlobalResourceKey.FontHeight);
         innerBoxContentStyle.Add(Border.MinHeightProperty, GlobalResourceKey.FontHeight);
         middleStyle.Add(innerBoxContentStyle);
      }
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<PathIcon>());
         iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSize);
         iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSize);
         middleStyle.Add(iconStyle);
      }
      commonStyle.Add(middleStyle);

      var smallStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.SizeTypeProperty, SizeType.Small));
      smallStyle.Add(AddOnDecoratedInnerBox.InnerBoxPaddingProperty, AddOnDecoratedBoxResourceKey.PaddingSM);
      {
         var innerBoxContentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
         innerBoxContentStyle.Add(TextBlock.LineHeightProperty, GlobalResourceKey.FontHeightSM);
         innerBoxContentStyle.Add(Border.MinHeightProperty, GlobalResourceKey.FontHeightSM);
         smallStyle.Add(innerBoxContentStyle);
      }
      {
         var iconStyle = new Style(selector => selector.Nesting().Template().Descendant().OfType<PathIcon>());
         iconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
         iconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
         smallStyle.Add(iconStyle);
      }
      commonStyle.Add(smallStyle);
      
      Add(commonStyle);
   }
   
   private void BuildOutLineStyle()
   {
      var outlineStyle =
         new Style(selector => selector.Nesting()
                                       .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, AddOnDecoratedVariant.Outline));

      {
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorBorder);
            outlineStyle.Add(innerBoxDecoratorStyle);
         }

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, AddOnDecoratedBoxResourceKey.HoverBorderColor);
            hoverStyle.Add(innerBoxDecoratorStyle);
         }

         outlineStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, AddOnDecoratedBoxResourceKey.ActiveBorderColor);
            focusStyle.Add(innerBoxDecoratorStyle);
         }
         outlineStyle.Add(focusStyle);
      }
      {
         var errorStyle = new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Error));
      
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorError);
            errorStyle.Add(innerBoxDecoratorStyle);
         }
      
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
            hoverStyle.Add(innerBoxDecoratorStyle);
         }
         errorStyle.Add(hoverStyle);
      
         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorError);
            focusStyle.Add(innerBoxDecoratorStyle);
         }
         errorStyle.Add(focusStyle);
         outlineStyle.Add(errorStyle);
      }
      
      {
         var warningStyle = new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Warning));
      
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarning);
            warningStyle.Add(innerBoxDecoratorStyle);
         }
      
         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarningBorderHover);
            hoverStyle.Add(innerBoxDecoratorStyle);
         }
         warningStyle.Add(hoverStyle);
      
         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarning);
            focusStyle.Add(innerBoxDecoratorStyle);
         }
         warningStyle.Add(focusStyle);
         outlineStyle.Add(warningStyle);
      }

      Add(outlineStyle);
   }
   
   private void BuildFilledStyle()
   {
      var filledStyle =
         new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, AddOnDecoratedVariant.Filled));

      {
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorFillTertiary);
            filledStyle.Add(innerBoxDecoratorStyle);
         }

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         {
            var innerBoxDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorFillSecondary);
            hoverStyle.Add(innerBoxDecoratorStyle);
         }
         filledStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         {
            var innerBoxDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, AddOnDecoratedBoxResourceKey.ActiveBorderColor);
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorTransparent);
            focusStyle.Add(innerBoxDecoratorStyle);
         }
         filledStyle.Add(focusStyle);
      }

      {
         var errorStyle = new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Error));

         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorErrorBg);
            errorStyle.Add(innerBoxDecoratorStyle);
         }

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorErrorBgHover);
            hoverStyle.Add(innerBoxDecoratorStyle);
         }
         errorStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorError);
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, AddOnDecoratedBoxResourceKey.ActiveBg);
            focusStyle.Add(innerBoxDecoratorStyle);
         }

         errorStyle.Add(focusStyle);
         filledStyle.Add(errorStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Warning));

         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));

            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorTransparent);
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorWarningBg);
            warningStyle.Add(innerBoxDecoratorStyle);
         }

         var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorWarningBgHover);
            hoverStyle.Add(innerBoxDecoratorStyle);
         }
         warningStyle.Add(hoverStyle);

         var focusStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.FocusWithIn));
         {
            var innerBoxDecoratorStyle =
               new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorWarning);
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, AddOnDecoratedBoxResourceKey.ActiveBg);
            focusStyle.Add(innerBoxDecoratorStyle);
         }
         warningStyle.Add(focusStyle);
         
         filledStyle.Add(warningStyle);
      }

      Add(filledStyle);
   }

   private void BuildAddOnStyle()
   {
      {
         var errorStyle = new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Error));
         {
            var iconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                                                               selector.Nesting().Template().Name(RightAddOnPart)).Nesting().Descendant().OfType<PathIcon>());
            iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorError);
            errorStyle.Add(iconStyle);
         }
         Add(errorStyle);
      }

      {
         var warningStyle = new Style(selector => selector.Nesting().PropertyEquals(AddOnDecoratedInnerBox.StatusProperty, AddOnDecoratedStatus.Warning));
         {
            var iconStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(LeftAddOnPart),
                                                               selector.Nesting().Template().Name(RightAddOnPart)).Nesting().Descendant().OfType<PathIcon>());
            iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalResourceKey.ColorWarning);
            warningStyle.Add(iconStyle);
         }
         Add(warningStyle);
      }
   }

   protected virtual void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
      decoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      disabledStyle.Add(decoratorStyle);
      Add(disabledStyle);
   }
}