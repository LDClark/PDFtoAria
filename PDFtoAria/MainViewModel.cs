using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Input;
using VMS.OIS.ARIALocal.WebServices.Document.Contracts;
using VMS.TPS.Common.Model.API;

namespace PDFtoAria
{
    public class MainViewModel : ViewModelBase
    {
        private string _patientId;
        public string PatientId
        {
            get => _patientId;
            set => Set(ref _patientId, value);
        }
        private string _dateOfService;
        public string DateOfService
        {
            get => _dateOfService;
            set => Set(ref _dateOfService, value);
        }
        private string _dateEntered;
        public string DateEntered
        {
            get => _dateEntered;
            set => Set(ref _dateEntered, value);
        }
        private byte[] _binaryContent;
        public byte[] BinaryContent
        {
            get => _binaryContent;
            set => Set(ref _binaryContent, value);
        }
        public string Directory { get; set; }
        private User _appUser;
        public User AppUser
        {
            get => _appUser;
            set => Set(ref _appUser, value);
        }
        private DocumentUser _authoredByUser;
        public DocumentUser AuthoredByUser
        {
            get => _authoredByUser;
            set => Set(ref _authoredByUser, value);
        }
        private DocumentUser _supervisedByUser;
        public DocumentUser SupervisedByUser
        {
            get => _supervisedByUser;
            set => Set(ref _supervisedByUser, value);
        }
        private DocumentUser _enteredByUser;
        public DocumentUser EnteredByUser
        {
            get => _enteredByUser;
            set => Set(ref _enteredByUser, value);
        }
        private DocumentType _documentType;
        public DocumentType DocumentType
        {
            get => _documentType;
            set => Set(ref _documentType, value);
        }
        private string _templateName;
        public string TemplateName
        {
            get => _templateName;
            set => Set(ref _templateName, value);
        }
        private FileFormat _fileFormat;
        public FileFormat FileFormat
        {
            get => _fileFormat;
            set => Set(ref _fileFormat, value);
        }
        private FileInfo[] _files;
        public FileInfo[] Files
        {
            get => _files;
            set => Set(ref _files, value);
        }
        private FileInfo _selectedFile;
        public FileInfo SelectedFile
        {
            get => _selectedFile;
            set => Set(ref _selectedFile, value);
        }
        public ICommand GetFilesCommand => new RelayCommand(GetFiles);
        public ICommand GetPDFCommand => new RelayCommand(GetPDF);
        public ICommand UploadToAriaCommand => new RelayCommand(UploadToAria);
        public MainViewModel(User user)
        {
            AppUser = user;
            Directory = ConfigurationManager.AppSettings["importDir"];
            DateOfService = $"/Date({Math.Floor((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)})/";
            DateEntered = $"/Date({Math.Floor((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)})/";
            AuthoredByUser = new DocumentUser
            {
                SingleUserId = user.Id
            };
            SupervisedByUser = new DocumentUser
            {
                SingleUserId = user.Id
            };
            EnteredByUser = new DocumentUser
            {
                SingleUserId = user.Id
            };
            FileFormat = FileFormat.PDF;
            
            DocumentType = new DocumentType
            {
                DocumentTypeDescription = "Treatment Plan"
            };
        }
        public void GetFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(Directory);
            Files = dir.GetFiles("*.pdf");
            DateOfService = $"/Date({Math.Floor((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)})/";
            DateEntered = $"/Date({Math.Floor((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)})/";
            TemplateName = SelectedFile.FullName.Split('\\').Last().Split('.').First();
            BinaryContent = File.ReadAllBytes(SelectedFile.FullName);
        }
        public void GetPDF()
        {
            DateOfService = $"/Date({Math.Floor((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)})/";
            DateEntered = $"/Date({Math.Floor((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)})/";
            TemplateName = SelectedFile.FullName.Split('\\').Last().Split('.').First();
            BinaryContent = File.ReadAllBytes(SelectedFile.FullName);
        }
        public void UploadToAria()
        {            
            CustomInsertDocumentsParameter.PostDocumentData(PatientId, AppUser,
                BinaryContent, TemplateName, DocumentType);
        }
    }
}
