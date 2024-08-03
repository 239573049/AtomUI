﻿using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CardTabControlTheme : BaseTabControlTheme
{
   public const string AddTabButtonPart = "Part_AddTabButton";
   public const string CardTabStripScrollViewerPart = "Part_CardTabStripScrollViewer";
   
   public CardTabControlTheme() : base(typeof(CardTabControl)) { }
   
    protected override void NotifyBuildTabStripTemplate(BaseTabControl baseTabControl, INameScope scope, DockPanel container)
   {
      var cardTabStripContainer = new Grid()
      {
         Name = TabsContainerPart,
      };
      cardTabStripContainer.RegisterInNameScope(scope);
      
      CreateTemplateParentBinding(cardTabStripContainer, DockPanel.DockProperty, BaseTabControl.TabStripPlacementProperty);
      CreateTemplateParentBinding(cardTabStripContainer, Grid.MarginProperty, BaseTabControl.TabStripMarginProperty);

      TokenResourceBinder.CreateTokenBinding(cardTabStripContainer, StackPanel.SpacingProperty,
                                             TabControlResourceKey.CardGutter);
      
      var tabScrollViewer = new TabControlScrollViewer()
      {
         Name = CardTabStripScrollViewerPart
      };
      tabScrollViewer.RegisterInNameScope(scope);
      CreateTemplateParentBinding(tabScrollViewer, BaseTabScrollViewer.TabStripPlacementProperty, TabControl.TabStripPlacementProperty);
      var contentPanel = CreateTabStripContentPanel(scope);
      tabScrollViewer.Content = contentPanel;
      tabScrollViewer.TabControl = baseTabControl;

      var addTabIcon = new PathIcon()
      {
         Kind = "PlusOutlined"
      };

      TokenResourceBinder.CreateTokenBinding(addTabIcon, PathIcon.NormalFilledBrushProperty, TabControlResourceKey.ItemColor);
      TokenResourceBinder.CreateTokenBinding(addTabIcon, PathIcon.ActiveFilledBrushProperty, TabControlResourceKey.ItemHoverColor);
      TokenResourceBinder.CreateTokenBinding(addTabIcon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
      
      TokenResourceBinder.CreateGlobalResourceBinding(addTabIcon, PathIcon.WidthProperty, GlobalResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalResourceBinding(addTabIcon, PathIcon.HeightProperty, GlobalResourceKey.IconSize);

      var addTabButton = new IconButton
      {
         Name = AddTabButtonPart,
         BorderThickness = new Thickness(1),
         Icon = addTabIcon
      };
      
      CreateTemplateParentBinding(addTabButton, IconButton.BorderThicknessProperty, CardTabControl.CardBorderThicknessProperty);
      CreateTemplateParentBinding(addTabButton, IconButton.CornerRadiusProperty, CardTabControl.CardBorderRadiusProperty);
      CreateTemplateParentBinding(addTabButton, IconButton.IsVisibleProperty, CardTabControl.IsShowAddTabButtonProperty);
      
      TokenResourceBinder.CreateGlobalResourceBinding(addTabButton, IconButton.BorderBrushProperty, GlobalResourceKey.ColorBorderSecondary);
      
      addTabButton.RegisterInNameScope(scope);
      
      cardTabStripContainer.Children.Add(tabScrollViewer);
      cardTabStripContainer.Children.Add(addTabButton);
      
      container.Children.Add(cardTabStripContainer);
   }
   
   private ItemsPresenter CreateTabStripContentPanel(INameScope scope)
   {
      var itemsPresenter = new ItemsPresenter
      {
         Name = ItemsPresenterPart,
      };
      itemsPresenter.RegisterInNameScope(scope);
      
      CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, TabControl.ItemsPanelProperty);
      return itemsPresenter;
   }

   protected override void BuildStyles()
   {
      base.BuildStyles();
      var commonStyle = new Style(selector => selector.Nesting());
      
      // 设置 items presenter 面板样式
      // 分为上、右、下、左
      {
         // 上
         var topStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.TopPC));
         var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
         topStyle.Add(containerStyle);
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.CardGutter);

         var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
         addTabButtonStyle.Add(IconButton.MarginProperty, TabControlResourceKey.AddTabButtonMarginHorizontal);
         topStyle.Add(addTabButtonStyle);
         
         topStyle.Add(itemPresenterPanelStyle);
         commonStyle.Add(topStyle);
      }

      {
         // 右
         var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.RightPC));
         
         var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
         containerStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
         rightStyle.Add(containerStyle);
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.CardGutter);
         rightStyle.Add(itemPresenterPanelStyle);
         
         var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
         addTabButtonStyle.Add(IconButton.MarginProperty, TabControlResourceKey.AddTabButtonMarginVertical);
         rightStyle.Add(addTabButtonStyle);
         
         commonStyle.Add(rightStyle);
      }
      {
         // 下
         var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.BottomPC));
         
         var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
         containerStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
         bottomStyle.Add(containerStyle);
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.CardGutter);
         bottomStyle.Add(itemPresenterPanelStyle);
         
         var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
         addTabButtonStyle.Add(IconButton.MarginProperty, TabControlResourceKey.AddTabButtonMarginHorizontal);
         bottomStyle.Add(addTabButtonStyle);
         
         commonStyle.Add(bottomStyle);
      }
      {
         // 左
         var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.LeftPC));
         
         var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
         containerStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
         leftStyle.Add(containerStyle);
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.CardGutter);
         leftStyle.Add(itemPresenterPanelStyle);
         
         var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
         addTabButtonStyle.Add(IconButton.MarginProperty, TabControlResourceKey.AddTabButtonMarginVertical);
         leftStyle.Add(addTabButtonStyle);
         
         commonStyle.Add(leftStyle);
      }

      Add(commonStyle);
   }
}