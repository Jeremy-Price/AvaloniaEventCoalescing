using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Themes.Fluent;

public static class ExampleAvaloniaApp
{
    public static void Main()
    {
        AppBuilder.Configure<AppExample>()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(Array.Empty<string>());
    }
}

class AppExample : Application
{
    public override void Initialize()
    {
        //Styles.Add(new Avalonia.Themes.Default.DefaultTheme());
        Styles.Add(new FluentTheme((Uri)null));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!).MainWindow = new WinExample();
    }
}

class WinExample : Window
{
    private readonly Canvas _canvas = new Canvas();

    private bool _isDrawing;
    private Point _pointLast = new Point(double.NaN, double.NaN);
    private float _x;
    
    public WinExample()
    {
        Content = _canvas;
        _canvas.Width = Width;
        _canvas.Height = Height;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _isDrawing = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        _isDrawing = false;
        _pointLast = new Point(double.NaN, double.NaN);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (!_isDrawing)
            return;

        // Pointer events are missed.
        DoExpensiveWork();

        int c = 0;
        foreach (PointerPoint pointer in e.GetIntermediatePoints(this))
        {
            c++;
            Point pointCur = pointer.Position;
            
            if (!double.IsNaN(_pointLast.X))
            {
                Line line = new Line()
                {
                    Stroke = Brushes.Red,
                    StartPoint = _pointLast,
                    EndPoint = pointCur
                };

                _canvas.Children.Add(line);
            }

            _pointLast = pointCur;
        }

        Title = c.ToString();
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        // Text events are not missed.
        DoExpensiveWork();
        
        Rectangle rect = new Rectangle()
        {
            Fill = Brushes.Green,
            Width = 10,
            Height = 10
        };

        Canvas.SetTop(rect, 0);
        Canvas.SetLeft(rect, _x);
        _canvas.Children.Add(rect);
        
        _x += 15;
    }

    // Simulation of expensive work occuring on the UI thread.
    private static void DoExpensiveWork() => Thread.Sleep(100);
}