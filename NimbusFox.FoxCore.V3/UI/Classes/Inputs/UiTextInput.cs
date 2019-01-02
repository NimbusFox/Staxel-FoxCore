using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Plukit.Base;
using Staxel;
using Staxel.Client;
using Staxel.Draw;
using Staxel.Input;
using Staxel.Logic;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace NimbusFox.FoxCore.V3.UI.Classes.Inputs {
    public class UiTextInput : UiSelectable {
        private UiTextBlock _textBlock;
        private bool _selected = false;
        private int _limit = int.MaxValue;
        private int _cursorIndex = 0;
        public UiTextInput() {
            _textBlock = new UiTextBlock();
            _textBlock.SetCaret();

            base.AddChild(_textBlock);
        }
        private Vector2 _size = new Vector2(150, 30);

        public event Action<string> OnChange;
        public event Func<char, bool> InputCheck;

        public override void AddChild(UiElement element) {

        }

        private DateTime _lastTick = DateTime.MinValue;

        public override void Update(Universe universe, Vector2 origin, AvatarController avatar, List<ScanCode> input, bool ctrl, bool shift, IReadOnlyList<InterfaceLogicalButton> inputPressed,
            MouseState mouseState) {
            base.Update(universe, origin, avatar, input, ctrl, shift, inputPressed, mouseState);
            var inside = Helpers.VectorContains(origin, origin + GetSize(), mouseState.Vector2());

            if (mouseState.LeftButton == ButtonState.Pressed) {
                _selected = inside;
            }

            if (!_selected) {
                _textBlock.RemoveCaret();
                return;
            }

            if ((DateTime.Now - _lastTick).Milliseconds > 500) {
                if (!_textBlock.HasCaret()) {
                    _textBlock.SetCaret();
                }
                _textBlock.ToggleCaret();
                _lastTick = DateTime.Now;
            }

            if (!input.Any()) {
                return;
            }

            if (ctrl) {
                if (input.Contains(ScanCode.V)) {
                    try {
                        ForceSetValue(GetValue() + Helpers.GetClipboardText(), true);
                    } catch {
                        // ignore
                    }
                    return;
                }
            }

            if (input.Any()) {
                var direction = input.GetDirectionKey();
                var key = input.GetPressedKey();

                if (key == null && direction == null) {
                    if (input.Contains(ScanCode.Space)) {
                        key = ' ';
                    } else if (input.Contains(ScanCode.Delete) || input.Contains(ScanCode.Backspace)) {

                    } else {
                        return;
                    }
                }

                if (direction != null) {
                    if (direction == ScanCode.Left) {
                        if (_cursorIndex > 0) {
                            _cursorIndex--;
                            _textBlock.SetCaretIndex((uint)_cursorIndex);
                        }

                        return;
                    }

                    if (direction == ScanCode.Right) {
                        if (_cursorIndex < GetValue().Length) {
                            _cursorIndex++;
                            _textBlock.SetCaretIndex((uint)_cursorIndex);
                        }

                        return;
                    }
                }

                var current = GetValue();

                if (input.Contains(ScanCode.Backspace)) {
                    if (current.Length == 0) {
                        return;
                    }
                    if (_cursorIndex != 0 && _cursorIndex != current.Length) {
                        SetValue(current.Substring(0, _cursorIndex - 1) + current.Substring(_cursorIndex), true);
                        _cursorIndex--;
                        if (_cursorIndex < 0) {
                            _cursorIndex = 0;
                        }
                        if (_cursorIndex >= current.Length) {
                            _cursorIndex = GetValue().Length;
                        }
                        _textBlock.SetCaretIndex((uint)_cursorIndex);
                    } else if (_cursorIndex >= current.Length) {
                        SetValue(current.Substring(0, current.Length - 1), true);
                        _cursorIndex--;
                        if (_cursorIndex < 0) {
                            _cursorIndex = 0;
                        }
                        if (_cursorIndex >= current.Length) {
                            _cursorIndex = GetValue().Length;
                        }
                        _textBlock.SetCaretIndex((uint)_cursorIndex);
                    }

                    return;
                }

                if (input.Contains(ScanCode.Delete)) {
                    if (_cursorIndex != current.Length && current.Length != 0) {
                        SetValue(current.Substring(0, _cursorIndex) + (_cursorIndex == current.Length ? "" : current.Substring(_cursorIndex + 1)), true);
                    }

                    return;
                }

                if (current.Length >= _limit) {
                    return;
                }

                if (key != null) {
                    var canInput = true;

                    if (InputCheck != null) {
                        canInput = InputCheck.Invoke(key.Value);
                    }

                    if (canInput) {
                        if (!shift) {
                            key = key.ToString().ToLower()[0];
                        }
                        SetValue(current.Substring(0, _cursorIndex <= 0 ? 0 : _cursorIndex) + key + (_cursorIndex >= current.Length ? "" : current.Substring(_cursorIndex)), true);
                        _cursorIndex++;
                        if (_cursorIndex >= GetValue().Length) {
                            _cursorIndex = GetValue().Length;
                        }
                        _textBlock.SetCaretIndex((uint)_cursorIndex);
                    }
                }
            }
        }

        public override void Draw(DeviceContext graphics, Entity entity, Universe universe, Vector2 origin,
            SpriteBatch spriteBatch, MouseState mouseState, Rectangle scissor) {
            var size = GetSize();
            if (_selected) {
                Background.Draw(graphics, origin, size, spriteBatch, _activeColor);

                foreach (var element in Elements) {
                    if (element is UiTextBlock textElement) {
                        textElement.SetColor(_activeTextColor);
                    }
                }
            } else {
                Background.Draw(graphics, origin, size, spriteBatch, _color);

                foreach (var element in Elements) {
                    if (element is UiTextBlock textElement) {
                        textElement.SetColor(_textColor);
                    }
                }
            }

            DrawChildren(graphics, entity, universe, origin + (Background?.TopLeftOffset ?? Vector2.Zero), spriteBatch, mouseState, scissor);
        }

        public string GetValue() {
            return _textBlock.ToString();
        }

        public void SetValue(string text, bool update = false) {
            var validText = "";

            if (InputCheck == null) {
                validText = text;
            } else {
                foreach (var ch in text) {
                    if (InputCheck.Invoke(ch)) {
                        validText += ch;
                    }
                }
            }

            if (validText.Length > _limit) {
                _textBlock.SetString(validText.Substring(0, _limit));
                return;
            }
            _textBlock.SetString(validText);
            _textBlock.SetCaretIndex((uint)_cursorIndex);
            if (update) {
                OnChange?.Invoke(GetValue());
            }
        }

        public void ForceSetValue(string text, bool update = false) {
            var validText = "";

            if (InputCheck == null) {
                validText = text;
            } else {
                foreach (var ch in text) {
                    if (InputCheck.Invoke(ch)) {
                        validText += ch;
                    }
                }
            }

            if (validText.Length > _limit) {
                _textBlock.SetString(validText.Substring(0, _limit));
                _cursorIndex = _textBlock.ToString().Length;
                return;
            }
            _textBlock.SetString(validText);
            _cursorIndex = _textBlock.ToString().Length;
            _textBlock.SetCaretIndex((uint)_cursorIndex);
            if (update) {
                OnChange?.Invoke(GetValue());
            }
        }

        public void SetLimit(int limit) {
            _limit = limit;
        }

        public void SetSize(uint width = 150, uint height = 30) {
            _size = new Vector2(width, height);
        }

        public override Vector2 GetSize() {
            var calcSize = _size + (Background?.TopLeftOffset ?? Vector2.Zero) + (Background?.BottomRightOffset ?? Vector2.Zero);

            if (Background != null) {
                var min = Background.GetMinSize();
                if (min.X > calcSize.X) {
                    calcSize.X = min.X;
                }

                if (min.Y > calcSize.Y) {
                    calcSize.Y = min.Y;
                }
            }

            calcSize.X = MinWidth < calcSize.X ? calcSize.X : MinWidth;
            calcSize.Y = MinHeight < calcSize.Y ? calcSize.Y : MinHeight;

            return calcSize;
        }
    }
}
