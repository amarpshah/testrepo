using Institute.BizComponent.Configurations;
using Institute.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Institute.BizComponent
{
    public class ExamContext : DbContext
    {
        public ExamContext()
            : base("Exam")
        {
            Database.SetInitializer<ExamContext>(null);
        }

        #region Entity Sets
        public IDbSet<User> UserSet { get; set; }
        public IDbSet<Role> RoleSet { get; set; }
        public IDbSet<UserRole> UserRoleSet { get; set; }
        public IDbSet<Error> ErrorSet { get; set; }
       
        public IDbSet<Student> StudentSet { get; set; }
        public IDbSet<Standard> StandardSet { get; set; }
        public IDbSet<Subject> SubjectSet { get; set; }
        public IDbSet<StandardSubjectMapping> StandardSubjectMappingSet { get; set; }
        public IDbSet<Topic> TopicSet { get; set; }
        public IDbSet<Question> QuestionSet { get; set; }
        public IDbSet<DescriptiveAnswer> DescriptiveAnswerSet { get; set; }
        public IDbSet<ChoiceAnswer> ChoiceAnswerSet { get; set; }
        public IDbSet<MatchingAnswer> MatchingAnswerSet { get; set; }

        public IDbSet<Test> TestSet { get; set; }
        public IDbSet<Pool> PoolSet { get; set; }
        public IDbSet<PoolQuestionMapping> PoolQuestionSet { get; set; }
        public IDbSet<Paper> PaperSet { get; set; }
        public IDbSet<TempTestQuestion> TempTestQuestionSet { get; set; }
        public IDbSet<TempDescriptiveAnswer> TempDescriptiveAnswerSet { get; set; }
        public IDbSet<TempChoiceAnswer> TempChoiceAnswerSet { get; set; }
        public IDbSet<TempMatchingAnswer> TempMatchingAnswerSet { get; set; }
        public IDbSet<TestQuestion> TestQuestionSet { get; set; }
        public IDbSet<FinalDescriptiveAnswer> FinalDescriptiveAnswerSet { get; set; }
        public IDbSet<FinalChoiceAnswer> FinalChoiceAnswerSet { get; set; }
        public IDbSet<FinalMatchingAnswer> FinalMatchingAnswerSet { get; set; }
        public IDbSet<Permission> PermissionSet { get; set; }
        public IDbSet<Form> FormSet { get; set; }
        
        #endregion

        public virtual void Commit()
        {
            base.SaveChanges();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new UserRoleConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
        }
    }
}
