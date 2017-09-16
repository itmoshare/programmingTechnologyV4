using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using Filterinqer.Helpers;

namespace Filterinqer
{
    public class MainViewModel : BaseViewModel
    {
        private const string BAD_IMAGE_FORMAT = "Возможно обработать изображение только форматов .png, .jpg или .bmp.";

        private ICommand _openCommand;
        private ICommand _saveCommand;
        private ICommand _sobelCommand;
        private ICommand _gausCommand;
        private ICommand _undoCommand;

        private string _selectedFilePath;
        private BitmapImage _selectedImageSource;
        private bool _isImageSelected;

        private FixedSizedStack<BitmapImage> _images;

        public MainViewModel()
        {
            _images = new FixedSizedStack<BitmapImage>(20);
        }

        public ICommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = 
                    new Command((param) => true, OpenFileAction));
            } 
        }
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand =
                    new Command((param) => IsImageSelected, SaveAction));
            }
        }

        public ICommand SobelCommand
        {
            get
            {
                return _sobelCommand ?? (_sobelCommand =
                    new Command((param) => IsImageSelected, () =>
                    {
                        SelectedImageSource = ApplySobel(SelectedImageSource);
                    }));
            }
        }

        public ICommand GausCommand
        {
            get
            {
                return _gausCommand ?? (_gausCommand =
                    new Command((param) => IsImageSelected, () =>
                    {
                        SelectedImageSource = ApplyGaus(SelectedImageSource);
                    }));
            }
        }

        public ICommand UndoCommand
        {
            get
            {
                return _undoCommand ?? (_undoCommand =
                    new Command((param) => _images.Count > 1, () =>
                    {
                        _images.Pop();
                        SelectedImageSource = _images.Peek();
                    }));
            }
        }

        public string SelectedFile { get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                NotifyPropertyChanged(nameof(SelectedFile));
            }
        }

        public BitmapImage SelectedImageSource { get => _selectedImageSource;
            set
            {
                _selectedImageSource = value;
                NotifyPropertyChanged(nameof(SelectedImageSource));
            }
        }

        public bool IsImageSelected { get => _isImageSelected;
            set
            {
                _isImageSelected = value;
                NotifyPropertyChanged(nameof(IsImageSelected));
            }
        }


        public void OpenFileAction()
        {
            var filePath = ChooseFile();
            if (string.IsNullOrEmpty(filePath))
                return;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.EndInit();

                SelectedFile = filePath;
                SelectedImageSource = bitmap;
                IsImageSelected = true;
                _images.Clear();
                _images.Push(bitmap);
            }
            catch (NotSupportedException e)
            {
                throw new NotSupportedException(BAD_IMAGE_FORMAT, e);
            }
        }

        public void SaveAction()
        {
            BitmapEncoder encoder;
            if (string.Equals(Path.GetExtension(SelectedFile), ".png", StringComparison.InvariantCultureIgnoreCase))
                encoder = new PngBitmapEncoder();
            else if (string.Equals(Path.GetExtension(SelectedFile), ".jpg", StringComparison.InvariantCultureIgnoreCase))
                encoder = new JpegBitmapEncoder();
            else
                encoder = new BmpBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(SelectedImageSource));

            var savePath = GetSaveFilePath();
            if (string.IsNullOrEmpty(savePath))
                return;

            using (var fileStream = new FileStream(savePath, FileMode.OpenOrCreate))
                encoder.Save(fileStream);
        }

        private BitmapImage ApplySobel(BitmapImage image)
        {
            var sobeled = image.ToCVImage().Sobel(1, 0, 3);
            var result = sobeled.ToBitmap().ToBitmapImage();
            _images.Push(result);
            return result;
        }

        private BitmapImage ApplyGaus(BitmapImage image)
        {
            var gaused = new Image<Bgr, byte>(image.ToBitmap());
            var cvImage = image.ToCVImage();
            CvInvoke.GaussianBlur(cvImage, gaused, new Size { Height = 25, Width = 25 }, 0, 0);
            var result = gaused.ToBitmap().ToBitmapImage();
            _images.Push(result);
            return result;
        }

        private string GetSaveFilePath()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                FileName = $"{Path.GetFileNameWithoutExtension(SelectedFile)}_filtered{Path.GetExtension(SelectedFile)}",
                Filter = "PNG or JPG|*.png;*.jpg"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                return saveFileDialog.FileName;
            return null;
        }

        private string ChooseFile()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Image files (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp|All files (*.*)|*.*",
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                return openFileDialog.FileName;
            else
                return null;
        }
    }
}
