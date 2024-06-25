using IconsTestApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IconsTestApp.Controls
{
    internal class Image : Microsoft.Maui.Controls.Image
    {
        public static readonly BindableProperty CustomImageSourceProperty = BindableProperty.Create(nameof(CustomImageSource), typeof(string), typeof(Image), propertyChanged: (b, o, n) => ((Image)b).OnCustomImageSourceChanged(o, n));

        public string CustomImageSource
        {
            get => (string)GetValue(CustomImageSourceProperty);
            set => SetValue(CustomImageSourceProperty, value);
        }

        private ImageScheduler _scheduler;
        public Image()
        {
            _scheduler = new ImageScheduler();
        }

        private int _counter = 0;
        private void OnCustomImageSourceChanged(object o, object n)
        {
            _scheduler.EnqueueTask(SetImageAsync, n as string ?? string.Empty);

            // You can also omit the queue, the issue still occurs.
            //_ = SetImage(n as string ?? string.Empty, _counter++);
        }

        private async Task SetImageAsync(string imageSource, int instanceNumber)
        {
            await Dispatcher.DispatchAsync(() => Source = null);

            if (string.IsNullOrEmpty(imageSource))
            {
                return;
            }

            // Loading images from stream has an issue when used in a collection view.
            // Make sure the test_icon* files have the right build action (Embedded resource).
            // Await makes it easier to reproduce the issue.
            await Task.Delay(1000);
            var assembly = Assembly.GetExecutingAssembly();
            Stream? stream = assembly.GetManifestResourceStream($"IconsTestApp.Resources.Images.{imageSource}");
            if (stream == null)
            {
                return;
            }
            ImageSource streamImageSource = ImageSource.FromStream(() => stream);

            //Loading image from file doesn't have the issue.
            // Make sure the test_icon* files have the right build action (Maui image).
            //ImageSource fileImageSource = ImageSource.FromFile(imageSource);

            await Dispatcher.DispatchAsync(() => Source = streamImageSource);
        }


        private class ImageScheduler
        {
            private bool _isExecuting = false;
            private string _targetText = string.Empty;
            private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
            private Queue<ScheduledTask> _tasks = new Queue<ScheduledTask>();

            public void SetTargetText(string text)
            {
                _targetText = text;
            }

            public void EnqueueTask(Func<string, int, Task> func, string newValue)
            {
                ScheduledTask scheduledTask = new ScheduledTask(func, newValue, OnTaskCompleted, _targetText);
                _tasks.Enqueue(scheduledTask);
                _ = StartNextTask();
            }

            private void OnTaskCompleted()
            {
                _isExecuting = false;
                _ = StartNextTask();
            }

            private async Task StartNextTask()
            {
                if (_isExecuting)
                {
                    return;
                }
                await _semaphoreSlim.WaitAsync();
                try
                {
                    if (_isExecuting || _tasks.Count == 0)
                    {
                        return;
                    }
                    _isExecuting = true;
                    ScheduledTask nextTask = _tasks.Dequeue();
                    nextTask.StartTask();
                }
                finally
                {
                    _semaphoreSlim.Release();
                }
            }
        }

        private class ScheduledTask
        {
            private static int _counter = 0;
            private int _instanceNumber;
            //private string _targetText;
            private Func<string, int, Task> _func;
            private Action _onTaskCompletedCallback;
            private string _newValue;
            public ScheduledTask(Func<string, int, Task> func, string newValue, Action onTaskCompletedCallback, string targetText)
            {
                _instanceNumber = _counter++;
                _onTaskCompletedCallback = onTaskCompletedCallback;
                _func = func;
                _newValue = newValue;
                //Console.WriteLine($"[{DateTime.Now:hh:mm:ss:fff}] #{_instanceNumber} ({targetText}) Queing task.");
            }

            public void StartTask()
            {
                _func.Invoke(_newValue, _instanceNumber).ContinueWith(task => _onTaskCompletedCallback.Invoke());
            }
        }
    }
}
