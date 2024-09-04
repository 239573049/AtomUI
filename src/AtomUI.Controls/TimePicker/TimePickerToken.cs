﻿using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TimePickerToken : AbstractControlDesignToken
{
   public const string ID = "TimePicker";
   
   public TimePickerToken()
      : base(ID)
   {
   }
   
   /// <summary>
   /// 时间选择项高度
   /// </summary>
   public double ItemHeight { get; set; }
   
   /// <summary>
   /// 时间选择项内间距
   /// </summary>
   public Thickness ItemPadding { get; set; }
   
   /// <summary>
   /// 按钮区域对上的外边距
   /// </summary>
   public Thickness ButtonsMargin { get; set; }
   
   /// <summary>
   /// 日期选择弹出框宽度
   /// </summary>
   public double PickerPopupWidth { get; set; }
   
   /// <summary>
   /// 日期选择弹出框高度
   /// </summary>
   public double PickerPopupHeight { get; set; }
   
   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      ItemHeight = _globalToken.SeedToken.ControlHeight - 4;
      ItemPadding = new Thickness(0, _globalToken.PaddingXXS);
      ButtonsMargin = new Thickness(0, _globalToken.MarginXS, 0, 0);
      PickerPopupWidth = 200;
      PickerPopupHeight = ItemHeight * 7;
   }
}