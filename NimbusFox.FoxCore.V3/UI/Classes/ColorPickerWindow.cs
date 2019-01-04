using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NimbusFox.FoxCore.V3;
using NimbusFox.FoxCore.V3.Classes;
using NimbusFox.FoxCore.V3.Managers;
using NimbusFox.FoxCore.V3.UI.Classes;
using NimbusFox.FoxCore.V3.UI.Classes.Inputs;
using Plukit.Base;
using Staxel;
using Staxel.Core;
using Constants = NimbusFox.FoxCore.V3.Constants;

namespace NimbusFox.ColorBlocks.Classes {
    public class ColorPickerWindow : IDisposable {

        private static List<Color> _colorHistory = new List<Color>();

        private Color _color = Color.White;

        private static Color _historyColor;

        private static DirectoryManager _favDir =
            CoreHook.FxCore.ConfigDirectory.FetchDirectory(FavFolder);

        internal const string FavFolder = "favorites";

        private UiWindow _current;

        public event Action OnClose;
        public event Action<Color> OnColorSet;
        public event Action OnShow;
        public event Action OnHide;

        public ColorPickerWindow(Color? _default = null) {
            if (_default != null) {
                _color = _default.Value;
            }

            _current = new UiWindow();

            _current.ListenForEscape(true);

            _current.Container.SetBackground(Constants.Backgrounds.Dark);

            var picker = new UiColorPicker();

            var currentColor = new UiTexture2D(context => Helpers.GetTexture(context));
            var hoverColor = new UiTexture2D(context => Helpers.GetTexture(context));
            var selectedColor = new UiTexture2D(context => Helpers.GetTexture(context));
            selectedColor.SetColor(_color);
            currentColor.SetColor(_color);

            var row = new UiRow();

            row.AddChild(picker);

            var column = new UiColumn();

            var spacer = new UiSpacer();
            spacer.SetHeight(20);
            spacer.SetWidth(20);

            row.AddChild(spacer);

            var mouseText = new UiTextBlock();

            mouseText.SetColor(Color.White);
            mouseText.SetString("Mouse");
            column.AddChild(mouseText);
            column.AddChild(hoverColor);
            column.AddChild(spacer);

            var selectedText = new UiTextBlock();

            selectedText.SetColor(Color.White);
            selectedText.SetString("Selected");
            column.AddChild(selectedText);
            column.AddChild(selectedColor);
            column.AddChild(spacer);

            var currentText = new UiTextBlock();
            currentText.SetColor(Color.White);
            currentText.SetString("Current");
            column.AddChild(currentText);
            column.AddChild(currentColor);

            row.AddChild(column);

            _current.AddChild(row);

            _current.AddChild(spacer);

            var colorRow = new UiRow();

            //var colorR = new UiTextBlock();
            //colorR.SetString("R: " + _color.R);
            //colorR.SetColor(Color.Red);

            UiTextInput colorHex = null;

            var colorR = new UiNumberInput();
            colorR.SetItteration(1.0f);
            colorR.SetMin(0);
            colorR.SetMax(255);
            colorR.SetValue(_color.R);
            colorR.SetBackgroundColors(Color.Red, Color.Pink);

            colorR.OnChange += value => {
                if (byte.TryParse(value.ToString(), out var val)) {
                    _color.R = val;
                    colorHex?.ForceSetValue($"{_color.R:X2}{_color.G:X2}{_color.B:X2}");
                    selectedColor.SetColor(_color);
                }
            };

            //var colorG = new UiTextBlock();
            //colorG.SetString("G: " + _color.G);
            //colorG.SetColor(Color.LimeGreen);
            var colorG = new UiNumberInput();
            colorG.SetItteration(1.0f);
            colorG.SetMin(0);
            colorG.SetMax(255);
            colorG.SetValue(_color.G);
            colorG.SetBackgroundColors(Color.Green, Color.LimeGreen);

            colorG.OnChange += value => {
                if (byte.TryParse(value.ToString(), out var val)) {
                    _color.G = val;
                    colorHex?.ForceSetValue($"{_color.R:X2}{_color.G:X2}{_color.B:X2}");
                    selectedColor.SetColor(_color);
                }
            };

            //var colorB = new UiTextBlock();
            //colorB.SetString("B: " + _color.B);
            //colorB.SetColor(Color.LightBlue);

            var colorB = new UiNumberInput();
            colorB.SetItteration(1.0f);
            colorB.SetMin(0);
            colorB.SetMax(255);
            colorB.SetValue(_color.B);
            colorB.SetBackgroundColors(Color.Blue, Color.LightBlue);
            colorB.SetTextColors(Color.White, Color.Black);

            colorB.OnChange += value => {
                if (byte.TryParse(value.ToString(), out var val)) {
                    _color.B = val;
                    colorHex?.ForceSetValue($"{_color.R:X2}{_color.G:X2}{_color.B:X2}");
                    selectedColor.SetColor(_color);
                }
            };

            //var colorHex = new UiTextBlock();
            //colorHex.SetString($"Hex: {_color.R:X2}{_color.G:X2}{_color.B:X2}");

            var colorHexRow = new UiRow();

            var hex = new UiTextBlock();
            hex.SetString("HEX: ");

            colorHex = new UiTextInput();
            colorHex.ForceSetValue($"{_color.R:X2}{_color.G:X2}{_color.B:X2}");
            colorHex.SetBackgroundColor(new Color(237, 207, 154));
            colorHex.SetTextColor(Color.Black);
            colorHex.SetActiveTextColor(Color.Black);
            colorHex.SetBackground(Constants.Backgrounds.TextInput);
            colorHex.SetLimit(6);
            colorHex.InputCheck += key => {
                switch (key.ToString().ToUpper()) {
                    default:
                        return false;
                    case "A":
                    case "B":
                    case "C":
                    case "D":
                    case "E":
                    case "F":
                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                        return true;
                }
            };

            colorHex.OnChange += text => {
                if (text.IsNullOrEmpty()) {
                    _color = Color.Black;
                } else {
                    _color = ColorMath.ParseString(text);
                }
                selectedColor.SetColor(_color);
                colorR.ForceSetValue(_color.R);
                colorG.ForceSetValue(_color.G);
                colorB.ForceSetValue(_color.B);
            };

            picker.ColorClick += color => {
                selectedColor.SetColor(color);
                _color = color;
                colorR.ForceSetValue(_color.R);
                colorG.ForceSetValue(_color.G);
                colorB.ForceSetValue(_color.B);
                colorHex.ForceSetValue($"{_color.R:X2}{_color.G:X2}{_color.B:X2}");
            };

            colorRow.AddChild(colorR);
            colorRow.AddChild(spacer);
            colorRow.AddChild(colorG);
            colorRow.AddChild(spacer);
            colorRow.AddChild(colorB);

            _current.AddChild(colorRow);
            _current.AddChild(spacer);
            colorHexRow.AddChild(hex);
            colorHexRow.AddChild(colorHex);
            _current.AddChild(colorHexRow);

            _current.AddChild(spacer);

            var confirmButton = new UiButton();
            confirmButton.SetBackground(Constants.Backgrounds.Button);

            var confirmText = new UiTextBlock();

            confirmText.SetString("Set Color");

            confirmButton.AddChild(confirmText);

            confirmButton.OnClick += () => {
                OnColorSet?.Invoke(_color);

                if (_colorHistory.Contains(_color)) {
                    _colorHistory.Remove(_color);
                }

                _colorHistory.Add(_color);

                _current.Dispose();
            };

            var cancelButton = new UiButton();
            cancelButton.SetBackground(Constants.Backgrounds.Button);

            var cancelText = new UiTextBlock();

            cancelText.SetString("Cancel");

            cancelButton.AddChild(cancelText);

            cancelButton.OnClick += () => {
                _current.Dispose();
            };

            var buttonRow = new UiRow();

            buttonRow.AddChild(confirmButton);
            buttonRow.AddChild(spacer);
            buttonRow.AddChild(cancelButton);
            buttonRow.AddChild(spacer);

            var favButton = new UiButton();
            favButton.SetBackground(Constants.Backgrounds.Button);
            var favPic = FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.favorites");
            favButton.AddChild(favPic);
            buttonRow.AddChild(favButton);
            buttonRow.AddChild(spacer);

            favButton.OnClick += () => {
                SpawnFavoriteFolderMenu(_current, spacer, color => {
                    colorHex.ForceSetValue($"{color.R:X2}{color.G:X2}{color.B:X2}", true);
                });
            };

            var historyButton = new UiButton();
            historyButton.SetBackground(Constants.Backgrounds.Button);
            var historyPic = FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.history");
            historyButton.AddChild(historyPic);
            buttonRow.AddChild(historyButton);

            historyButton.OnClick += () => {
                SpawnHistoryWindow(_current, spacer, color => {
                    colorHex.ForceSetValue($"{color.R:X2}{color.G:X2}{color.B:X2}", true);
                });
            };

            _current.AddChild(buttonRow);

            picker.ColorHover += color => { hoverColor.SetColor(color); };

            _current.OnClose += () => {
                OnClose?.Invoke();
            };
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            _current.Dispose();
        }

        public void Show() {
            _current.Show();
            OnShow?.Invoke();
        }

        public void Hide() {
            _current.Hide();
            OnHide?.Invoke();
        }

        private static void SpawnHistoryWindow(UiWindow window, UiSpacer spacer, Action<Color> setColor) {
            var historyWindow = new UiWindow();
            historyWindow.Container.SetBackground(Constants.Backgrounds.Dark);
            historyWindow.ListenForEscape(true);

            var reverse = new List<Color>(_colorHistory);
            reverse.Reverse();

            SpawnColorWindowContent(window, historyWindow, spacer, setColor, SpawnHistoryWindow, color => {
                _colorHistory.Remove(color);
            }, reverse, "History", "No history", true);

            historyWindow.OnClose += () => {
                window.ListenForEscape(true);
                window.Show();
                window.StartUpdateCalls();
            };

            window.Hide();
            window.ListenForEscape(false);
            window.AddChildWindow(historyWindow);
            window.StopUpdateCalls();
            historyWindow.Show();
        }

        private static void SpawnFavoriteFolderMenu(UiWindow window, UiSpacer spacer, Action<Color> setColor) {
            var favFolderMenu = new UiWindow();
            favFolderMenu.Container.SetBackground(Constants.Backgrounds.Dark);
            favFolderMenu.ListenForEscape(true);

            SpawnFavoriteFolderMenuContent(window, favFolderMenu, spacer, color => {
                setColor(color);
                favFolderMenu.Dispose();
            });

            favFolderMenu.OnClose += () => {
                window.ListenForEscape(true);
                window.Show();
                window.StartUpdateCalls();
            };

            window.Hide();
            window.ListenForEscape(false);
            window.AddChildWindow(favFolderMenu);
            window.StopUpdateCalls();
            favFolderMenu.Show();
        }

        private static void SpawnAddToFavFromHistoryMenu(UiWindow window, UiSpacer spacer, Action<Color> setColor) {
            var favFolderMenu = new UiWindow();
            favFolderMenu.Container.SetBackground(Constants.Backgrounds.Dark);
            favFolderMenu.ListenForEscape(true);
            favFolderMenu.StopUpdateCalls();
            favFolderMenu.StartUpdateCalls();

            SpawnAddToFavFromHistoryMenuContent(window, favFolderMenu, spacer, setColor);

            favFolderMenu.OnClose += () => {
                window.ListenForEscape(true);
                window.Show();
                window.StartUpdateCalls();
            };

            window.Hide();
            window.ListenForEscape(false);
            window.AddChildWindow(favFolderMenu);
            window.StopUpdateCalls();
            favFolderMenu.Show();
        }

        private static void SpawnColorWindowContent(UiWindow window, UiWindow historyWindow, UiSpacer spacer,
            Action<Color> setColor, Action<UiWindow, UiSpacer, Action<Color>> renew, Action<Color> remove, List<Color> colors, string title, string noColorText, bool showAddToFav = false) {
            var historyTitle = new UiTextBlock();
            historyTitle.SetString(title);
            historyTitle.SetColor(Color.Orange);
            historyTitle.SetFont(Constants.Fonts.MyFirstCrush36);
            historyWindow.AddChild(historyTitle);
            historyWindow.AddChild(spacer);

            if (!colors.Any()) {
                var historyText = new UiTextBlock();
                historyText.SetString(noColorText);
                historyWindow.AddChild(historyText);
            } else {
                var column1 = new UiRow();
                var column1Squares = new UiColumn();
                var column1Text = new UiColumn();
                var column1Buttons = new UiColumn();
                column1.AddChild(column1Squares);
                column1.AddChild(UiSpacer.GetSpacer());
                column1.AddChild(column1Text);
                column1.AddChild(UiSpacer.GetSpacer());
                column1.AddChild(column1Buttons);

                var column2 = new UiRow();
                var column2Squares = new UiColumn();
                var column2Text = new UiColumn();
                var column2Buttons = new UiColumn();
                column2.AddChild(column2Squares);
                column2.AddChild(UiSpacer.GetSpacer());
                column2.AddChild(column2Text);
                column2.AddChild(UiSpacer.GetSpacer());
                column2.AddChild(column2Buttons);

                var column = 1;
                var count = 0;
                foreach (var color in colors) {
                    SpawnHistoryColorItem(window, historyWindow, color, spacer, setColor, renew, remove, showAddToFav, out var colorSquare, out var colorText, out var buttons);
                    if (count >= 2) {
                        if (column == 1) {
                            column1Squares.AddChild(UiSpacer.GetSpacer(0, 20));
                            column1Text.AddChild(UiSpacer.GetSpacer(0, 12));
                            column1Buttons.AddChild(UiSpacer.GetSpacer(0, 15));
                        } else {
                            column2Squares.AddChild(UiSpacer.GetSpacer(0, 20));
                            column2Text.AddChild(UiSpacer.GetSpacer(0, 12));
                            column2Buttons.AddChild(UiSpacer.GetSpacer(0, 15));
                        }
                    }

                    if (column == 1) {
                        column1Squares.AddChild(colorSquare);
                        column1Text.AddChild(colorText);
                        column1Buttons.AddChild(buttons);
                        column = 2;
                    } else {
                        column2Squares.AddChild(colorSquare);
                        column2Text.AddChild(colorText);
                        column2Buttons.AddChild(buttons);
                        column = 1;
                    }

                    count++;
                }
                var columnRow = new UiRow();
                columnRow.AddChild(column1);
                if (colors.Count >= 2) {
                    columnRow.AddChild(UiSpacer.GetSpacer(10, 0));
                    columnRow.AddChild(column2);
                }

                var scrollable = new UiScrollableContainer();
                scrollable.AddChild(columnRow);
                scrollable.SetDimensions(600, 400);
                historyWindow.AddChild(scrollable);
            }

            historyWindow.AddChild(spacer);

            var closeButton = new UiButton();
            closeButton.SetBackground(Constants.Backgrounds.Button);

            var closeText = new UiTextBlock();
            closeText.SetString("Close");

            closeButton.AddChild(closeText);

            historyWindow.AddChild(closeButton);

            closeButton.OnClick += historyWindow.Dispose;
        }

        private static void SpawnAddToFavFromHistoryMenuContent(UiWindow window, UiWindow favFolderMenuWindow, UiSpacer spacer,
            Action<Color> setColor) {
            var scrollable = new UiScrollableContainer();
            scrollable.SetDimensions(400, 500);
            var favFolderMenuTitle = new UiTextBlock();
            favFolderMenuTitle.SetFont(Constants.Fonts.MyFirstCrush36);
            favFolderMenuTitle.SetString("Favorites");
            favFolderMenuTitle.SetColor(Color.Orange);
            favFolderMenuWindow.AddChild(favFolderMenuTitle);
            favFolderMenuWindow.AddChild(spacer);
            favFolderMenuWindow.AddChild(scrollable);

            var newFolderButton = new UiButton();
            var newFolderText = new UiTextBlock();
            newFolderText.SetString("New Folder");
            newFolderButton.OnClick += () => {
                favFolderMenuWindow.StopUpdateCalls();
                favFolderMenuWindow.Dispose();
                SpawnInputTextWindow(window, spacer, "Name:", "New Folder", value => {
                    if (_favDir.Files.Any(x => x.ToLower() == value.ToLower())) {
                        return false;
                    }

                    using (var stream = _favDir
                        .ObtainFileStream(value, FileMode.Create, FileAccess.ReadWrite)) {
                        stream.Seek(0L, SeekOrigin.Begin);
                    }

                    return true;
                }, "Folder exists", "A folder with this name already exists", () => {
                    SpawnAddToFavFromHistoryMenu(window, spacer, setColor);
                });
            };
            newFolderButton.AddChild(newFolderText);
            scrollable.AddChild(newFolderButton);
            scrollable.AddChild(spacer);

            var files = _favDir.Files;

            if (files.Any()) {
                foreach (var file in files) {
                    scrollable.AddChild(SpawnFolderItem(window, favFolderMenuWindow, spacer, setColor, file,
                        (folder) => {
                            var values = new List<string>();
                            if (_favDir.FileExists(folder)) {
                                values.AddRange(
                                    File.ReadAllLines(
                                        Path.Combine(_favDir.GetPath(Path.DirectorySeparatorChar), folder)));
                            }

                            if (!values.Contains($"{_historyColor.R:X2}{_historyColor.G:X2}{_historyColor.B:X2}")) {
                                values.Add($"{_historyColor.R:X2}{_historyColor.G:X2}{_historyColor.B:X2}");
                            }

                            File.WriteAllLines(Path.Combine(_favDir.GetPath(Path.DirectorySeparatorChar), folder), values);
                            return true;
                        }, SpawnAddToFavFromHistoryMenu));
                    scrollable.AddChild(spacer);
                }
            }

            var closeButton = new UiButton();
            closeButton.SetBackground(Constants.Backgrounds.Button);

            var closeText = new UiTextBlock();
            closeText.SetString("Close");

            closeButton.AddChild(closeText);

            scrollable.AddChild(closeButton);

            closeButton.OnClick += () => {
                favFolderMenuWindow.Dispose();
                window.ListenForEscape(true);
                window.Show();
                window.StartUpdateCalls();
            };
        }

        private static void SpawnFolderColorWindow(UiWindow window, UiSpacer spacer, Action<Color> setColor, string folder) {
            var historyWindow = new UiWindow();
            historyWindow.Container.SetBackground(Constants.Backgrounds.Dark);
            historyWindow.ListenForEscape(true);

            var colValues = new List<string>();
            if (_favDir.FileExists(folder)) {
                colValues.AddRange(
                    File.ReadAllLines(
                        Path.Combine(_favDir.GetPath(Path.DirectorySeparatorChar), folder)));
            }

            var colors = new List<Color>();

            foreach (var color in colValues) {
                var parsedColor = color;
                if (color.Contains(" ")) {
                    parsedColor = color.Replace(" ", "");
                }
                if (parsedColor.Length <= 6) {
                    var valid = true;
                    foreach (var ch in parsedColor) {
                        switch (ch) {
                            default:
                                valid = false;
                                break;
                            case 'a':
                            case 'A':
                            case 'b':
                            case 'B':
                            case 'c':
                            case 'C':
                            case 'd':
                            case 'D':
                            case 'e':
                            case 'E':
                            case 'f':
                            case 'F':
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
                                break;
                        }
                    }

                    if (valid) {
                        colors.Add(ColorMath.ParseString(parsedColor));
                    }
                }
            }

            SpawnColorWindowContent(window, historyWindow, spacer, setColor,
                (item1, item2, item3) => { SpawnFolderColorWindow(item1, item2, item3, folder); },
                color => {
                    var values = new List<string>();
                    if (_favDir.FileExists(folder)) {
                        values.AddRange(
                            File.ReadAllLines(
                                Path.Combine(_favDir.GetPath(Path.DirectorySeparatorChar), folder)));
                    }

                    var toRemove = "";

                    foreach (var val in values) {
                        try {
                            var co = ColorMath.ParseString(val);

                            if (co == color) {
                                toRemove = val;
                                break;
                            }
                        } catch {
                            // ignore
                        }
                    }

                    if (values.Contains(toRemove)) {
                        values.Remove(toRemove);
                    }

                    File.WriteAllLines(Path.Combine(_favDir.GetPath(Path.DirectorySeparatorChar), folder), values);
                }, colors, folder, "No colors");

            historyWindow.OnClose += () => {
                window.ListenForEscape(true);
                window.Show();
                window.StartUpdateCalls();
            };

            window.Hide();
            window.ListenForEscape(false);
            window.AddChildWindow(historyWindow);
            window.StopUpdateCalls();
            historyWindow.Show();
        }

        private static void SpawnFavoriteFolderMenuContent(UiWindow window, UiWindow favFolderMenuWindow, UiSpacer spacer,
            Action<Color> setColor) {
            var scrollable = new UiScrollableContainer();
            scrollable.SetDimensions(400, 500);
            var favFolderMenuTitle = new UiTextBlock();
            favFolderMenuTitle.SetFont(Constants.Fonts.MyFirstCrush36);
            favFolderMenuTitle.SetString("Favorites");
            favFolderMenuTitle.SetColor(Color.Orange);
            favFolderMenuWindow.AddChild(favFolderMenuTitle);
            favFolderMenuWindow.AddChild(spacer);
            favFolderMenuWindow.AddChild(scrollable);

            var newFolderButton = new UiButton();
            var newFolderText = new UiTextBlock();
            newFolderText.SetString("New Folder");
            newFolderButton.OnClick += () => {
                favFolderMenuWindow.Dispose();
                SpawnInputTextWindow(window, spacer, "Name: ", "New Folder", value => {
                    if (_favDir.Files
                        .Any(x => x.ToLower() == value.ToLower())) {
                        return false;
                    }

                    using (var stream = _favDir
                        .ObtainFileStream(value, FileMode.Create, FileAccess.ReadWrite)) {
                        stream.Seek(0L, SeekOrigin.Begin);
                    }

                    return true;
                }, "Folder exists", "A folder with this name already exists", () => {
                    SpawnFavoriteFolderMenu(window, spacer, setColor);
                });
            };
            newFolderButton.AddChild(newFolderText);
            scrollable.AddChild(newFolderButton);
            scrollable.AddChild(spacer);

            var files = _favDir.Files;

            if (files.Any()) {
                foreach (var file in files) {
                    scrollable.AddChild(SpawnFolderItem(window, favFolderMenuWindow, spacer, setColor, file,
                        (folder) => {
                            SpawnFolderColorWindow(favFolderMenuWindow, spacer, setColor, folder);
                            return false;
                        }, SpawnFavoriteFolderMenu));
                    scrollable.AddChild(spacer);
                }
            }

            var closeButton = new UiButton();
            closeButton.SetBackground(Constants.Backgrounds.Button);

            var closeText = new UiTextBlock();
            closeText.SetString("Close");

            closeButton.AddChild(closeText);

            favFolderMenuWindow.AddChild(closeButton);

            closeButton.OnClick += favFolderMenuWindow.Dispose;
        }

        private static void SpawnColorItem(Color color, UiSpacer spacer, out UiColumn colorSquare, out UiTextBlock colorText) {
            colorSquare = new UiColumn();
            var colorSquare1 = new UiTexture2D(graphics => Helpers.GetTexture(graphics));
            colorSquare1.SetColor(color);
            colorSquare.AddChild(UiSpacer.GetSpacer(0, 5));
            colorSquare.AddChild(colorSquare1);

            colorText = new UiTextBlock();
            colorText.SetString($"{color.R:X2}{color.G:X2}{color.B:X2}");
        }

        private static void SpawnHistoryColorItem(UiWindow window, UiWindow parentwindow, Color color, UiSpacer spacer, Action<Color> setColor,
            Action<UiWindow, UiSpacer, Action<Color>> renew, Action<Color> remove, bool showAddFav, out UiColumn colorSquare, out UiTextBlock colorText, out UiRow buttons) {
            SpawnColorItem(color, spacer, out colorSquare, out colorText);

            var buttonSpacer = UiSpacer.GetSpacer(5, 5);

            buttons = new UiRow();

            if (showAddFav) {
                var addToFavButton = new UiButton();
                addToFavButton.SetBackground(Constants.Backgrounds.Button);
                var addToFavePic = FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.addFavorite");
                addToFavButton.AddChild(addToFavePic);
                buttons.AddChild(addToFavButton);
                buttons.AddChild(buttonSpacer);

                addToFavButton.OnClick += () => {
                    _historyColor = color;
                    SpawnAddToFavFromHistoryMenu(parentwindow, spacer, setColor);
                };
            }

            var setButton = new UiButton();
            setButton.SetBackground(Constants.Backgrounds.Button);
            var setPic = FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.set");
            setButton.AddChild(setPic);
            setButton.OnClick += () => {
                setColor(color);
                parentwindow.Dispose();
            };
            buttons.AddChild(setButton);
            buttons.AddChild(buttonSpacer);

            var deleteButton = new UiButton();
            deleteButton.SetBackground(Constants.Backgrounds.Button);
            var deletePic = FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.delete");
            deleteButton.AddChild(deletePic);
            deleteButton.OnClick += () => {
                remove(color);
                parentwindow.Dispose();
                renew(window, spacer, setColor);
            };
            buttons.AddChild(deleteButton);
        }

        private static UiElement SpawnFolderItem(UiWindow window, UiWindow parent, UiSpacer spacer, Action<Color> setColor, string folder, Func<string, bool> onClick, Action<UiWindow, UiSpacer, Action<Color>> renew) {
            var itemRow = new UiRow();

            var itemButton = new UiButton();
            itemRow.AddChild(itemButton);
            var itemText = new UiTextBlock();
            itemText.SetString(folder);
            itemButton.AddChild(itemText);
            itemRow.AddChild(spacer);
            itemButton.OnClick += () => {
                if (onClick(folder)) {
                    parent.Dispose();
                }
            };

            var deleteButton = new UiButton();
            itemRow.AddChild(deleteButton);
            var deleteIcon = FoxUIHook.Instance.GetPicture("nimbusfox.ui.images.delete.text");
            deleteButton.AddChild(deleteIcon);
            deleteButton.OnClick += () => {
                var confirmWindow = new UiWindow();
                confirmWindow.Container.SetBackground(Constants.Backgrounds.Dark);
                parent.AddChildWindow(confirmWindow);

                var confirmTitle = new UiTextBlock();
                confirmWindow.AddChild(confirmTitle);
                confirmTitle.SetFont(Constants.Fonts.MyFirstCrush36);
                confirmTitle.SetColor(Color.Orange);
                confirmTitle.SetString($"Delete \"{folder}\"?");
                confirmWindow.AddChild(spacer);

                var confirmMessage = new UiTextBlock();
                confirmWindow.AddChild(confirmMessage);
                confirmMessage.SetString($"Are you sure you want to delete \"{folder}\"?");
                confirmWindow.AddChild(spacer);

                var buttonRow = new UiRow();
                confirmWindow.AddChild(buttonRow);

                var confirmButton = new UiButton();
                buttonRow.AddChild(confirmButton);
                var confirmButtonText = new UiTextBlock();
                confirmButton.AddChild(confirmButtonText);
                confirmButtonText.SetString("Delete");
                buttonRow.AddChild(spacer);
                confirmButton.OnClick += () => {
                    if (_favDir.FileExists(folder)) {
                        _favDir.DeleteFile(folder);
                    }
                    parent.Dispose();
                    renew(window, spacer, setColor);
                };

                confirmWindow.OnClose += () => {
                    parent.ListenForEscape(true);
                    parent.Show();
                    parent.StartUpdateCalls();
                };

                var cancelButton = new UiButton();
                buttonRow.AddChild(cancelButton);
                var cancelText = new UiTextBlock();
                cancelButton.AddChild(cancelText);
                cancelText.SetString("Cancel");

                cancelButton.OnClick += () => { confirmWindow.Dispose(); };

                parent.Hide();
                parent.ListenForEscape(false);
                parent.StopUpdateCalls();
                confirmWindow.Show();
            };

            return itemRow;
        }

        private static void SpawnInputTextWindow(UiWindow window, UiSpacer spacer,
            string labelText, string titleText, Func<string, bool> validationAndSuccess, string errorTitle, string errorMessage, Action onClose) {
            var textInputWindow = new UiWindow();
            textInputWindow.Container.SetBackground(Constants.Backgrounds.Dark);
            textInputWindow.ListenForEscape(true);

            textInputWindow.OnClose += () => {
                onClose();
            };

            var title = new UiTextBlock();
            title.SetColor(Color.Orange);
            title.SetFont(Constants.Fonts.MyFirstCrush36);
            title.SetString(titleText);
            textInputWindow.AddChild(title);
            textInputWindow.AddChild(spacer);

            var inputRow = new UiRow();
            textInputWindow.AddChild(inputRow);
            textInputWindow.AddChild(spacer);

            var label = new UiTextBlock();
            label.SetString(labelText);
            inputRow.AddChild(label);
            inputRow.AddChild(spacer);

            var input = new UiTextInput();
            input.SetBackgroundColor(new Color(237, 207, 154));
            input.SetTextColor(Color.Black);
            input.SetActiveTextColor(Color.Black);
            input.SetBackground(Constants.Backgrounds.TextInput);
            inputRow.AddChild(input);
            input.SetSize(300);
            input.SetLimit(25);

            var buttonRow = new UiRow();
            textInputWindow.AddChild(buttonRow);

            var createButton = new UiButton();
            buttonRow.AddChild(createButton);
            buttonRow.AddChild(spacer);
            var createText = new UiTextBlock();
            createText.SetString("Create");
            createButton.AddChild(createText);

            createButton.OnClick += () => {
                if (validationAndSuccess(input.GetValue())) {
                    textInputWindow.Dispose();
                } else {
                    var errorWindow = new UiWindow();
                    errorWindow.Container.SetBackground(Constants.Backgrounds.Dark);

                    var errorTitleText = new UiTextBlock();
                    errorTitleText.SetColor(Color.Orange);
                    errorTitleText.SetFont(Constants.Fonts.MyFirstCrush36);
                    errorTitleText.SetString(errorTitle);
                    errorWindow.AddChild(errorTitleText);
                    errorWindow.AddChild(spacer);

                    var message = new UiTextBlock();
                    message.SetString(errorMessage);
                    message.SetColor(Color.Red);
                    errorWindow.AddChild(message);
                    errorWindow.AddChild(spacer);

                    var okButton = new UiButton();
                    var okText = new UiTextBlock();
                    okText.SetString("OK");
                    okButton.AddChild(okText);
                    okButton.OnClick += () => {
                        textInputWindow.StartUpdateCalls();
                        errorWindow.Dispose();
                    };
                    errorWindow.AddChild(okButton);

                    textInputWindow.AddChildWindow(errorWindow);
                    textInputWindow.ListenForEscape(false);
                    textInputWindow.StopUpdateCalls();
                    errorWindow.ListenForEscape(true);

                    errorWindow.OnClose += () => {
                        textInputWindow.ListenForEscape(true);
                        textInputWindow.StartUpdateCalls();
                    };

                    errorWindow.Show();
                }
            };

            var cancelButton = new UiButton();
            buttonRow.AddChild(cancelButton);
            var cancelText = new UiTextBlock();
            cancelButton.AddChild(cancelText);
            cancelText.SetString("Cancel");

            cancelButton.OnClick += () => {
                textInputWindow.Dispose();
            };

            window.Hide();
            window.ListenForEscape(false);
            window.AddChildWindow(textInputWindow);
            window.StopUpdateCalls();
            textInputWindow.Show();
        }
    }
}
