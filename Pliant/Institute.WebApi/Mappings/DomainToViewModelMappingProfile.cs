using AutoMapper;
using Institute.Entities;
using Institute.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Institute.WebApi.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public virtual string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected override void Configure()
        {
            Mapper.Reset();
            //Mapper.CreateMap<Movie, MovieViewModel>()
            //    .ForMember(vm => vm.Genre, map => map.MapFrom(m => m.Genre.Name))
            //    .ForMember(vm => vm.GenreId, map => map.MapFrom(m => m.Genre.ID))
            //    .ForMember(vm => vm.IsAvailable, map => map.MapFrom(m => m.Stocks.Any(s => s.IsAvailable)))
            //    .ForMember(vm => vm.NumberOfStocks, map => map.MapFrom(m => m.Stocks.Count))
            //    .ForMember(vm => vm.Image, map => map.MapFrom(m => string.IsNullOrEmpty(m.Image) == true ? "unknown.jpg" : m.Image));

            Mapper.CreateMap<Standard, StandardViewModel>()
                .ForMember(vm => vm.Standard, map => map.MapFrom(m => m.Name))
                .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.SubjectCount, map => map.MapFrom(m => m.SubjectMapping != null ? m.SubjectMapping.Count : 0))
                .ForMember(vm => vm.Division, map => map.MapFrom(m => m.Division));

            Mapper.CreateMap<Subject, SubjectViewModel>()
                .ForMember(vm => vm.Subject, map => map.MapFrom(m => m.Name))
                .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
                .ForMember(vm => vm.StandardCount, map => map.MapFrom(m => m.StandardMapping != null ? m.StandardMapping.Count : 0))
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID));

            //Mapper.CreateMap<Subject, Subject2ViewModel>()
            //    .ForMember(vm => vm.Standard, map => 
            //        map.MapFrom(m => m.StandardMapping.Select(s=>s.Standard.Name)));

            Mapper.CreateMap<Topic, TopicViewModel>()
                .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.IsActive, map => map.MapFrom(m => m.IsActive))
                .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
                .ForMember(vm => vm.Standard, map => map.MapFrom(m => m.Mapping.Standard.Name))
                .ForMember(vm => vm.StandardId, map => map.MapFrom(m => m.Mapping.Standard.ID))
                .ForMember(vm => vm.Subject, map => map.MapFrom(m => m.Mapping.Subject.Name))
                .ForMember(vm => vm.SubjectId, map => map.MapFrom(m => m.Mapping.Subject.ID))
                .ForMember(vm => vm.Objective, map => map.MapFrom(m => m.Objective))
                .ForMember(vm => vm.MappingID, map => map.MapFrom(m => m.MappingID))
                .ForMember(vm => vm.QuestionCount, map => map.MapFrom(m => m.Questions != null?m.Questions.Count:0))
                .ForMember(vm => vm.MappingID, map => map.MapFrom(m => m.Mapping.ID));

            Mapper.CreateMap<Question, QuestionViewModel>()
                .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.IsActive, map => map.MapFrom(m => m.IsActive))
                .ForMember(vm => vm.Points, map => map.MapFrom(m => m.Points))
                .ForMember(vm => vm.Type, map => map.MapFrom(m => m.Type))
                .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Status))
                .ForMember(vm => vm.Randomize, map => map.MapFrom(m => m.Randomize))
                .ForMember(vm => vm.Text, map => map.MapFrom(m => m.Text))
                .ForMember(vm => vm.TopicID, map => map.MapFrom(m => m.TopicId))
                .ForMember(vm => vm.TopicID, map => map.MapFrom(m => m.Topic.ID))
                .ForMember(vm => vm.TopicName, map => map.MapFrom(m => m.Topic.Name))
                .ForMember(vm => vm.Standard, map => map.MapFrom(m => m.Topic.Mapping.Standard.Name))
                .ForMember(vm => vm.Subject, map => map.MapFrom(m => m.Topic.Mapping.Subject.Name))

                .ForMember(vm => vm.StandardID, map => map.MapFrom(m => m.Topic.Mapping.Standard.ID))
                .ForMember(vm => vm.SubjectID, map => map.MapFrom(m => m.Topic.Mapping.Subject.ID))
                .ForMember(vm => vm.MappingID, map => map.MapFrom(m => m.Topic.Mapping.ID))

                .ForMember(vm => vm.DifficultyLevel, map => map.MapFrom(m => m.DifficultyLevel))
                .ForMember(vm => vm.Hint, map => map.MapFrom(m => m.Hint))
                .ForMember(vm => vm.Objective, map => map.MapFrom(m => m.Objective))
                .ForMember(vm => vm.IsLock, map => map.MapFrom(m => m.IsLock))
                .ForMember(vm => vm.LockedBy, map => map.MapFrom(m => m.LockedBy))
                .ForMember(vm => vm.OnCreated, map => map.MapFrom(m => m.OnCreated))
                .ForMember(vm => vm.OnLocked, map => map.MapFrom(m => m.OnLocked))
                .ForMember(vm => vm.OnUpdated, map => map.MapFrom(m => m.OnUpdated));

            Mapper.CreateMap<ChoiceAnswer, AnswerChoiceViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.ChoiceId, map => map.MapFrom(m => m.ChoiceId))
                .ForMember(vm => vm.DisplayType, map => map.MapFrom(m => m.DisplayType))
                .ForMember(vm => vm.IsAnswer, map => map.MapFrom(m => m.IsAnswer))
                .ForMember(vm => vm.PointsPerChoice, map => map.MapFrom(m => m.PointsPerChoice))
                .ForMember(vm => vm.QuestionId, map => map.MapFrom(m => m.QuestionId))
                .ForMember(vm => vm.Text, map => map.MapFrom(m => m.Text));

            Mapper.CreateMap<DescriptiveAnswer, AnswerDescriptiveViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.Keywords, map => map.MapFrom(m => m.Keywords))
                .ForMember(vm => vm.NegativePoints, map => map.MapFrom(m => m.NegativePoints))
                .ForMember(vm => vm.PositivePoints, map => map.MapFrom(m => m.PositivePoints))
                .ForMember(vm => vm.IsAnswer, map => map.MapFrom(m => m.IsAnswer))
                .ForMember(vm => vm.QuestionId, map => map.MapFrom(m => m.QuestionId));

            Mapper.CreateMap<MatchingAnswer, AnswerMatchPairViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.ChoiceA, map => map.MapFrom(m => m.ChoiceA))
                .ForMember(vm => vm.ChoiceB, map => map.MapFrom(m => m.ChoiceB))
                .ForMember(vm => vm.ChoiceId, map => map.MapFrom(m => m.ChoiceId))
                .ForMember(vm => vm.QuestionId, map => map.MapFrom(m => m.QuestionId));

            Mapper.CreateMap<StandardSubjectMapping, MappingViewModel>()
                .ForMember(vm => vm.Subject, map => map.MapFrom(m => m.Subject.Name))
                .ForMember(vm => vm.SubjectID, map => map.MapFrom(m => m.Subject.ID))
                .ForMember(vm => vm.Standard, map => map.MapFrom(m => m.Standard.Name))
                .ForMember(vm => vm.Division, map => map.MapFrom(m => m.Standard.Division))
                .ForMember(vm => vm.StandardID, map => map.MapFrom(m => m.Standard.ID))
                .ForMember(vm => vm.IsActive, map => map.MapFrom(m => m.IsActive))
                 .ForMember(vm => vm.TopicCount, map => map.MapFrom(m => m.Topic != null ? m.Topic.Count : 0))
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID));

            Mapper.CreateMap<Student, StudentViewModel>()
                .ForMember(vm => vm.Standard, map => map.MapFrom(m => m.Standard.Name))
                .ForMember(vm => vm.StandardId, map => map.MapFrom(m => m.Standard.ID));

            Mapper.CreateMap<User, RegistrationViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                //.ForMember(vm => vm.RoleID, map => map.MapFrom(m =>(m.UserRoles.ElementAt(0).RoleId.ToString()))
                .ForMember(vm => vm.Email, map => map.MapFrom(m => m.Email))
                .ForMember(vm => vm.Username, map => map.MapFrom(m => m.Username));

            Mapper.CreateMap<Test, TestViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
                .ForMember(vm => vm.Text, map => map.MapFrom(m => m.Text))
                .ForMember(vm => vm.Description, map => map.MapFrom(m => m.Description))
                .ForMember(vm => vm.Objective, map => map.MapFrom(m => m.Objective))
                .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Status))
                .ForMember(vm => vm.NegativeMarks, map => map.MapFrom(m => m.NegativeMarks))
                .ForMember(vm => vm.PointPerQuestion, map => map.MapFrom(m => m.PointsPerQuestion))
                .ForMember(vm => vm.TotalMarks, map => map.MapFrom(m => m.TotalMarks))
                .ForMember(vm => vm.PassingMarks, map => map.MapFrom(m => m.PassingMarks))
                .ForMember(vm => vm.ScoringMode, map => map.MapFrom(m => m.ScoringMode))
                .ForMember(vm => vm.DifficultyLevel, map => map.MapFrom(m => m.DifficultyLevel))
                .ForMember(vm => vm.PoolSequence, map => map.MapFrom(m => m.PoolSequence))
                .ForMember(vm => vm.PoolCount, map => map.MapFrom(m => m.Pools != null ? m.Pools.Count : 0))
                .ForMember(vm => vm.ShowHint, map => map.MapFrom(m => m.ShowHint))
                .ForMember(vm => vm.Lock, map => map.MapFrom(m => m.Lock))
                .ForMember(vm => vm.LockedBy, map => map.MapFrom(m => m.LockedBy))
                .ForMember(vm => vm.OnLocked, map => map.MapFrom(m => m.OnLocked))
                .ForMember(vm => vm.OnCreated, map => map.MapFrom(m => m.OnCreated))
                .ForMember(vm => vm.OnUpdated, map => map.MapFrom(m => m.OnUpdated));

            Mapper.CreateMap<Pool, PoolViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.TestID, map => map.MapFrom(m => m.TestID))
                .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
                .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Status))
                .ForMember(vm => vm.IsMandatoryToPass, map => map.MapFrom(m => m.IsMandatoryToPass))
                .ForMember(vm => vm.NoOfQuestionsOutOf, map => map.MapFrom(m => m.NoOfQuestionsOutOf))
                .ForMember(vm => vm.PassingScore, map => map.MapFrom(m => m.PassingScore))
                .ForMember(vm => vm.NegativeMarks, map => map.MapFrom(m => m.NegativeMarks))
                .ForMember(vm => vm.PoolTotalMarks, map => map.MapFrom(m => m.PoolTotalMarks))
                .ForMember(vm => vm.RandomizeChoice, map => map.MapFrom(m => m.RandomizeChoice))
                .ForMember(vm => vm.RandomizeQuestion, map => map.MapFrom(m => m.RandomizeQuestion))
                .ForMember(vm => vm.QuestionCount, map => map.MapFrom(m => m.PoolQuestionMapping != null ? m.PoolQuestionMapping.Count : 0))
                .ForMember(vm => vm.DifficultyLevel, map => map.MapFrom(m => m.DifficultyLevel));

            Mapper.CreateMap<PoolQuestionMapping, PoolQuestionViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.QuestionID, map => map.MapFrom(m => m.QuestionId))
                .ForMember(vm => vm.PoolID, map => map.MapFrom(m => m.PoolId))
                .ForMember(vm => vm.IsMandatory, map => map.MapFrom(m => m.IsMandatory));

            Mapper.CreateMap<Paper, PaperViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.TestID, map => map.MapFrom(m => m.TestID))
                .ForMember(vm => vm.TestName, map => map.MapFrom(m => m.TestName))
                .ForMember(vm => vm.NoOfSets, map => map.MapFrom(m => m.NoOfSets))
                .ForMember(vm => vm.IsFinalized, map => map.MapFrom(m => m.IsFinalized))
                .ForMember(vm => vm.OnCreated, map => map.MapFrom(m => m.OnCreated))
                .ForMember(vm => vm.CreatedBy, map => map.MapFrom(m => m.CreatedBy))
                .ForMember(vm => vm.Description, map => map.MapFrom(m => m.Description));

            Mapper.CreateMap<TempTestQuestion, TempTestQuestionViewModel>()
               .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
               .ForMember(vm => vm.TestID, map => map.MapFrom(m => m.TestID))
               .ForMember(vm => vm.TestName, map => map.MapFrom(m => m.TestName))
               .ForMember(vm => vm.PoolID, map => map.MapFrom(m => m.PoolID))
               .ForMember(vm => vm.PoolName, map => map.MapFrom(m => m.PoolName))
               .ForMember(vm => vm.QuestionID, map => map.MapFrom(m => m.QuestionID))
               .ForMember(vm => vm.QuestionType, map => map.MapFrom(m => m.QuestionType))
               .ForMember(vm => vm.QuestionText, map => map.MapFrom(m => m.QuestionText))
               .ForMember(vm => vm.SequenceNo, map => map.MapFrom(m => m.SequenceNo))
               .ForMember(vm => vm.PaperID, map => map.MapFrom(m => m.PaperID))
               .ForMember(vm => vm.TestSetNo, map => map.MapFrom(m => m.TestSetNo));

            Mapper.CreateMap<TempChoiceAnswer, TempAnswerChoiceViewModel>()
              .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
              .ForMember(vm => vm.ChoiceId, map => map.MapFrom(m => m.ChoiceId))
              .ForMember(vm => vm.DisplayType, map => map.MapFrom(m => m.DisplayType))
              .ForMember(vm => vm.IsAnswer, map => map.MapFrom(m => m.IsAnswer))
              .ForMember(vm => vm.PointsPerChoice, map => map.MapFrom(m => m.PointsPerChoice))
              .ForMember(vm => vm.QuestionId, map => map.MapFrom(m => m.QuestionId))
              .ForMember(vm => vm.Text, map => map.MapFrom(m => m.Text))
              .ForMember(vm => vm.TempTestQuestionID, map => map.MapFrom(m => m.TempTestQuestionID))
            .ForMember(vm => vm.PaperID, map => map.MapFrom(m => m.PaperID));

            Mapper.CreateMap<TempDescriptiveAnswer, TempAnswerDescriptiveViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.Keywords, map => map.MapFrom(m => m.Keywords))
                .ForMember(vm => vm.NegativePoints, map => map.MapFrom(m => m.NegativePoints))
                .ForMember(vm => vm.PositivePoints, map => map.MapFrom(m => m.PositivePoints))
                .ForMember(vm => vm.IsAnswer, map => map.MapFrom(m => m.IsAnswer))
                .ForMember(vm => vm.QuestionId, map => map.MapFrom(m => m.QuestionId))
                .ForMember(vm => vm.TempTestQuestionID, map => map.MapFrom(m => m.TempTestQuestionID))
                .ForMember(vm => vm.PaperID, map => map.MapFrom(m => m.PaperID));

            Mapper.CreateMap<TempMatchingAnswer, TempAnswerMatchPairViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.ChoiceA, map => map.MapFrom(m => m.ChoiceA))
                .ForMember(vm => vm.ChoiceB, map => map.MapFrom(m => m.ChoiceB))
                .ForMember(vm => vm.DisplayB, map => map.MapFrom(m => m.DisplayB))
                .ForMember(vm => vm.ChoiceId, map => map.MapFrom(m => m.ChoiceId))
                .ForMember(vm => vm.QuestionId, map => map.MapFrom(m => m.QuestionId))
                .ForMember(vm => vm.TempTestQuestionID, map => map.MapFrom(m => m.TempTestQuestionID))
                .ForMember(vm => vm.PaperID, map => map.MapFrom(m => m.PaperID));


            Mapper.CreateMap<TestQuestion, TestQuestionViewModel>()
               .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
               .ForMember(vm => vm.TestID, map => map.MapFrom(m => m.TestID))
               .ForMember(vm => vm.TestName, map => map.MapFrom(m => m.TestName))
               .ForMember(vm => vm.PoolID, map => map.MapFrom(m => m.PoolID))
               .ForMember(vm => vm.PoolName, map => map.MapFrom(m => m.PoolName))
               .ForMember(vm => vm.QuestionID, map => map.MapFrom(m => m.QuestionID))
               .ForMember(vm => vm.QuestionText, map => map.MapFrom(m => m.QuestionText))
               .ForMember(vm => vm.SequenceNo, map => map.MapFrom(m => m.SequenceNo))
               .ForMember(vm => vm.PaperID, map => map.MapFrom(m => m.PaperID))
               .ForMember(vm => vm.TestSetNo, map => map.MapFrom(m => m.TestSetNo));

            Mapper.CreateMap<FinalChoiceAnswer, FinalAnswerChoiceViewModel>()
              .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
              .ForMember(vm => vm.ChoiceId, map => map.MapFrom(m => m.ChoiceId))
              .ForMember(vm => vm.DisplayType, map => map.MapFrom(m => m.DisplayType))
              .ForMember(vm => vm.IsAnswer, map => map.MapFrom(m => m.IsAnswer))
              .ForMember(vm => vm.PointsPerChoice, map => map.MapFrom(m => m.PointsPerChoice))
              .ForMember(vm => vm.QuestionId, map => map.MapFrom(m => m.QuestionId))
              .ForMember(vm => vm.Text, map => map.MapFrom(m => m.Text))
              .ForMember(vm => vm.TestQuestionID, map => map.MapFrom(m => m.TestQuestionID))
            .ForMember(vm => vm.PaperID, map => map.MapFrom(m => m.PaperID));

            Mapper.CreateMap<FinalDescriptiveAnswer, FinalAnswerDescriptiveViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.Keywords, map => map.MapFrom(m => m.Keywords))
                .ForMember(vm => vm.NegativePoints, map => map.MapFrom(m => m.NegativePoints))
                .ForMember(vm => vm.PositivePoints, map => map.MapFrom(m => m.PositivePoints))
                .ForMember(vm => vm.IsAnswer, map => map.MapFrom(m => m.IsAnswer))
                .ForMember(vm => vm.QuestionId, map => map.MapFrom(m => m.QuestionId))
                .ForMember(vm => vm.TestQuestionID, map => map.MapFrom(m => m.TestQuestionID))
                .ForMember(vm => vm.PaperID, map => map.MapFrom(m => m.PaperID));

            Mapper.CreateMap<FinalMatchingAnswer, FinalAnswerMatchPairViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.ChoiceA, map => map.MapFrom(m => m.ChoiceA))
                .ForMember(vm => vm.ChoiceB, map => map.MapFrom(m => m.ChoiceB))
                .ForMember(vm => vm.DisplayB, map => map.MapFrom(m => m.DisplayB))
                .ForMember(vm => vm.ChoiceId, map => map.MapFrom(m => m.ChoiceId))
                .ForMember(vm => vm.QuestionId, map => map.MapFrom(m => m.QuestionId))
                .ForMember(vm => vm.TestQuestionID, map => map.MapFrom(m => m.TestQuestionID))
                .ForMember(vm => vm.PaperID, map => map.MapFrom(m => m.PaperID));

            Mapper.CreateMap<Role, RoleViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
                .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
                .ForMember(vm => vm.UserCount, map => map.MapFrom(m => m.RoleMapping != null ? m.RoleMapping.Count : 0));

            Mapper.CreateMap<Permission, PermissionViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.ID))
                .ForMember(vm => vm.FormID, map => map.MapFrom(m => m.Form.FormID))
                .ForMember(vm => vm.FormName, map => map.MapFrom(m => m.Form.Name))
                .ForMember(vm => vm.RoleID, map => map.MapFrom(m => m.RoleID))
                .ForMember(vm => vm.Action, map => map.MapFrom(m => m.Action))
                .ForMember(vm => vm.IsPermission, map => map.MapFrom(m => m.IsPermission));

            Mapper.CreateMap<Form, FormViewModel>()
           .ForMember(vm => vm.FormID, map => map.MapFrom(m => m.FormID))
           .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name));

            
       
        }
    }
}