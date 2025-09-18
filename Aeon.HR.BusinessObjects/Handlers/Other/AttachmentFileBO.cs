using Aeon.HR.BusinessObjects.Interfaces;
using Aeon.HR.Data.Models;
using Aeon.HR.Infrastructure.Interfaces;
using Aeon.HR.ViewModels;
using Aeon.HR.ViewModels.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.BusinessObjects.Handlers.Other
{
    public class AttachmentFileBO: IAttachmentFileBO
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AttachmentFileBO> _logger;
        public AttachmentFileBO(IUnitOfWork uow, ILogger<AttachmentFileBO> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<ResultDTO> Save(AttachmentFileViewModel model)
        {
            var res = new ResultDTO();
            try
            {
                if (model == null)
                {
                    res.Messages.Add("Attachment to save is null");
                    res.ErrorCodes.Add(0);
                    return res;
                }

                if (model.Id == Guid.Empty) // Create
                {
                    var attachment = Mapper.Map<AttachmentFile>(model);

                    _uow.GetRepository<AttachmentFile>().Add(attachment);
                    await _uow.CommitAsync();

                    res.Object = Mapper.Map<AttachmentFileViewModel>(attachment);
                }
                else
                {
                    var att = await _uow.GetRepository<AttachmentFile>().GetSingleAsync(x => x.Id == model.Id);
                    if (att != null)
                    {
                        Mapper.Map(model, att);

                        await _uow.CommitAsync();

                        res.Object = Mapper.Map<AttachmentFileViewModel>(att);
                    }
                    else
                    {
                        res.Messages.Add("Can not find Attachment");
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                res.Messages.Add(ex.Message);
            }
            return res;
        }

        public async Task<AttachmentFileViewModel> Get(Guid id)
        {
            AttachmentFileViewModel res = null;
            try
            {
                if (id != Guid.Empty)
                {
                    var att = await _uow.GetRepository<AttachmentFile>().GetSingleAsync(x => x.Id == id);
                    if (att != null)
                    {
                        res = Mapper.Map<AttachmentFileViewModel>(att);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return res;
        }

        public async Task<ResultDTO> Delete(Guid id)
        {
            var res = new ResultDTO();
            try
            {
                if (id == Guid.Empty)
                {
                    res.Messages.Add("Attachment Id is not valid");
                    return res;
                }

                var attachment = await _uow.GetRepository<AttachmentFile>().GetSingleAsync(x => x.Id == id);
                if (attachment != null)
                {
                    _uow.GetRepository<AttachmentFile>().Delete(attachment);
                    await _uow.CommitAsync();
                }
                else
                {
                    res.Messages.Add("Can not find Attachment");
                    return res;
                }

                res.Object = Mapper.Map<AttachmentFileViewModel>(attachment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                res.Messages.Add(ex.Message);
            }
            return res;
        }
        public async Task<ResultDTO> DeleteMultiFile(Guid[] ids)
        {
            var res = new ResultDTO();
            try
            {
                var attachments = await _uow.GetRepository<AttachmentFile>().FindByAsync(x => ids.Contains(x.Id));
                if (attachments.Any())
                {
                    _uow.GetRepository<AttachmentFile>().Delete(attachments);
                    await _uow.CommitAsync();
                }
                else
                {
                    res.Messages.Add("Can not find Attachment");
                    return res;
                }

                res.Object = Mapper.Map<List<AttachmentFileViewModel>>(attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                res.Messages.Add(ex.Message);
            }
            return res;
        }
    }
}
