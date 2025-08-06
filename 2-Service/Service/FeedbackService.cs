using AutoMapper;
using BusinessObject.Model;
using BusinessObject;
using BusinessObject.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessObject.RequestDTO.RequestDTO;
using static BusinessObject.ResponseDTO.ResponseDTO;
using _3_Repository.IRepository;
using _2_Service.Service.IService;

namespace _2_Service.Service
{
    //public interface IFeedbackService
    //{
    //    Task<IEnumerable<Feedback>> GetAllFeedbacks();
    //    Task<Feedback> GetFeedbackById(int id);
    //    Task AddFeedback(Feedback feedback);
    //    Task UpdateFeedback(Feedback feedback);
    //    Task DeleteFeedback(int id);
    //}

    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<IEnumerable<Feedback>> GetAllFeedbacks()
        {
            return await _feedbackRepository.GetAllAsync();
        }

        public async Task<Feedback> GetFeedbackById(int id)
        {
            return await _feedbackRepository.GetByIdAsync(id);
        }

        public async Task AddFeedback(Feedback feedback)
        {
            await _feedbackRepository.AddAsync(feedback);
        }

        public async Task UpdateFeedback(Feedback feedback)
        {
            await _feedbackRepository.UpdateAsync(feedback);
        }

        public async Task DeleteFeedback(int id)
        {
            await _feedbackRepository.DeleteAsync(id);
        }
    }
}
