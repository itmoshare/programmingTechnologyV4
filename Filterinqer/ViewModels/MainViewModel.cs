using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Filterinqer
{
    public class MainViewModel : BaseViewModel
    {
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
        public ICommand SaveCommand { get => _saveCommand; }
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
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filePath);
            bitmap.EndInit();
            SelectedImageSource = bitmap;
            IsImageSelected = true;
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
