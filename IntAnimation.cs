using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Animations
{
    public class IntAnimation : IAnimation
    {
        private readonly Control _control;
        private readonly FieldInfo _fieldInfo;
        private readonly int _time;
        private readonly int _startValue;
        private readonly int _endValue;

        private Thread _thread;

        public IntAnimation(Control control, string varName, int time, int startValue, int endValue)
        {
            _control = control ?? throw new ArgumentNullException(nameof(control));
            _fieldInfo = control.GetType().GetField(varName)
                ?? throw new ArgumentException($"Field '{varName}' does not exist in control.");

            _time = time;
            _startValue = startValue;
            _endValue = endValue;
        }

        public void Run()
        {
            _thread = new Thread(() =>
            {
                SetValue(_startValue);

                int change = _endValue - _startValue;
                int delta = change / _time;
                int tik = change / delta;

                for (int i = 1; i <= tik; i++)
                {
                    int currentValue = _startValue + i * delta;

                    SetValue(currentValue);

                    _control.Invoke((Action)(() => _control.Invalidate()));

                    Thread.Sleep(_time / tik);
                }

                SetValue(_endValue);
                _thread.Abort();
            });

            _thread.Start();
        }

        public void Stop()
        {
            _thread?.Abort();
        }

        private void SetValue(int value)
        {
            _fieldInfo.SetValue(_control, value);
        }
    }
}