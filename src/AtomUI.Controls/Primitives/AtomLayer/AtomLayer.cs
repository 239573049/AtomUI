using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Rect = Avalonia.Rect;

// ReSharper disable SuggestBaseTypeForParameter

namespace AtomUI.Controls.Primitives
{
    public class AtomLayer : Canvas
    {
        #region Static

        public static AtomLayer? GetLayer(Visual target)
        {
            return target.GetLayer();
        }

        public static Visual GetTarget(Control adorner)
        {
            return adorner.GetValue(TargetProperty);
        }
        private static void SetTarget(Control adorner, Visual value)
        {
            adorner.SetValue(TargetProperty, value);
        }
        public static readonly AttachedProperty<Visual> TargetProperty = AvaloniaProperty
            .RegisterAttached<AtomLayer, Control, Visual>("Target");

        public static Visual? GetBoundsAnchor(Visual element)
        {
            return element.GetValue(BoundsAnchorProperty);
        }
        public static void SetBoundsAnchor(Visual element, Visual? value)
        {
            element.SetValue(BoundsAnchorProperty, value);
        }
        public static readonly AttachedProperty<Visual?> BoundsAnchorProperty = AvaloniaProperty
            .RegisterAttached<AtomLayer, Visual, Visual?>("BoundsAnchor");
        
        #endregion


        public Visual? Host
        {
            get => GetValue(HostProperty);
            set => SetValue(HostProperty, value);
        }
        public static readonly StyledProperty<Visual?> HostProperty = AvaloniaProperty
            .Register<AtomLayer, Visual?>(nameof(Host));

        public Vector HostOffset
        {
            get => GetValue(HostOffsetProperty);
            set => SetValue(HostOffsetProperty, value);
        }
        public static readonly StyledProperty<Vector> HostOffsetProperty = AvaloniaProperty
            .Register<AtomLayer, Vector>(nameof(HostOffset));

        private readonly IList<WeakReference<Control>> _detachedAdorners = new List<WeakReference<Control>>();


        #region Ctor

        static AtomLayer()
        {
            HostOffsetProperty.Changed.AddClassHandler<AtomLayer>((layer, args) => { layer.Measure(); });
            BoundsAnchorProperty.Changed.AddClassHandler<Visual>((target, args) =>
            {
                var layer = target.GetLayer();
                layer?.MonitorTargetBounds(target);
                layer?.UpdateAdornersLocationOfTarget(target);
                layer?.Measure();
            });
        }

        internal AtomLayer()
        {
            Children.CollectionChanged += (sender, args) =>
            {
                if (_internalOperation)
                {
                    return;
                }

                throw new InvalidOperationException(
                    $"Please use {nameof(AddAdorner)}() method to add a child to {nameof(AtomLayer)} instead of adding it by Children's Add().");
            };
        }

        #endregion


        #region Public Methods

        public T? GetAdorner<T>(Visual target) where T : Control
        {
            return this.Children.OfType<T>().FirstOrDefault(a => GetTarget(a) == target);
        }

        public IEnumerable<Control> GetAdorners(Visual target)
        {
            return this.Children.Where(v => GetTarget(v) == target).ToList();
        }

        public void AddAdorner(Visual target, Control adorner)
        {
            if (Children.Contains(adorner))
            {
                return;
            }

            MonitorTargetBounds(target);

            target.AttachedToVisualTree   -= OnTargetOnAttachedToVisualTree;
            target.DetachedFromVisualTree -= OnTargetOnDetachedFromVisualTree;
            target.AttachedToVisualTree   += OnTargetOnAttachedToVisualTree;
            target.DetachedFromVisualTree += OnTargetOnDetachedFromVisualTree;

            SetTarget(adorner, target);
            AddChild(adorner);
            UpdateLocation(target, adorner);
            Arrange();
        }

        public void RemoveAdorner<T>(Visual target) where T : Control
        {
            var adorners = GetAdorners(target).OfType<T>().ToList();
            foreach (var adorner in adorners)
            {
                RemoveChild(adorner);
            }
        }

        public void RemoveAdorner(Visual target, Control adorner)
        {
            RemoveChild(adorner);
        }

        #endregion
        

        #region Measure & Arrange

        private void Measure()
        {
            Measure(new Size());
            Arrange();
        }

        private void Arrange()
        {
            Arrange(new Rect(new Point(HostOffset.X, -HostOffset.Y), new Size()));
        }

        #endregion
        

        #region Update Adorner Location

        private void UpdateAdornersLocationOfTarget(Visual target)
        {
            foreach (var a in GetAdorners(target))
            {
                UpdateLocation(target, a);
            }
        }

        private void UpdateLocation(Visual target, Control adorner)
        {
            if (this.Host is Control { IsLoaded: false })
            {
                this.Host.PropertyChanged -= ParentHostOnPropertyChanged;
                this.Host.PropertyChanged += ParentHostOnPropertyChanged;
            }
            else
            {
                UpdateLocationCore(target, adorner);
            }

            return;

            void ParentHostOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
            {
                this.Host.PropertyChanged -= ParentHostOnPropertyChanged;
                UpdateLocationCore(target, adorner);
            }
        }

        private void UpdateLocationCore(Visual target, Control adorner)
        {
            var provider = GetBoundsAnchor(target);
            provider ??= target;

            var matrix = provider.TransformToVisual(this)!;
            var x      = matrix.Value.M31;
            var y      = matrix.Value.M32;

            SetLeft(adorner, x);
            SetTop(adorner, y);
            adorner.Width  = provider.Bounds.Width;
            adorner.Height = provider.Bounds.Height;
        }

        #endregion
        

        #region Attach & Detach
        
        private void OnTargetOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args)
        {
            if (sender is not Visual target)
            {
                return;
            }

            var adorners = new List<Control>();
            for (var i = 0; i < _detachedAdorners.Count; i++)
            {
                var w = _detachedAdorners[i];
                if (w.TryGetTarget(out var t) && Children.Contains(t) == false && GetTarget(t) == target)
                {
                    adorners.Add(t);
                    _detachedAdorners.RemoveAt(i--);
                }
            }

            adorners = adorners.Where(a => Children.Contains(a) == false && GetTarget(a) == target).ToList();
            foreach (var adorner in adorners)
            {
                AddChild(adorner);
            }
        }

        private void OnTargetOnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args)
        {
            if (sender is not Visual target)
            {
                return;
            }

            var adorners = target.GetAdorners();
            foreach (var adorner in adorners)
            {
                RemoveChild(adorner);
                _detachedAdorners.Add(new WeakReference<Control>(adorner));
            }
        }

        #endregion
        

        #region Monitor Target Bounds

        private IDisposable? _monitoringTargetBoundsDisposable;

        private void MonitorTargetBounds(Visual target)
        {
            _monitoringTargetBoundsDisposable?.Dispose();
            _monitoringTargetBoundsDisposable = null;

            var provider = GetBoundsAnchor(target);
            provider ??= target;

            provider.PropertyChanged -= TargetBoundsOnPropertyChanged;
            provider.PropertyChanged += TargetBoundsOnPropertyChanged;

            _monitoringTargetBoundsDisposable = Disposable.Create(() =>
            {
                provider.PropertyChanged -= TargetBoundsOnPropertyChanged;
            });

            return;

            void TargetBoundsOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
            {
                if (e.Property != BoundsProperty)
                {
                    return;
                }

                // Child element's bounds will be updated before it's ancestors do.
                Dispatcher.UIThread.Post(() => UpdateAdornersLocationOfTarget(target), DispatcherPriority.Send);
            }
        }

        #endregion


        #region Children

        private bool _internalOperation = false;

        private void AddChild(Control adorner)
        {
            try
            {
                _internalOperation = true;
                Children.Add(adorner);
            }
            finally
            {
                _internalOperation = false;
            }
        }

        private void RemoveChild(Control adorner)
        {
            try
            {
                _internalOperation = true;
                Children.Remove(adorner);
            }
            finally
            {
                _internalOperation = false;
            }
        }

        #endregion
    }
}