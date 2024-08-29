﻿using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[TemplatePart(WindowNotificationManagerTheme.ItemsPart, typeof(Panel))]
[PseudoClasses(TopLeftPC, TopRightPC, BottomLeftPC, BottomRightPC, TopCenterPC, BottomCenterPC)]
public class WindowNotificationManager : TemplatedControl, INotificationManager
{
   public const string TopLeftPC = ":topleft";
   public const string TopRightPC = ":topright";
   public const string BottomLeftPC = ":bottomleft";
   public const string BottomRightPC = ":bottomright";
   public const string TopCenterPC = ":topcenter";
   public const string BottomCenterPC = ":bottomcenter";

   private IList? _items;
   private Queue<NotificationCard> _notificationCards;

   public static readonly StyledProperty<NotificationPosition> PositionProperty =
      AvaloniaProperty.Register<WindowNotificationManager, NotificationPosition>(
         nameof(Position), NotificationPosition.TopRight);
   
   public NotificationPosition Position
   {
      get => GetValue(PositionProperty);
      set => SetValue(PositionProperty, value);
   }
   
   public static readonly StyledProperty<int> MaxItemsProperty =
      AvaloniaProperty.Register<WindowNotificationManager, int>(nameof(MaxItems), 5);
   
   public int MaxItems
   {
      get => GetValue(MaxItemsProperty);
      set => SetValue(MaxItemsProperty, value);
   }
   
   public WindowNotificationManager(TopLevel? host) : this()
   {
      if (host is not null) {
         InstallFromTopLevel(host);
      }
   }
   
   public WindowNotificationManager()
   {
      UpdatePseudoClasses(Position);
      _notificationCards = new Queue<NotificationCard>();
   }

   static WindowNotificationManager()
   {
      HorizontalAlignmentProperty.OverrideDefaultValue<WindowNotificationManager>(HorizontalAlignment.Stretch);
      VerticalAlignmentProperty.OverrideDefaultValue<WindowNotificationManager>(VerticalAlignment.Stretch);
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);

      var itemsControl = e.NameScope.Find<Panel>("PART_Items");
      _items = itemsControl?.Children;
   }
   
   public void Show(INotification notification, string[]? classes = null)
   {
      var expiration = notification.Expiration;
      var onClick = notification.OnClick;
      var onClose = notification.OnClose;
      Dispatcher.UIThread.VerifyAccess();
      
      var notificationControl = new NotificationCard
      {
         Title = notification.Title,
         CardContent = notification.Content,
         NotificationType = notification.Type
      };

      // Add style classes if any
      if (classes != null) {
         foreach (var @class in classes) {
            notificationControl.Classes.Add(@class);
         }
      }

      notificationControl.NotificationClosed += (sender, args) =>
      {
         onClose?.Invoke();

         _items?.Remove(sender);
      };

      notificationControl.PointerPressed += (sender, args) =>
      {
         onClick?.Invoke();

         (sender as NotificationCard)?.Close();
      };

      Dispatcher.UIThread.Post(() =>
      {
         _items?.Add(notificationControl);

         if (_items?.OfType<NotificationCard>().Count(i => !i.IsClosing) > MaxItems) {
            _items.OfType<NotificationCard>().First(i => !i.IsClosing).Close();
         }
      });

      if (expiration == TimeSpan.Zero) {
         return;
      }
      
      _notificationCards.Enqueue(notificationControl);

      // await Task.Delay(expiration ?? TimeSpan.FromSeconds(1000));
      //
      // notificationControl.Close();
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);

      if (change.Property == PositionProperty) {
         UpdatePseudoClasses(change.GetNewValue<NotificationPosition>());
      }
   }
   
   private void InstallFromTopLevel(TopLevel topLevel)
   {
      topLevel.TemplateApplied += TopLevelOnTemplateApplied;
      var adorner = topLevel.FindDescendantOfType<VisualLayerManager>()?.AdornerLayer;
      if (adorner is not null) {
         adorner.Children.Add(this);
         AdornerLayer.SetAdornedElement(this, adorner);
      }
   }

   private void TopLevelOnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
   {
      if (Parent is AdornerLayer adornerLayer) {
         adornerLayer.Children.Remove(this);
         AdornerLayer.SetAdornedElement(this, null);
      }

      // Reinstall notification manager on template reapplied.
      var topLevel = (TopLevel)sender!;
      topLevel.TemplateApplied -= TopLevelOnTemplateApplied;
      InstallFromTopLevel(topLevel);
   }

   private void UpdatePseudoClasses(NotificationPosition position)
   {
      PseudoClasses.Set(TopLeftPC, position == NotificationPosition.TopLeft);
      PseudoClasses.Set(TopRightPC, position == NotificationPosition.TopRight);
      PseudoClasses.Set(BottomLeftPC, position == NotificationPosition.BottomLeft);
      PseudoClasses.Set(BottomRightPC, position == NotificationPosition.BottomRight);
      PseudoClasses.Set(TopCenterPC, position == NotificationPosition.TopCenter);
      PseudoClasses.Set(BottomCenterPC, position == NotificationPosition.BottomCenter);
   }
}