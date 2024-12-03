using System.Windows.Input;

using Avalonia.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AchxTool.ViewModels
{
    public partial class CanvasItemViewModel : ObservableObject
    {
        [ObservableProperty]
        private double _x;
        [ObservableProperty]
        private double _y;
        [ObservableProperty] 
        private double _z;
        [ObservableProperty]
        private double _width;
        [ObservableProperty]
        private double _height;

        // Command for moving and resizing
        //public ICommand MouseMoveCommand { get; }
        //public ICommand MouseDownCommand { get; }
        //public ICommand MouseUpCommand { get; }

        protected virtual void OnMouseDown(PointerPressedEventArgs? e) { }
        protected virtual void OnMouseMove(PointerEventArgs? e) { }
        protected virtual void OnMouseUp(PointerReleasedEventArgs? e) { }

        public CanvasItemViewModel()
        {
            //MouseMoveCommand = new RelayCommand<PointerEventArgs>(OnMouseMove);
            //MouseDownCommand = new RelayCommand<PointerEventArgs>(OnMouseDown);
            //MouseUpCommand = new RelayCommand<PointerEventArgs>(OnMouseUp);
        }
    }

    public class CanvasBoxViewModel : CanvasItemViewModel
    {

        // For Dragging
        private bool _isDragging;
        public bool IsDragging
        {
            get => _isDragging;
            private set => SetProperty(ref _isDragging, value);
        }

        // For Resizing
        private bool _isResizing;
        public bool IsResizing
        {
            get => _isResizing;
            private set => SetProperty(ref _isResizing, value);
        }

        protected void OnMouseDown(PointerEventArgs? e)
        {
            IsDragging = true;
            // For resizing, check if the pointer is near the edges of the control
            if (e.GetPosition(null).X >= X + Width - 10 && e.GetPosition(null).Y >= Y + Height - 10)
            {
                IsResizing = true;
            }
        }

        protected override void OnMouseMove(PointerEventArgs? e)
        {
            if (IsDragging)
            {
                var deltaX = e.GetPosition(null).X - X;
                var deltaY = e.GetPosition(null).Y - Y;
                X += deltaX;
                Y += deltaY;
            }
            else if (IsResizing)
            {
                Width = Math.Max(50, e.GetPosition(null).X - X);
                Height = Math.Max(50, e.GetPosition(null).Y - Y);
            }
        }

        protected void OnMouseUp(PointerEventArgs? e)
        {
            
        }
    }
}