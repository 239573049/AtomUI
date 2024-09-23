﻿using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

[PseudoClasses(InlineModePC, HorizontalModePC, VerticalModePC)]
public class NavMenu : NavMenuBase
{
    public const string InlineModePC = ":inline-mode";
    public const string HorizontalModePC = ":horizontal-mode";
    public const string VerticalModePC = ":vertical-mode";
    public const string DarkStylePC = ":dark";
    public const string LightStylePC = ":light";
    
    #region 公共属性定义

    /// <summary>
    /// Defines the <see cref="Mode"/> property.
    /// </summary>
    public static readonly StyledProperty<NavMenuMode> ModeProperty =
        AvaloniaProperty.Register<NavMenu, NavMenuMode>(nameof(Mode), NavMenuMode.Horizontal);
    
    public static readonly StyledProperty<bool> IsDarkStyleProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsDarkStyle), false);
    
    public static readonly StyledProperty<double> ActiveBarWidthProperty =
        AvaloniaProperty.Register<NavMenu, double>(nameof(ActiveBarWidth), 1.0d,
            coerce: (o, v) => Math.Max(Math.Min(v, 1.0), 0.0));
    
    public static readonly StyledProperty<double> ActiveBarHeightProperty =
        AvaloniaProperty.Register<NavMenu, double>(nameof(ActiveBarHeight));
    
    public NavMenuMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    
    public bool IsDarkStyle
    {
        get => GetValue(IsDarkStyleProperty);
        set => SetValue(IsDarkStyleProperty, value);
    }
    
    public double ActiveBarWidth
    {
        get => GetValue(ActiveBarWidthProperty);
        set => SetValue(ActiveBarWidthProperty, value);
    }
    
    public double ActiveBarHeight
    {
        get => GetValue(ActiveBarHeightProperty);
        set => SetValue(ActiveBarHeightProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> HorizontalBorderThicknessProperty =
        AvaloniaProperty.Register<NavMenuItem, double>(nameof(HorizontalBorderThickness));
    
    public double HorizontalBorderThickness
    {
        get => GetValue(HorizontalBorderThicknessProperty);
        set => SetValue(HorizontalBorderThicknessProperty, value);
    }
    
    #endregion
    
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel { Orientation = Orientation.Vertical });

    static NavMenu()
    {
        ItemsPanelProperty.OverrideDefaultValue(typeof(NavMenu), DefaultPanel);
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
            typeof(NavMenu),
            KeyboardNavigationMode.Once);
        AutomationProperties.AccessibilityViewProperty.OverrideDefaultValue<NavMenu>(AccessibilityView.Control);
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<NavMenu>(AutomationControlType.Menu);
    }

    public NavMenu()
    {
        UpdatePseudoClasses();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavMenu"/> class.
    /// </summary>
    /// <param name="interactionHandler">The menu interaction handler.</param>
    public NavMenu(INavMenuInteractionHandler interactionHandler)
        : base(interactionHandler)
    {
        UpdatePseudoClasses();
    }

    public override void Close()
    {
        if (!IsOpen)
        {
            return;
        }

        foreach (var i in ((INavMenu)this).SubItems)
        {
            i.Close();
        }

        IsOpen        = false;
        SelectedIndex = -1;

        RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = ClosedEvent,
            Source      = this,
        });
    }

    public override void Open()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;

        RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = OpenedEvent,
            Source      = this,
        });
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (VisualRoot is not null)
        {
            if (change.Property == ModeProperty)
            {
                SetupItemContainerTheme(true);
            }
            UpdatePseudoClasses();
        }
    }

    protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
    {
        base.PrepareContainerForItemOverride(element, item, index);
        // Child menu items should not inherit the menu's ItemContainerTheme as that is specific
        // for top-level menu items.
        if ((element as NavMenuItem)?.ItemContainerTheme == ItemContainerTheme)
        {
            element.ClearValue(ItemContainerThemeProperty);
        }

        if (element is NavMenuItem navMenuItem && Mode == NavMenuMode.Horizontal)
        {
            BindUtils.RelayBind(this, ActiveBarHeightProperty, navMenuItem, NavMenuItem.ActiveBarHeightProperty);
            BindUtils.RelayBind(this, ActiveBarWidthProperty, navMenuItem, NavMenuItem.ActiveBarWidthProperty);
        }
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(HorizontalModePC, Mode == NavMenuMode.Horizontal);
        PseudoClasses.Set(VerticalModePC, Mode == NavMenuMode.Vertical);
        PseudoClasses.Set(InlineModePC, Mode == NavMenuMode.Inline);
        PseudoClasses.Set(DarkStylePC, IsDarkStyle);
        PseudoClasses.Set(LightStylePC, !IsDarkStyle);
    }
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        SetupItemContainerTheme();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        TokenResourceBinder.CreateGlobalTokenBinding(this, HorizontalBorderThicknessProperty, GlobalTokenResourceKey.LineWidth,
            BindingPriority.Template,
            new RenderScaleAwareDoubleConfigure(this));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        TokenResourceBinder.CreateTokenBinding(this, ActiveBarWidthProperty, NavMenuTokenResourceKey.ActiveBarWidth);
        TokenResourceBinder.CreateTokenBinding(this, ActiveBarHeightProperty, NavMenuTokenResourceKey.ActiveBarHeight);
    }

    private void SetupItemContainerTheme(bool force = false)
    {
        if (ItemContainerTheme is null || force)
        {
            var resourceKey = string.Empty; 
            if (Mode == NavMenuMode.Vertical)
            {
                resourceKey = VerticalNavMenuItemTheme.ID;
            }
            else if (Mode == NavMenuMode.Inline)
            {
                resourceKey = InlineNavMenuItemTheme.ID;
            }
            else
            {
                resourceKey = TopLevelHorizontalNavMenuItemTheme.ID;
            }
            TokenResourceBinder.CreateGlobalResourceBinding(this, ItemContainerThemeProperty, resourceKey);
        }
    }
}