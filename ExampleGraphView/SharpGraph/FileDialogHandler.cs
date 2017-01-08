using System.IO;
using Microsoft.Win32;

namespace ExampleGraphView {
    public static class FileDialogHandler<T> where T : FileDialog, new() {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
             "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static string OpenDialog(string defaultFileName = null) {
            var saveFileDialog = new T();
            if (!string.IsNullOrEmpty(defaultFileName)) {
                var file = new FileInfo(defaultFileName);
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