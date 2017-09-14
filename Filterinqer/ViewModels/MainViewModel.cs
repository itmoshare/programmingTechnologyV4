using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;

namespace Filterinqer
{
    public class MainViewModel : BaseViewModel
    {
        private const string BAD_IMAGE_FORMAT = "Возможно обработать изображение только форматов .png и .jpg.";

        private ICommand _openCommand;
        private ICommand _saveCommand;
        private ICommand _sobelCommand;
        private ICommand _gausCommand;

        private string _selectedFilePath;
        private BitmapImage _selectedImageSource;
        private bool _isImageSelected;

        public ICommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = 
                    new Command((param) => true, OpenFileProcess));
            } 
        }
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand =
                    new Command((param) => IsImageSelected, SaveProcess));
            }
        }

        public ICommand SobelCommand { get => _sobelCommand; }
        public ICommand GausCommand { get => _gausCommand; }

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


        public void OpenFileProcess()
        {
            var filePath = ChooseFile();
            if (string.IsNullOrEmpty(filePath))
                return;

            SelectedFile = filePath;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filePath);
            bitmap.EndInit();
            SelectedImageSource = bitmap;
            IsImageSelected = true;
        }

        public void SaveProcess()
        {
            BitmapEncoder encoder;
            if (string.Equals(Path.GetExtension(SelectedFile), ".png", StringComparison.InvariantCultureIgnoreCase))
                encoder = new PngBitmapEncoder();
            else if (string.Equals(Path.GetExtension(SelectedFile), ".jpg", StringComparison.InvariantCultureIgnoreCase))
                encoder = new JpegBitmapEncoder();
            else
                throw new InvalidOperationException(BAD_IMAGE_FORMAT);

            encoder.Frames.Add(BitmapFrame.Create(SelectedImageSource));

            var savePath = GetSaveFilePath();
            if (string.IsNullOrEmpty(savePath))
                return;

            using (var fileStream = new FileStream(savePath, FileMode.OpenOrCreate))
                encoder.Save(fileStream);
        }

        private string GetSaveFilePath()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                FileName = $"{Path.GetFileNameWithoutExtension(SelectedFile)}_filtered{Path.GetExtension(SelectedFile)}",
                DefaultExt = ".png",
                Filter = "PNG |*.png"
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
                Filter = "Image files (*.png, *.jpg)|*.png;*.jpg|All files (*.*)|*.*",
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                return openFileDialog.FileName;
            else
                return null;
        }
    }
}
