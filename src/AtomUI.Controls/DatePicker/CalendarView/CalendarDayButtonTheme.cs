﻿using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.CalendarView;

[ControlThemeProvider]
internal class CalendarDayButtonTheme : BaseControlTheme
{
    private const string ContentPart = "PART_Content";

    public CalendarDayButtonTheme()
        : base(typeof(CalendarDayButton))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<CalendarDayButton>((calendarDayButton, scope) =>
        {
            var contentPresenter = new ContentPresenter
            {
                Name = ContentPart
            };

            CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty,
                TemplatedControl.PaddingProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty,
                TemplatedControl.ForegroundProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.BackgroundProperty,
                TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderBrushProperty,
                TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.BorderThicknessProperty,
                TemplatedControl.BorderThicknessProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty,
                TemplatedControl.FontSizeProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, Layoutable.HorizontalAlignmentProperty,
                Layoutable.HorizontalAlignmentProperty);
            CreateTemplateParentBinding(contentPresenter, Layoutable.VerticalAlignmentProperty,
                Layoutable.VerticalAlignmentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty,
                ContentControl.HorizontalContentAlignmentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty,
                ContentControl.VerticalContentAlignmentProperty);

            return contentPresenter;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        commonStyle.Add(Avalonia.Controls.Button.ClickModeProperty, ClickMode.Release);
        commonStyle.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Hand));
        commonStyle.Add(TemplatedControl.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
        commonStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorTextLabel);
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        commonStyle.Add(TemplatedControl.BorderBrushProperty, GlobalTokenResourceKey.ColorTransparent);
        commonStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        commonStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);
        commonStyle.Add(ContentControl.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
        commonStyle.Add(ContentControl.VerticalContentAlignmentProperty, VerticalAlignment.Center);
        commonStyle.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSize);
        commonStyle.Add(Layoutable.WidthProperty, DatePickerTokenResourceKey.CellWidth);
        commonStyle.Add(Layoutable.HeightProperty, DatePickerTokenResourceKey.CellHeight);
        commonStyle.Add(Layoutable.MarginProperty, DatePickerTokenResourceKey.CellMargin);

        var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
        contentStyle.Add(ContentPresenter.LineHeightProperty, DatePickerTokenResourceKey.CellLineHeight);
        commonStyle.Add(contentStyle);

        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.BackgroundProperty, DatePickerTokenResourceKey.CellHoverBg);
        commonStyle.Add(hoverStyle);

        var inactiveStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.InActive));
        inactiveStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
        commonStyle.Add(inactiveStyle);

        var todayStyle = new Style(selector => selector.Nesting().Class(BaseCalendarDayButton.TodayPC));
        todayStyle.Add(TemplatedControl.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
        commonStyle.Add(todayStyle);

        var selectedStyle = new Style(selector =>
            selector.Nesting().Class(StdPseudoClass.Selected)
                    .Not(selector1 => selector1.Nesting().Class(StdPseudoClass.InActive)));
        selectedStyle.Add(TemplatedControl.BackgroundProperty, GlobalTokenResourceKey.ColorPrimary);
        selectedStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorWhite);
        selectedStyle.Add(TemplatedControl.BorderThicknessProperty, new Thickness(0));
        commonStyle.Add(selectedStyle);

        Add(commonStyle);
    }
}