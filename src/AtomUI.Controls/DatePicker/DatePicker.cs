﻿using AtomUI.Controls.Internal;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace AtomUI.Controls;

public class DatePicker : InfoPickerInput
{
    #region 公共属性定义

    public static readonly StyledProperty<DateTime?> SelectedDateTimeProperty =
        AvaloniaProperty.Register<DatePicker, DateTime?>(nameof(SelectedDateTime),
            enableDataValidation: true);
    
    public static readonly StyledProperty<DateTime?> DefaultDateTimeProperty =
        AvaloniaProperty.Register<DatePicker, DateTime?>(nameof(DefaultDateTime),
            enableDataValidation: true);

    public static readonly StyledProperty<string?> FormatProperty = 
        AvaloniaProperty.Register<DatePicker, string?>(nameof(Format));
    
    public static readonly StyledProperty<bool> IsShowTimeProperty =
        AvaloniaProperty.Register<DatePicker, bool>(nameof(IsShowTime));
    
    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        AvaloniaProperty.Register<DatePicker, bool>(nameof(IsNeedConfirm));
    
    public static readonly StyledProperty<bool> IsShowNowProperty =
        AvaloniaProperty.Register<DatePicker, bool>(nameof(IsShowNow), true);

    public DateTime? SelectedDateTime
    {
        get => GetValue(SelectedDateTimeProperty);
        set => SetValue(SelectedDateTimeProperty, value);
    }
    
    public DateTime? DefaultDateTime
    {
        get => GetValue(DefaultDateTimeProperty);
        set => SetValue(DefaultDateTimeProperty, value);
    }
    
    public string? Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
    
    public bool IsShowTime
    {
        get => GetValue(IsShowTimeProperty);
        set => SetValue(IsShowTimeProperty, value);
    }
    
    public bool IsNeedConfirm
    {
        get => GetValue(IsNeedConfirmProperty);
        set => SetValue(IsNeedConfirmProperty, value);
    }
    
    public bool IsShowNow
    {
        get => GetValue(IsShowNowProperty);
        set => SetValue(IsShowNowProperty, value);
    }
    
    #endregion
    
    private DatePickerPresenter? _pickerPresenter;
    
    /// <summary>
    /// 清除时间选择器的值，不考虑默认值
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        SelectedDateTime = null;
    }

    /// <summary>
    /// 重置时间选择器的值，当有默认值设置的时候，会将当前的值设置成默认值
    /// </summary>
    public void Reset()
    {
        SelectedDateTime = DefaultDateTime;
    }

    private string EffectiveFormat()
    {
        if (Format is not null)
        {
            return Format;
        }

        var format = "yyyy-MM-dd";
        if (IsShowTime)
        {
            format = $"{format} HH:mm:ss";
        }

        return format;
    }
    
    protected override Flyout CreatePickerFlyout()
    {
        return new DatePickerFlyout();
    }
    
    protected override void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
        if (flyoutPresenter is DatePickerFlyoutPresenter datePickerFlyoutPresenter)
        {
            datePickerFlyoutPresenter.AttachedToVisualTree += (sender, args) =>
            {
                _pickerPresenter = datePickerFlyoutPresenter.DatePickerPresenter;
                ConfigurePickerPresenter(_pickerPresenter);
            };
        }
    }

    private void ConfigurePickerPresenter(DatePickerPresenter? presenter)
    {
        if (presenter is null)
        {
            return;
        }

        presenter.ChoosingStatueChanged += (sender, args) =>
        {
            _isChoosing = args.IsChoosing;
            UpdatePseudoClasses();
            if (!args.IsChoosing)
            {
                ClearHoverSelectedInfo();
            }
        };
        presenter.HoverDateTimeChanged += (sender, args) =>
        {
            if (args.Value.HasValue)
            {
                Text = args.Value.Value.ToString(EffectiveFormat());
            }
            else
            {
                Text = null;
            }
        };

        presenter.Confirmed += (sender, args) =>
        {
            SelectedDateTime = presenter.SelectedDateTime;
            Dispatcher.UIThread.Post(() => ClosePickerFlyout());
        };
        
        BindUtils.RelayBind(this, SelectedDateTimeProperty, presenter, DatePickerPresenter.SelectedDateTimeProperty);
        BindUtils.RelayBind(this, IsNeedConfirmProperty, presenter, DatePickerPresenter.IsNeedConfirmProperty);
        BindUtils.RelayBind(this, IsShowNowProperty, presenter, DatePickerPresenter.IsShowNowProperty);
        BindUtils.RelayBind(this, IsShowTimeProperty, presenter, DatePickerPresenter.IsShowTimeProperty);
    }

    private void ClearHoverSelectedInfo()
    {
        DateTime? targetValue = default;
        targetValue = SelectedDateTime ?? DefaultDateTime;
        Text        = targetValue?.ToString(EffectiveFormat());
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (DefaultDateTime is not null && SelectedDateTime is null)
        {
            SelectedDateTime = DefaultDateTime;
        }
        Text = SelectedDateTime?.ToString(EffectiveFormat());
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedDateTimeProperty)
        {
            Text = SelectedDateTime?.ToString(EffectiveFormat());
        }
    }

    protected override bool ShowClearButtonPredicate()
    {
        return SelectedDateTime is not null;
    }
}