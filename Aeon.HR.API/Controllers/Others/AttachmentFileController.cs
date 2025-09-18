using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.ViewModels.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Configuration;
using System.IO;
using Aeon.HR.ViewModels;
using System.Web;
using AutoMapper;
using Aeon.HR.API.Helpers;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using System.Security.Policy;
using File = System.IO.File;
using DocumentFormat.OpenXml.Office2013.Drawing.Chart;

namespace Aeon.HR.API.Controllers.Others
{
    [RoutePrefix("api/AttachmentFile")]
    public class AttachmentFileController : ApiController
    {
        private readonly ILogger<AttachmentFileController> _logger;
        private readonly IAttachmentFileBO _attachmentFileBO;
        private string _uploadedFilesFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Attachments");

        public AttachmentFileController(IAttachmentFileBO attachmentFileBO, ILogger<AttachmentFileController> logger)
        {
            _logger = logger;
            _attachmentFileBO = attachmentFileBO;
        }

        [HttpPost]
        [Route("UploadFiles")]
        public async Task<IHttpActionResult> UploadFiles()
        {
            var result = new ResultDTO();
            try
            {
                Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists

                var provider = new MultipartFormDataStreamProvider(_uploadedFilesFolder);
                var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                foreach (var header in Request.Content.Headers)
                {
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                await content.ReadAsMultipartAsync(provider);

                if (provider.FileData.Count == 0)
                {
                    result.Messages.Add("No file is attach");
                    result.ErrorCodes.Add(0);
                    return Ok(result);
                }

                var fileResults = new List<AttachmentFileViewModel>();

                for (int i = 0; i < provider.FileData.Count; i++)
                {
                    string uploadingFileName = provider.FileData[i].LocalFileName;
                    string originalFileName = String.Concat(_uploadedFilesFolder, "\\" + (provider.Contents[i].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' }));
                    var fileUniqueName = $"{Path.GetFileNameWithoutExtension(originalFileName)}_{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
                    var filePath = Path.Combine(_uploadedFilesFolder, fileUniqueName);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    File.Move(uploadingFileName, filePath);

                    var fileResult = await _attachmentFileBO.Save(new AttachmentFileViewModel
                    {
                        Extension = Path.GetExtension(originalFileName),
                        FileDisplayName = $"{Path.GetFileNameWithoutExtension(originalFileName)}{Path.GetExtension(originalFileName)}",
                        FileUniqueName = fileUniqueName,
                        Name = $"{Path.GetFileNameWithoutExtension(originalFileName)}{Path.GetExtension(originalFileName)}",
                        Size = (new System.IO.FileInfo(filePath).Length) / 1024,
                        Type = provider.FileData[i].Headers.ContentType.MediaType,
                    });

                    if (fileResult != null && fileResult.Object != null && fileResult.Object is AttachmentFileViewModel)
                    {
                        fileResults.Add(fileResult.Object as AttachmentFileViewModel);
                    }
                }

                result.Object = fileResults;
            }
            catch (Exception ex)
            {
                result.Object = false;
                result.Messages.Add(ex.Message);
                result.ErrorCodes.Add(0);

                _logger.LogError(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(Guid id, bool getBase64Image = false)
        {
            var res = new ResultDTO();
            try
            {
                if (id == Guid.Empty)
                {
                    res.Messages.Add("File Id is invalid");
                }

                var attachment = await _attachmentFileBO.Get(id);
                if (attachment == null)
                {
                    res.Messages.Add("File Id is invalid");
                }

                var att = Mapper.Map<AttachmentFileViewModel>(attachment);

                if (getBase64Image)
                {
                    try
                    {
                        Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists
                        var filepath = Path.Combine(_uploadedFilesFolder, attachment.FileUniqueName);
                        byte[] imageArray = System.IO.File.ReadAllBytes(filepath);
                        //att.Base64ImageValue = Convert.ToBase64String(imageArray);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }

                res.Object = att;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                res.Messages.Add(ex.Message);
                return Ok(res);
            }
            return Ok(res);
        }

        public async Task<ResultDTO> Delete(Guid id)
        {
            var res = await _attachmentFileBO.Delete(id);
            return res;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetImage(Guid id)
        {
            var res = new ResultDTO();
            try
            {
                if (id == Guid.Empty)
                {
                    res.Messages.Add("Id is null");
                }

                var attachment = await _attachmentFileBO.Get(id);
                if (attachment == null)
                {
                    res.Messages.Add("File Id not found");
                }

                Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists
                var filepath = Path.Combine(_uploadedFilesFolder, attachment.FileUniqueName);

                byte[] imageArray = File.ReadAllBytes(filepath);
                res.Object = Convert.ToBase64String(imageArray);

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                res.Messages.Add(ex.Message);
                return Ok(res);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> Download(Guid id)
        {
            var resultDto = new ResultDTO();
            byte[] fileBytes = null;
            try
            {
                if (id == Guid.Empty)
                {
                    resultDto.Messages.Add("Guid not empty");
                    return Ok(resultDto);
                }

                var attachment = await _attachmentFileBO.Get(id);
                if (attachment == null)
                {
                    resultDto.Messages.Add("File Id not found");
                    return Ok(resultDto);
                }
                //Tìm kiếm file tại sharepoint List trước không có thì vòng quay lại Attachment
                try
                {
                    var _linkdowload = "/Shared Documents";
                    string linkFile = _linkdowload + "/" + attachment.Id + "/" + attachment.FileUniqueName;
                    fileBytes = this.GetDataFromUrl(linkFile);
                }
                catch (Exception ex)
                {
                    fileBytes = null;
                    _logger.LogError(ex, ex.Message);
                }
                // nếu vẫn bằng null thì vòng quay lại Attachment down xem có không cho chắc
                if (fileBytes == null)
                {
                    Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists
                    var filepath = Path.Combine(_uploadedFilesFolder, attachment.FileUniqueName);
                    if (!File.Exists(filepath) && attachment.Extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        filepath = Path.Combine(_uploadedFilesFolder, "zip", attachment.FileUniqueName);
                    }

                    if (!File.Exists(filepath))
                    {
                        // check trên từng con trước
                        try
                        {
                            var ip73AttachmentFiles = ConfigurationManager.AppSettings["RootAttachment73"];
                            filepath = Path.Combine(ip73AttachmentFiles, attachment.FileUniqueName);
                            if (!File.Exists(filepath) && attachment.Extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                            {
                                filepath = Path.Combine(ip73AttachmentFiles, "zip", attachment.FileUniqueName);
                            }

                            if (!File.Exists(filepath))
                            {
                                var ip74AttachmentFiles = ConfigurationManager.AppSettings["RootAttachment74"];
                                filepath = Path.Combine(ip74AttachmentFiles, attachment.FileUniqueName);
                                if (!File.Exists(filepath) && attachment.Extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    filepath = Path.Combine(ip74AttachmentFiles, "zip", attachment.FileUniqueName);
                                }
                                if (!File.Exists(filepath))
                                {
                                    resultDto.Messages.Add("File not found");
                                    return Ok(resultDto);
                                }
                            }
                        } catch (Exception e)
                        {

                        }
                    }
                    fileBytes = File.ReadAllBytes(filepath);
                }

                if (fileBytes != null)
                {
                    var fileResult = new FileResultDto
                    {
                        FileName = attachment.FileDisplayName,
                        Content = fileBytes,
                        Type = attachment.Type,
                    };
                    resultDto.Object = fileResult;
                    return Ok(resultDto);
                }
                else
                {
                    resultDto.Messages.Add("File Id not found");
                    return Ok(resultDto);
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                resultDto.Messages.Add(ex.Message);
                return Ok(resultDto);
            }
        }


        [HttpGet]
        public async Task<IHttpActionResult> DownloadBK(Guid id)
        {
            var resultDto = new ResultDTO();
            try
            {
                if (id == Guid.Empty)
                {
                    resultDto.Messages.Add("Guid not empty");
                    return Ok(resultDto);
                }

                var attachment = await _attachmentFileBO.Get(id);
                if (attachment == null)
                {
                    resultDto.Messages.Add("File Id not found");
                    return Ok(resultDto);
                }

                Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists
                var filepath = Path.Combine(_uploadedFilesFolder, attachment.FileUniqueName);
                if (!File.Exists(filepath) && attachment.Extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    filepath = Path.Combine(_uploadedFilesFolder, "zip", attachment.FileUniqueName);
                }

                if (!File.Exists(filepath))
                {
                    resultDto.Messages.Add("File not found");
                    return Ok(resultDto);
                }

                byte[] fileBytes = File.ReadAllBytes(filepath);
                var fileResult = new FileResultDto
                {
                    FileName = attachment.FileDisplayName,
                    Content = fileBytes,
                    Type = MimeTypeMapHelper.GetMimeType(Path.GetExtension(filepath))
                };
                resultDto.Object = fileResult;
                // return Ok(File(fileBytes, MimeTypeMapHelper.GetMimeType(Path.GetExtension(filepath)), attachment.FileDisplayName));
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                resultDto.Messages.Add(ex.Message);
                return Ok(resultDto);
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetFilePath(Guid id)
        {
            var resultDto = new ResultDTO();
            try
            {
                if (id == Guid.Empty)
                {
                    resultDto.Messages.Add("Guid not empty");
                    return Ok(resultDto);
                }

                var attachment = await _attachmentFileBO.Get(id);
                if (attachment == null)
                {
                    resultDto.Messages.Add("File Id not found");
                    return Ok(resultDto);
                }

                Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists
                var filepath = Path.Combine(_uploadedFilesFolder, attachment.FileUniqueName);
                if (!File.Exists(filepath) && attachment.Extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    filepath = Path.Combine(_uploadedFilesFolder, "zip", attachment.FileUniqueName);
                }

                if (!File.Exists(filepath))
                {
                    // check trên từng con trước
                    var ip73AttachmentFiles = ConfigurationManager.AppSettings["RootAttachment73"];
                    filepath = Path.Combine(ip73AttachmentFiles, attachment.FileUniqueName);
                    if (!File.Exists(filepath) && attachment.Extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        filepath = Path.Combine(ip73AttachmentFiles, "zip", attachment.FileUniqueName);
                    }

                    if (!File.Exists(filepath))
                    {
                        var ip74AttachmentFiles = ConfigurationManager.AppSettings["RootAttachment74"];
                        filepath = Path.Combine(ip74AttachmentFiles, attachment.FileUniqueName);
                        if (!File.Exists(filepath) && attachment.Extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                        {
                            filepath = Path.Combine(ip74AttachmentFiles, "zip", attachment.FileUniqueName);
                        }
                        if (!File.Exists(filepath))
                        {
                            resultDto.Messages.Add("File not found");
                        }
                    }
                }

                resultDto.Object = filepath;
                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                resultDto.Messages.Add(ex.Message);
                return Ok(resultDto);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadImage(Guid userId, Guid profilePictureId)  // lưu trả về 1 value kiểu string đường dẫn 
        {
            var result = new ResultDTO();
            try
            {
                Directory.CreateDirectory(_uploadedFilesFolder); // Make sure the folder exists

                var provider = new MultipartFormDataStreamProvider(_uploadedFilesFolder);
                var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                foreach (var header in Request.Content.Headers)
                {
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                await content.ReadAsMultipartAsync(provider);

                if (provider.FileData.Count == 0)
                {
                    result.Messages.Add("No file is attach");
                    result.ErrorCodes.Add(0);
                    return Ok(result);
                }

                var fileResults = new List<AttachmentFileViewModel>();
                for (int i = 0; i < provider.FileData.Count; i++)
                {
                    string uploadingFileName = provider.FileData[i].LocalFileName;
                    string originalFileName = String.Concat(_uploadedFilesFolder, "\\" + (provider.Contents[i].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' }));
                    var fileUniqueName = "user-profile-picture-" + $"{userId}_{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
                    var filePath = Path.Combine(_uploadedFilesFolder, fileUniqueName);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    File.Move(uploadingFileName, filePath);

                    //byte[] imageArray = System.IO.File.ReadAllBytes(filepath);
                    //var bbb = Convert.ToBase64String(imageArray);
                    var fileResult = await _attachmentFileBO.Save(new AttachmentFileViewModel
                    {
                        Id = profilePictureId,
                        Extension = Path.GetExtension(originalFileName),
                        FileDisplayName = $"{Path.GetFileNameWithoutExtension(originalFileName)}{Path.GetExtension(originalFileName)}",
                        FileUniqueName = fileUniqueName,
                        Name = $"{Path.GetFileNameWithoutExtension(originalFileName)}{Path.GetExtension(originalFileName)}",
                        Size = (new System.IO.FileInfo(filePath).Length) / 1024,
                        Type = provider.FileData[i].Headers.ContentType.MediaType,
                        Base64ImageValue = System.IO.File.ReadAllBytes(Path.Combine(_uploadedFilesFolder, fileUniqueName))
                    }); ;

                    if (fileResult != null && fileResult.Object != null && fileResult.Object is AttachmentFileViewModel)
                    {
                        fileResults.Add(fileResult.Object as AttachmentFileViewModel);
                        var data = fileResult.Object as AttachmentFileViewModel;
                        result.Object = data.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Object = false;
                result.Messages.Add(ex.Message);
                result.ErrorCodes.Add(0);

                _logger.LogError(ex.Message);
            }

            return Ok(result);
        }
        [HttpPost]
        public async Task<IHttpActionResult> DeleteMultiFile(Guid[] ids)
        {
            try
            {
                return Ok(await _attachmentFileBO.DeleteMultiFile(ids));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error at: DeleteMultiFile", ex.Message);
                return BadRequest("System Error");
            }
        }
        [HttpPost]
        public async Task<IHttpActionResult> UploadImageByType(string type, Guid id, Guid profilePictureId)
        {
            var result = new ResultDTO();
            try
            {
                if (type != null && !type.Equals(""))
                {
                    Directory.CreateDirectory(_uploadedFilesFolder);

                    var provider = new MultipartFormDataStreamProvider(_uploadedFilesFolder);
                    var content = new StreamContent(HttpContext.Current.Request.GetBufferlessInputStream(true));
                    foreach (var header in Request.Content.Headers)
                    {
                        content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }

                    await content.ReadAsMultipartAsync(provider);

                    if (provider.FileData.Count == 0)
                    {
                        result.Messages.Add("No file is attach");
                        result.ErrorCodes.Add(0);
                        return Ok(result);
                    }

                    var fileResults = new List<AttachmentFileViewModel>();
                    for (int i = 0; i < provider.FileData.Count; i++)
                    {
                        string uploadingFileName = provider.FileData[i].LocalFileName;
                        string originalFileName = String.Concat(_uploadedFilesFolder, "\\" + (provider.Contents[i].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' }));
                        var fileUniqueName = type + "-profile-picture-" + $"{id}_{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
                        var filePath = Path.Combine(_uploadedFilesFolder, fileUniqueName);

                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        File.Move(uploadingFileName, filePath);

                        var fileResult = await _attachmentFileBO.Save(new AttachmentFileViewModel
                        {
                            Id = profilePictureId,
                            Extension = Path.GetExtension(originalFileName),
                            FileDisplayName = $"{Path.GetFileNameWithoutExtension(originalFileName)}{Path.GetExtension(originalFileName)}",
                            FileUniqueName = fileUniqueName,
                            Name = $"{Path.GetFileNameWithoutExtension(originalFileName)}{Path.GetExtension(originalFileName)}",
                            Size = (new System.IO.FileInfo(filePath).Length) / 1024,
                            Type = provider.FileData[i].Headers.ContentType.MediaType,
                            Base64ImageValue = System.IO.File.ReadAllBytes(Path.Combine(_uploadedFilesFolder, fileUniqueName))
                        });

                        if (fileResult != null && fileResult.Object != null && fileResult.Object is AttachmentFileViewModel)
                        {
                            fileResults.Add(fileResult.Object as AttachmentFileViewModel);
                            var data = fileResult.Object as AttachmentFileViewModel;
                            result.Object = data.Id;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.Object = false;
                result.Messages.Add(ex.Message);
                result.ErrorCodes.Add(0);

                _logger.LogError(ex.Message);
            }
            return Ok(result);
        }
        public byte[] GetDataFromUrl(string fullUrl)
        {
            byte[] m_Result = null;

            var _SharePointURL = ConfigurationManager.AppSettings["siteUrl"];

            var _Username = ConfigurationManager.AppSettings["SharePoint_Username"];
            var _Domain = ConfigurationManager.AppSettings["SharePoint_Domain"];
            var _Password = ConfigurationManager.AppSettings["SharePoint_Password"];

            using (ClientContext Context = new ClientContext(_SharePointURL))
            {

                Context.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                Context.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(_Username, _Password);
                Microsoft.SharePoint.Client.File filetoDownload = Context.Web.GetFileByServerRelativeUrl(fullUrl);
                Context.Load(filetoDownload);
                Context.ExecuteQuery();

                var stream = filetoDownload.OpenBinaryStream();
                Context.ExecuteQuery();
                using (var memoryStream = new MemoryStream())
                {
                    stream.Value.CopyTo(memoryStream);
                    m_Result = memoryStream.ToArray();
                }
            }

            return m_Result;
        }
    }
}
