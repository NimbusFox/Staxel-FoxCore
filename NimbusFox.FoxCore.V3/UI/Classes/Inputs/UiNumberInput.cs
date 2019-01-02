using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;

namespace NimbusFox.FoxCore.V3.UI.Classes.Inputs {
    public class UiNumberInput : UiSelectable {
        private UiRow _displayRow;
        private UiTextInput _textInput;
        private string _format = "N0";

        private float _value;
        private float _min = float.MinValue;
        private float _max = float.MaxValue;
        private float _itterations = 1.0f;

        public event Action<float> OnChange;

        public UiNumberInput() {
            _textInput = new UiTextInput();
            _textInput.SetBackground(Constants.Backgrounds.TextInput);
            _textInput.SetTextColor(Color.Black);
            _textInput.SetActiveTextColor(Color.Black);
            _textInput.InputCheck += ch => {
                switch (ch) {
                    default:
                        return false;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '.':
                        return true;
                }
            };
            _textInput.OnChange += text => {
                if (float.TryParse(text, out var value)) {
                    SetValue(value, true);
                }
            };
            _displayRow = new UiRow();

            var downButton = new UiButton();
            downButton.SetBackground(Constants.Backgrounds.Button);
            downButton.OnHold += () => {
                SetValue(_value - _itterations, true);
            };
            downButton.SetTextColor(Color.Black);
            downButton.SetActiveTextColor(Color.Black);
            var downText = new UiTextBlock();
            downText.SetString("-");
            downButton.AddChild(downText);

            var spacer = UiSpacer.GetSpacer(5, 5);

            _displayRow.AddChild(downButton);
            _displayRow.AddChild(spacer);

            base.AddChild(_displayRow);
            var upButton = new UiButton();
            upButton.SetBackground(Constants.Backgrounds.Button);
            upButton.OnHold += () => {
                SetValue(_value + _itterations, true);
            };

            var inputColumn = new UiColumn();

            inputColumn.AddChild(spacer);
            inputColumn.AddChild(_textInput);
            inputColumn.AddChild(spacer);

            _displayRow.AddChild(inputColumn);
            _displayRow.AddChild(spacer);
            _displayRow.AddChild(upButton);
            upButton.SetTextColor(Color.Black);
            upButton.SetActiveTextColor(Color.Black);
            var upText = new UiTextBlock();
            upText.SetString("+");
            upButton.AddChild(upText);

            SetSize(60, 30);
        }

        public override void AddChild(UiElement element) {
            
        }

        public void SetMin(float min) {
            _min = min;
            if (_value <= min) {
                SetValue(min);
            }
        }

        public void SetMax(float max) {
            _max = max;
            if (_value >= max) {
                SetValue(max);
            }
        }

        public void ForceSetValue(float value, bool update = false) {
            _value = value;
            if (value > _max) {
                _value = _max;
            }

            if (value < _min) {
                _value = _min;
            }

            _textInput.ForceSetValue(_value.ToString(_format));
            if (update) {
                OnChange?.Invoke(_value);
            }
        }

        public void SetValue(float value, bool update = false) {
            _value = value;
            if (value > _max) {
                _value = _max;
            }

            if (value < _min) {
                _value = _min;
            }

            _textInput.SetValue(_value.ToString(_format));
            if (update) {
                OnChange?.Invoke(_value);
            }
        }

        public void SetFormat(string format) {
            _format = format;
        }

        public void SetSize(uint width, uint height) {
            _textInput.SetSize(width, height);
        }

        public void SetItteration(float value) {
            _itterations = value;
        }

        public void SetBackgroundColors(Color idle, Color active) {
            _textInput.SetBackgroundColor(idle);
            _textInput.SetActiveBackgroundColor(active);
        }

        public void SetTextColors(Color idle, Color active) {
            _textInput.SetTextColor(idle);
            _textInput.SetActiveTextColor(active);
        }

        public override Vector2 GetSize() {
            return _displayRow.GetSize();
        }
    }
}
