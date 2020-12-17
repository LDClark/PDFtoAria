using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<FileViewModel> _files;
        public ObservableCollection<FileViewModel> Files
        {
            get => _files;
            set => Set(ref _files, value);
        }
        public IEnumerable<FileViewModel> SelectedFiles
        {
            get { return Files.Where(x => x.IsSelected); }
        }
        public ICommand GetFilesCommand => new RelayCommand(GetFiles);
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
            FileInfo[] fileInfos = dir.GetFiles("*.pdf");
            Files = new ObservableCollection<FileViewModel>();
            foreach (var file in fileInfos)
            {
                Files.Add( new FileViewModel
                {
                    FileName = file.Name,
                    FullPath = file.FullName,
                    CreationTime = file.CreationTime
                });
            }
            DateOfService = $"/Date({Math.Floor((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)})/";
            TemplateName = "";
        }
        public void UploadToAria()
        {
            PdfDocument outputDocument = new PdfDocument();
            foreach (FileViewModel file in SelectedFiles)
            {
                PdfDocument inputDocument = PdfReader.Open(file.FullPath, PdfDocumentOpenMode.Import);
                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    PdfPage page = inputDocument.Pages[i];
                    outputDocument.AddPage(page);
                }
            }
            MemoryStream stream = new MemoryStream();
            outputDocument.Save(stream, false);
            BinaryContent = stream.ToArray();

            CustomInsertDocumentsParameter.PostDocumentData(PatientId, AppUser,
                BinaryContent, TemplateName, DocumentType);
        }
    }
}
