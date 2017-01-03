using System.IO;
using Microsoft.Win32;

namespace ExampleGraphView {
    public static class FileDialogHandler<T> where T : FileDialog, new() {
        public static string OpenDialog(string defaultFilename = null) {
            var saveFileDialog = new T();
            if (!string.IsNullOrEmpty(defaultFilename)) {
                var file = new FileInfo(defaultFilename);
                saveFileDialog.FileName = file.Name;
                saveFileDialog.InitialDirectory = file.DirectoryName;
            }
            if (saveFileDialog.ShowDialog() == true) {
                return saveFileDialog.FileName;
            }
            return null;
        }
    }
}