﻿using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Animations
{
    internal class ColorAnimation : IAnimation
    {
        private readonly Control _control;
        private readonly FieldInfo _fieldInfo;
        private readonly int _time;
        private readonly Color _startValue;
        private readonly Color _endValue;

        private Thread _thread;

        public ColorAnimation(Control control, string varName, int time, Color startValue, Color endValue)
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

                int changeR = _endValue.R - _startValue.R;
                int changeG = _endValue.G - _startValue.G;
                int changeB = _endValue.B - _startValue.B;

                int deltaR = (changeR != 0) ? changeR / _time : 0;
                int deltaG = (changeG != 0) ? changeG / _time : 0;
                int deltaB = (changeB != 0) ? changeB / _time : 0;

                int tikR = (deltaR != 0) ? changeR / deltaR : 0;
                int tikG = (deltaG != 0) ? changeG / deltaG : 0;
                int tikB = (deltaB != 0) ? changeB / deltaB : 0;

                int tik = Math.Max(tikR, Math.Max(tikG, tikB));

                for (int i = 1; i <= tik; i++)
                {
                    int newR = Math.Max(0, _startValue.R + deltaR * i);
                    int newG = Math.Max(0, _startValue.G + deltaG * i);
                    int newB = Math.Max(0, _startValue.B + deltaB * i);

                    int currentR = newR < _endValue.R ? newR : _endValue.R;
                    int currentG = newG < _endValue.G ? newG : _endValue.G;
                    int currentB = newB < _endValue.B ? newB : _endValue.B;

                    SetValue(Color.FromArgb(currentR, currentG, currentB));

                    _control.Invoke((Action)(() => _control.Invalidate()));

                    Thread.Sleep(_time / tik);
                }

                SetValue(_endValue);
                _thread.Abort();
            });

            _thread.Start();
        }


        private void SetValue(Color value)
        {
            _fieldInfo.SetValue(_control, value);
        }
    }
}
