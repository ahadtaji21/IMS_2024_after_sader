using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using jrs.Models;
using ScaffoldModels.Models;

namespace jrs.DBContexts
{
    public partial class GeneralContext : DbContext
    {
        public GeneralContext()
        {
        }

        public GeneralContext(DbContextOptions<GeneralContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ImsLookup> ImsLookup { get; set; }
        public virtual DbSet<ImsLookupType> ImsLookupType { get; set; }
        public virtual DbSet<ImsUser> ImsUser { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<HrOffice> HrOffices { get; set; }
        public virtual DbSet<HrOfficeRelation> HrOfficeRelations { get; set; }
        public virtual DbSet<ImsStatus> Status { get; set; }
        public virtual DbSet<ImsTemplate> ImsTemplate { get; set; }
        public virtual DbSet<ImsTemplateParams> ImsTemplateParams { get; set; }
        public virtual DbSet<ImsTemplateType> ImsTemplateType { get; set; }
        public virtual DbSet<ImsTemplateOffice> ImsTemplateOffice { get; set; }
        public virtual DbSet<ImsQuestionnaire> ImsQuestionnaires { get; set; }
        public virtual DbSet<ImsQuestion> ImsQuestions { get; set; }
        public virtual DbSet<ImsQuestionnaireQuestion> ImsQuestionnaireQuestions { get; set; }
        public virtual DbSet<ImsQuestionnaireOffice> ImsQuestionnaireOffices { get; set; }
        public virtual DbSet<ImsQuestionnaireInstance> ImsQuestionnaireInstance { get; set; }
        public virtual DbSet<ImsAnswerInstance> ImsAnswerInstance { get; set; }
        public virtual DbSet<ImsQuestionAnswerOptions> ImsQuestionAnswerOptions { get; set; }
        public virtual DbSet<ImsQuestionnaireTab> ImsQuestionnaireTab { get; set; }
        public virtual DbSet<ImsFormFieldType> ImsFormFieldType { get; set; }
        // Sfip datasets
        public virtual DbSet<SfipPlan> SfipPlans { get; set; }
		public virtual DbSet<SfipPriority> SfipPriorities { get; set; }
		public virtual DbSet<SfipPrioritySet> SfipPrioritySets { get; set; }
		public virtual DbSet<SfipGoal> SfipGoals { get; set; }
		public virtual DbSet<SfipObjective> SfipObjectives { get; set; }
		public virtual DbSet<SfipIndicator> SfipIndicators { get; set; }
		public virtual DbSet<SfipIndicatorOffice> SfipIndicatorOffices { get; set; }
		public virtual DbSet<SfipActivity> SfipActivities { get; set; }
		public virtual DbSet<SfipActivitySchedule> SfipActivitySchedules { get; set; } = null!;
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<MaritalStatus> MaritalStatuses { get; set; }
        public virtual DbSet<IndividualDocumentType> IndividualDocumentTypes { get; set; }
        public virtual DbSet<EducationalLevel> EducationalLevel { get; set; }

        

        public virtual DbSet<HouseholdDocumentType> HouseholdDocumentTypes { get; set; }
        public virtual DbSet<DocumentType> DocumentTypes{ get; set; }
        public virtual DbSet<CategoryIntervention> CategoryInterventions { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<ContactType> ContactTypes  { get; set; }
        public virtual DbSet<IndividualWizardClass> IndividualWizard  { get; set; }

        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=JRSDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImsLookup>(entity =>
            {
                entity.ToTable("IMS_LOOKUP");

                entity.Property(e => e.ImsLookupId).HasColumnName("IMS_LOOKUP_ID");

                entity.Property(e => e.ImsLookupCode)
                    .HasColumnName("IMS_LOOKUP_CODE")
                    .HasMaxLength(50);

                entity.Property(e => e.ImsLookupLookupTypeId).HasColumnName("IMS_LOOKUP_LOOKUP_TYPE_ID");

                entity.Property(e => e.ImsLookupValue)
                    .IsRequired()
                    .HasColumnName("IMS_LOOKUP_VALUE");

                entity.HasOne(d => d.ImsLookupLookupType)
                    .WithMany()
                    .HasForeignKey(d => d.ImsLookupLookupTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__IMS_LOOKU__IMS_L__29E1370A");
            });

            modelBuilder.Entity<ImsLookupType>(entity =>
            {
                entity.ToTable("IMS_LOOKUP_TYPE");

                entity.Property(e => e.ImsLookupTypeId).HasColumnName("IMS_LOOKUP_TYPE_ID");

                entity.Property(e => e.ImsLookupTypeCode)
                    .IsRequired()
                    .HasColumnName("IMS_LOOKUP_TYPE_CODE")
                    .HasMaxLength(25);

                entity.Property(e => e.ImsLookupTypeDescr)
                    .IsRequired()
                    .HasColumnName("IMS_LOOKUP_TYPE_DESCR")
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<ImsUser>(entity =>
            {
                entity.HasKey(e => e.ImsUserUid)
                    .HasName("PK__IMS_USER__AB0D98D42C3DCF53");

                entity.ToTable("IMS_USER");

                entity.Property(e => e.ImsUserUid)
                    .HasColumnName("IMS_USER_UID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ImsUserCreationDate)
                    .HasColumnName("IMS_USER_CREATION_DATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImsUserLocale)
                    .IsRequired()
                    .HasColumnName("IMS_USER_LOCALE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('en')");

                entity.Property(e => e.ImsUserPassword)
                    .HasColumnName("IMS_USER_PASSWORD")
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.ImsUserRefreshToken)
                    .HasColumnName("IMS_USER_REFRESH_TOKEN")
                    .HasMaxLength(100);

                entity.Property(e => e.ImsUserUsername)
                    .HasColumnName("IMS_USER_USERNAME")
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.ImsUserEmail)
                    .HasColumnName("IMS_USER_EMAIL")
                    .HasMaxLength(320)
                    .IsUnicode(false);

                entity.Property(e => e.accountIdentifier)
                    .HasColumnName("IMS_USER_ACCOUNT_UID")
                    .IsUnicode(false);

                entity.Property(e => e.ImsUserIsDeleted)
                    .HasColumnName("IMS_USER_IS_DELETED")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("MENU");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Descr)
                    .IsRequired()
                    .HasColumnName("DESCR")
                    .HasMaxLength(100);

                entity.Property(e => e.IconCode)
                    .HasColumnName("ICON_CODE")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LabelKey)
                    .IsRequired()
                    .HasColumnName("LABEL_KEY")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ParentMenuId)
                    .HasColumnName("PARENT_MENU_ID");

                entity.HasMany(p => p.InverseParentMenu)
                    .WithOne()
                    .HasForeignKey(d => d.ParentMenuId)
                    .HasConstraintName("FK__MENU__PARENT_MEN__3FD07829");

                entity.Property(e => e.OrdinalPosition)
                    .HasColumnName("ORDINAL_POSITION");

                entity.Property(e => e.ModuleId)
                    .HasColumnName("MODULE_ID");

                entity.Property(e => e.ModuleName)
                    .HasColumnName("MODULE_NAME")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.ModuleCode)
                    .HasColumnName("MODULE_CODE")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.isHidden)
                    .HasColumnName("IS_HIDDEN");
            });

            modelBuilder.Entity<HrOffice>(entity =>
            {
                entity.ToTable("HR_OFFICE");

                entity.Property(e => e.HrOfficeId).HasColumnName("HR_OFFICE_ID");

                entity.Property(e => e.HrOfficeCode)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HR_OFFICE_CODE");

                entity.Property(e => e.HrOfficeCountryAdminAreaId).HasColumnName("HR_OFFICE_COUNTRY_ADMIN_AREA_ID");

                entity.Property(e => e.HrOfficeLegalAddressId).HasColumnName("HR_OFFICE_LEGAL_ADDRESS_ID");

                entity.Property(e => e.HrOfficeLegalLanguageId).HasColumnName("HR_OFFICE_LEGAL_LANGUAGE_ID");

                entity.Property(e => e.HrOfficeLegalName)
                    .HasMaxLength(250)
                    .HasColumnName("HR_OFFICE_LEGAL_NAME");

                entity.Property(e => e.HrOfficeLegalNameNative)
                    .HasMaxLength(250)
                    .HasColumnName("HR_OFFICE_LEGAL_NAME_NATIVE");

                entity.Property(e => e.HrOfficeLegalRepName)
                    .HasMaxLength(500)
                    .HasColumnName("HR_OFFICE_LEGAL_REP_NAME");

                entity.Property(e => e.HrOfficeNativeLanguageId).HasColumnName("HR_OFFICE_NATIVE_LANGUAGE_ID");

                entity.Property(e => e.HrOfficeRegisteredName)
                    .HasMaxLength(250)
                    .HasColumnName("HR_OFFICE_REGISTERED_NAME");

                entity.Property(e => e.HrOfficeSignatoryName)
                    .HasMaxLength(500)
                    .HasColumnName("HR_OFFICE_SIGNATORY_NAME");

                entity.Property(e => e.IsCountry).HasColumnName("IS_COUNTRY");

                entity.Property(e => e.IsInternational).HasColumnName("IS_INTERNATIONAL");

                entity.Property(e => e.IsRegional).HasColumnName("IS_REGIONAL");


                // entity.HasOne(d => d.HrOfficeLegalAddress)
                //     .WithMany(p => p.HrOffices)
                //     .HasForeignKey(d => d.HrOfficeLegalAddressId)
                //     .HasConstraintName("FK__HR_OFFICE__HR_OF__16CE6296");

                // entity.HasOne(d => d.HrOfficeLegalLanguage)
                //     .WithMany(p => p.HrOfficeHrOfficeLegalLanguages)
                //     .HasForeignKey(d => d.HrOfficeLegalLanguageId)
                //     .HasConstraintName("FK__HR_OFFICE__HR_OF__17C286CF");

                // entity.HasOne(d => d.HrOfficeNativeLanguage)
                //     .WithMany(p => p.HrOfficeHrOfficeNativeLanguages)
                //     .HasForeignKey(d => d.HrOfficeNativeLanguageId)
                //     .HasConstraintName("FK__HR_OFFICE__HR_OF__18B6AB08");
            });

            modelBuilder.Entity<HrOfficeRelation>(entity =>
            {
                entity.ToTable("HR_OFFICE_RELATIONS");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ChildOfficeId).HasColumnName("CHILD_OFFICE_ID");

                entity.Property(e => e.DateEnd)
                    .HasColumnType("date")
                    .HasColumnName("DATE_END");

                entity.Property(e => e.DateStart)
                    .HasColumnType("date")
                    .HasColumnName("DATE_START");

                entity.Property(e => e.ParentOfficeId).HasColumnName("PARENT_OFFICE_ID");

                entity.HasOne(d => d.ChildOffice)
                    .WithMany(p => p.HrOfficeRelationChildOffices)
                    .HasForeignKey(d => d.ChildOfficeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HR_OFFICE__CHILD__6F9F86DC");

                entity.HasOne(d => d.ParentOffice)
                    .WithMany(p => p.HrOfficeRelationParentOffices)
                    .HasForeignKey(d => d.ParentOfficeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HR_OFFICE__PAREN__6EAB62A3");
            });

            modelBuilder.Entity<ImsStatus>(entity =>
            {
                entity.ToTable("IMS_STATUS");
                entity.HasKey(e => e.imsStatusId);
                entity.Property(e => e.imsStatusId).HasColumnName("IMS_STATUS_ID").IsRequired();
                entity.Property(e => e.imsStatusName).HasColumnName("IMS_STATUS_NAME").IsRequired();
                entity.Property(e => e.ImsStatusDescr).HasColumnName("IMS_STATUS_DESCR").IsRequired();
                entity.Property(e => e.ImsStatusCode).HasColumnName("IMS_STATUS_CODE");
            });

            modelBuilder.Entity<ImsTemplate>(entity =>
            {
                entity.ToTable("IMS_TEMPLATE");

                entity.HasKey(e => e.TemplateId)
                    .HasName("PK__IMS_TEMP__BACD412FFD39FA32");

                entity.Property(e => e.TemplateId).HasColumnName("TEMPLATE_ID");

                entity.Property(e => e.TemplateTypeId)
                    .HasColumnName("TEMPLATE_TYPE_ID")
                    .IsRequired();

                entity.Property(e => e.TemplateCode)
                    .HasColumnName("TEMPLATE_CODE")
                    .HasMaxLength(128)
                    .IsRequired();

                entity.Property(e => e.TemplateText)
                    .HasColumnName("TEMPLATE_TEXT")
                    .IsRequired();

                entity.HasOne(e => e.TemplateType)
                    .WithMany()
                    .HasForeignKey(e => e.TemplateTypeId)
                    .HasConstraintName("FK__IMS_TEMPL__TEMPL__5C629536");

                entity.HasMany(template => template.TemplateOfficeList)
                    .WithOne()
                    .HasForeignKey(to => to.TemplateId);
            });

            modelBuilder.Entity<ImsTemplateParams>(entity =>
            {
                entity.ToTable("IMS_TEMPLATE_PARAMS");

                entity.HasKey(e => e.TemplateParamId)
                    .HasName("PK__IMS_TEMP__A5C79E293B48F6E0");

                entity.Property(e => e.TemplateParamId).HasColumnName("TEMPLATE_PARAM_ID");

                entity.Property(e => e.TemplateTypeId)
                    .HasColumnName("TEMPLATE_TYPE_ID")
                    .IsRequired();

                entity.Property(e => e.TemplateParamDescr)
                    .HasColumnName("TEMPLATE_PARAM_DESCR")
                    .HasMaxLength(250)
                    .IsRequired();

                entity.Property(e => e.TemplateParamColumnName)
                    .HasColumnName("TEMPLATE_PRARAM_COLUMN_NAME")
                    .HasMaxLength(128)
                    .IsRequired();

                //entity.HasOne(e => e.TemplateType)
                //   .WithMany()
                //   .HasForeignKey(e => e.TemplateTypeId)
                //   .HasConstraintName("FK__IMS_TEMPL__TEMPL__13B2CA20");

                // // entity.HasOne(d => d.ImsLookupLookupType)
                // //     .WithMany()
                // //     .HasForeignKey(d => d.ImsLookupLookupTypeId)
                // //     .OnDelete(DeleteBehavior.ClientSetNull)
                // //     .HasConstraintName("FK__IMS_LOOKU__IMS_L__29E1370A");
            });

            modelBuilder.Entity<ImsTemplateType>(entity =>
            {
                entity.ToTable("IMS_TEMPLATE_TYPE");

                entity.HasKey(e => e.TemplateTypeId)
                    .HasName("PK__IMS_TEMP__DD8BD289BAECB170");

                entity.Property(e => e.TemplateTypeId).HasColumnName("TEMPLATE_TYPE_ID").IsRequired();
                entity.Property(e => e.TemplateTypeCode).HasColumnName("TEMPLATE_TYPE_CODE").IsRequired().HasMaxLength(128);
                entity.Property(e => e.TemplateTypeDescr).HasColumnName("TEMPLATE_TYPE_DESCR").IsRequired().HasMaxLength(250);
                entity.Property(e => e.TemplateDataQuery).HasColumnName("TEMPLATE_DATA_QUERY");
                entity.Property(e => e.SelectItemsDatasource).HasColumnName("SELECT_ITEMS_DATASOURCE");
                entity.Property(e => e.SelectItemKey).HasColumnName("SELECT_ITEM_KEY");
                entity.Property(e => e.SelectItemText).HasColumnName("SELECT_ITEM_TEXT");
                entity.Property(e => e.FieldTranslationKey).HasColumnName("FIELD_TRANSLATION_KEY");
                entity.Property(e => e.DefaultDataQueryCondition).HasColumnName("DEFAULT_DATA_QUERY_CONDITION");
                entity.HasMany(templateType => templateType.TemplateParams)
                    .WithOne()
                    .HasForeignKey(param => param.TemplateTypeId);
            });

            modelBuilder.Entity<ImsTemplateOffice>(entity =>
            {
                entity.ToTable("IMS_TEMPLATE_OFFICE");

                entity.HasKey(e => e.Id)
                    .HasName("PK__IMS_TEMP__3214EC27639F4D14");

                entity.Property(e => e.Id).HasColumnName("ID").IsRequired();
                entity.Property(e => e.TemplateId).HasColumnName("TEMPLATE_ID").IsRequired();
                entity.Property(e => e.OfficeId).HasColumnName("OFFICE_ID").IsRequired();
                // entity.Property(e => e.DateFrom).HasColumnName("DATE_FROM");
                // entity.Property(e => e.DateTo).HasColumnName("DATE_TO");
                entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED").HasDefaultValue(false);
            });

            modelBuilder.Entity<ImsQuestionnaire>(entity =>
            {
                entity.ToTable("IMS_QUESTIONNAIRE");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("ID").IsRequired();
                entity.Property(e => e.code).HasColumnName("CODE").IsRequired();
                entity.Property(e => e.title).HasColumnName("TITLE").IsRequired();
                entity.Property(e => e.descr).HasColumnName("DESCR").IsRequired();
                entity.Property(e => e.includeBeneficiarySelection).HasColumnName("INCLUDE_BENEFICIARY_SELECTION");
                entity.Property(e => e.includeHouseholdSelection).HasColumnName("INCLUDE_HOUSEHOLD_SELECTION");
                entity.Property(e => e.includeSettlementSelection).HasColumnName("INCLUDE_SETTLEMENT_SELECTION");
                entity.Property(e => e.includeProjectSelection).HasColumnName("INCLUDE_PROJECT_SELECTION");

                entity.HasMany(e => e.QuestionnaireQuestionList)
                    .WithOne()
                    .HasForeignKey(qq => qq.questionnaireId);

                entity.HasMany(e => e.QuestionnaireOfficeList)
                    .WithOne()
                    .HasForeignKey(qo => qo.questionnaireId);

                entity.HasMany(e => e.QuestionnaireTabList)
                    .WithOne()
                    .HasForeignKey(qt => qt.questionnaireId);
            });

            modelBuilder.Entity<ImsQuestionnaireOffice>(entity =>
            {
                entity.ToTable("IMS_QUESTIONNAIRE_OFFICE");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("ID").IsRequired();
                entity.Property(e => e.questionnaireId).HasColumnName("QUESTIONNAIRE_ID").IsRequired();
                entity.Property(e => e.officeId).HasColumnName("OFFICE_ID").IsRequired();
            });

            modelBuilder.Entity<ImsQuestion>(entity =>
            {
                entity.ToTable("IMS_QUESTION");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("ID").IsRequired();
                entity.Property(e => e.questionText).HasColumnName("QUESTION_TEXT").IsRequired();
                entity.Property(e => e.questionHint).HasColumnName("QUESTION_HINT");
                entity.Property(e => e.questionType).HasColumnName("QUESTION_TYPE").IsRequired();

                entity.HasOne(e => e.FormFieldType)
                    .WithMany()
                    .HasForeignKey(e => e.questionType);
                entity.HasMany(e => e.QuestionnaireQuestionList)
                    .WithOne()
                    .HasForeignKey(qq => qq.questionId);
                entity.HasMany(e => e.QuestionAnswerOptionsList)
                    .WithOne()
                    .HasForeignKey(qao => qao.questionId);
            });

            modelBuilder.Entity<ImsQuestionnaireQuestion>(entity =>
            {
                entity.ToTable("IMS_QUESTIONNAIRE_QUESTION");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("ID").IsRequired();
                entity.Property(e => e.questionnaireId).HasColumnName("QUESTIONNAIRE_ID").IsRequired();
                entity.Property(e => e.questionId).HasColumnName("QUESTION_ID").IsRequired();
                entity.Property(e => e.ordinalPosition).HasColumnName("ORDINAL_POSITION").IsRequired();
                entity.Property(e => e.tabId).HasColumnName("TAB_ID");
                entity.Property(e => e.isRequired).HasColumnName("IS_REQUIRED").IsRequired();

                entity.HasOne(e => e.Questionnaire)
                    .WithMany(questionnaire => questionnaire.QuestionnaireQuestionList)
                    .HasForeignKey(e => e.questionnaireId);

                entity.HasOne(e => e.Question)
                    .WithMany(question => question.QuestionnaireQuestionList)
                    .HasForeignKey(e => e.questionId);

                // entity.HasOne(e => e.QuestionnaireTab)
                //     .WithMany(questionnaireTab => questionnaireTab.QuestionnaireQuestionList)
                //     .HasForeignKey(e => e.tabId);
                entity.HasOne(e => e.QuestionnaireTab)
                    .WithMany()
                    .HasForeignKey(e => e.tabId);

            });

            modelBuilder.Entity<ImsQuestionnaireInstance>(entity =>
            {
                entity.ToTable("IMS_QUESTIONNAIRE_INSTANCE");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("ID").IsRequired();
                entity.Property(e => e.questionnaireId).HasColumnName("QUESTIONNAIRE_ID").IsRequired();
                entity.Property(e => e.fillInUser).HasColumnName("FILL_IN_USER");
                entity.Property(e => e.fillInHousehold).HasColumnName("FILL_IN_HOUSEHOLD");
                entity.Property(e => e.fillInAssistant).HasColumnName("FILL_IN_ASSISTANT").IsRequired();
                entity.Property(e => e.fillInDate).HasColumnName("FILL_IN_DATE").IsRequired();
                entity.Property(e => e.reviewUser).HasColumnName("REVIEW_USER");
                entity.Property(e => e.reviewDate).HasColumnName("REVIEW_DATE");
                entity.Property(e => e.statusId).HasColumnName("STATUS_ID").IsRequired();
                entity.Property(e => e.relevantOfficeId).HasColumnName("RELEVANT_OFFICE_ID").IsRequired();
                entity.Property(e => e.selectedSettlement).HasColumnName("SELECTED_SETTLEMENT");
                entity.Property(e => e.selectedProject).HasColumnName("SELECTED_PROJECT");
                entity.HasMany(e => e.AnswerInstanceList)
                    .WithOne()
                    .HasForeignKey(a => a.questionnaireInstanceId);
                entity.HasOne(e => e.Questionnaire)
                    .WithMany()
                    .HasForeignKey(e => e.questionnaireId);
                entity.HasOne(e => e.Status)
                    .WithMany()
                    .HasForeignKey(e => e.statusId);
            });

            modelBuilder.Entity<ImsAnswerInstance>(entity =>
            {
                entity.ToTable("IMS_ANSWER_INSTANCE");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("ID");
                entity.Property(e => e.questionnaireInstanceId).HasColumnName("QUESTIONNAIRE_INSTANCE_ID");
                entity.Property(e => e.questionId).HasColumnName("QUESTION_ID");
                entity.Property(e => e.answerText).HasColumnName("ANSWER_TEXT");
                entity.Property(e => e.answerDate).HasColumnName("ANSWER_DATE");
                entity.Property(e => e.answerCheckbox).HasColumnName("ANSWER_CHECKBOX");
                entity.Property(e => e.answerNumber).HasColumnName("ANSWER_NUMBER").HasColumnType("decimal(18,2)");
                entity.Property(e => e.answerOptionId).HasColumnName("ANSWER_OPTION_ID");
                entity.Property(e => e.answerMatrixOptionId).HasColumnName("ANSWER_MATRIX_OPTION_ID");
                entity.Property(e => e.fillInUser).HasColumnName("FILL_IN_USER");
                entity.Property(e => e.fillInHousehold).HasColumnName("FILL_IN_HOUSEHOLD");
                entity.HasOne(e => e.Question)
                    .WithMany()
                    .HasForeignKey(e => e.questionId);
                entity.HasOne(e => e.QuestionAnswerOption)
                    .WithMany()
                    .HasForeignKey(e => e.answerOptionId);
                
            });

            modelBuilder.Entity<ImsQuestionAnswerOptions>(entity =>
            {
                entity.ToTable("IMS_QUESTION_ANSWER_OPTIONS");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("ID").IsRequired();
                entity.Property(e => e.questionId).HasColumnName("QUESTION_ID").IsRequired();
                entity.Property(e => e.answerText).HasColumnName("ANSWER_TEXT").IsRequired();
                entity.Property(e => e.isCorrectAnswer).HasColumnName("IS_CORRECT_ANSWER");
                entity.Property(e => e.score).HasColumnName("SCORE");
                // entity.HasOne(e => e.Question)
                //     .WithMany()
                //     .HasForeignKey(e => e.questionId);
            });

            modelBuilder.Entity<ImsQuestionnaireTab>(entity =>
            {
                entity.ToTable("IMS_QUESTIONNAIRE_TAB");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("ID").IsRequired();
                entity.Property(e => e.code).HasColumnName("CODE").IsRequired();
                entity.Property(e => e.descr).HasColumnName("DESCR").IsRequired();
                entity.Property(e => e.questionnaireId).HasColumnName("QUESTIONNAIRE_ID").IsRequired();
                entity.Property(e => e.ordinalPosition).HasColumnName("ORDINAL_POSITION");

                // entity.HasMany(qt => qt.QuestionnaireQuestionList)
                //     .WithOne()
                //     .HasForeignKey(qq => qq.tabId);
                entity.HasOne(qt => qt.Questionnaire)
                    .WithMany(q => q.QuestionnaireTabList)
                    .HasForeignKey(qt => qt.questionnaireId);
            });

            modelBuilder.Entity<ImsFormFieldType>(entity =>
            {
                entity.ToTable("IMS_FORM_FIELD_TYPE");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("ID").IsRequired();
                entity.Property(e => e.code).HasColumnName("CODE").IsRequired();
                entity.Property(e => e.descr).HasColumnName("DESCR").IsRequired();
                entity.Property(e => e.isAvailableForQuestionnaire).HasColumnName("IS_AVAILBALE_FOR_QUESTIONNAIRE").IsRequired();
            });

            
            // Sfip datasets
            modelBuilder.Entity<SfipPlan>(entity =>
			{
				entity.ToTable("SFIP_STRATEGIC_FRAMEWORK_IMPLEMENTATION_PLAN");
				entity.HasKey(e => e.id);
				entity.Property(e => e.id).HasColumnName("ID").IsRequired();
				entity.Property(e => e.code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
				entity.Property(e => e.startYear).HasColumnName("START_YEAR").IsRequired();
				entity.Property(e => e.endYear).HasColumnName("END_YEAR").IsRequired();
				entity.Property(e => e.isDeleted).HasColumnName("IS_DELETED").IsRequired();

				entity.HasMany(e => e.IndicarotList)
					.WithOne()
					.HasForeignKey(indicator => indicator.sfipId);

				entity.HasMany(e => e.IndicarotList)
					.WithOne();
			});

			modelBuilder.Entity<SfipPrioritySet>(entity =>
			{
				entity.ToTable("SFIP_PRIORITY_SET");
				entity.HasKey(e => e.id);
				entity.Property(e => e.id).HasColumnName("ID").IsRequired();
				entity.Property(e => e.code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
				entity.Property(e => e.name).HasColumnName("NAME").IsRequired().HasMaxLength(128);
				entity.Property(e => e.isDeleted).HasColumnName("IS_DELETED").IsRequired();

				entity.HasMany(e => e.PriorityList)
					.WithOne()
					.HasForeignKey(priority => priority.prioritySetId);

				// entity.HasOne(prioritySet => prioritySet.Plan)
				//     .WithMany(plan => plan.PrioritySets);

			});

			modelBuilder.Entity<SfipPriority>(entity =>
			{
				entity.ToTable("SFIP_PRIORITY");
				entity.HasKey(e => e.id);
				entity.Property(e => e.id).HasColumnName("ID").IsRequired();
				entity.Property(e => e.code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
				entity.Property(e => e.name).HasColumnName("NAME").IsRequired().HasMaxLength(128);
				entity.Property(e => e.formulation).HasColumnName("FORMULATION").IsRequired();
				entity.Property(e => e.prioritySetId).HasColumnName("PRIORITY_SET_ID").IsRequired();
				entity.Property(e => e.isDeleted).HasColumnName("IS_DELETED").IsRequired();

				// entity.HasOne(e => e.PrioritySet)
				// 	.WithMany()
				// 	.HasForeignKey(e => e.prioritySetId);
				entity.HasOne(priority => priority.PrioritySet)
					.WithMany(set => set.PriorityList)
					.HasForeignKey(priority => priority.prioritySetId);

				entity.HasMany(e => e.GoalList)
					.WithOne()
					.HasForeignKey(goal => goal.priorityId);
			});

			modelBuilder.Entity<SfipGoal>(entity =>
			{
				entity.ToTable("SFIP_GOAL");
				entity.HasKey(e => e.id);
				entity.Property(e => e.id).HasColumnName("ID").IsRequired();
				entity.Property(e => e.code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
				entity.Property(e => e.formulation).HasColumnName("FORMULATION").IsRequired();
				entity.Property(e => e.priorityId).HasColumnName("PRIORITY_ID").IsRequired();
				entity.Property(e => e.isDeleted).HasColumnName("IS_DELETED").IsRequired();

				entity.HasMany(e => e.ObjectiveList)
					.WithOne()
					.HasForeignKey(objective => objective.goalId);

				entity.HasOne(goal => goal.Priority)
					.WithMany(priority => priority.GoalList)
					.HasForeignKey(goal => goal.priorityId);
			});

			modelBuilder.Entity<SfipObjective>(entity =>
			{
				entity.ToTable("SFIP_OBJECTIVE");
				entity.HasKey(e => e.id);
				entity.Property(e => e.id).HasColumnName("ID").IsRequired();
				entity.Property(e => e.code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
				entity.Property(e => e.goalId).HasColumnName("GOAL_ID").IsRequired();
				entity.Property(e => e.isDeleted).HasColumnName("IS_DELETED").IsRequired();

				entity.HasMany(e => e.IndicatorList)
					.WithOne()
					.HasForeignKey(indicator => indicator.objectiveId);

				entity.HasOne(obj => obj.Goal)
					.WithMany(goal => goal.ObjectiveList)
					.HasForeignKey(obj => obj.goalId);
			});

			modelBuilder.Entity<SfipIndicator>(entity =>
			{
				entity.ToTable("SFIP_INDICATOR");
				entity.HasKey(e => e.id);
				entity.Property(e => e.id).HasColumnName("ID").IsRequired();
				entity.Property(e => e.code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
				entity.Property(e => e.formulation).HasColumnName("FORMULATION").IsRequired();
				entity.Property(e => e.sfipId).HasColumnName("SFIP_ID").IsRequired();
				entity.Property(e => e.objectiveId).HasColumnName("OBJECTIVE_ID").IsRequired();
				entity.Property(e => e.value1).HasColumnName("VALUE_1");
				entity.Property(e => e.value2).HasColumnName("VALUE_2");
				entity.Property(e => e.value3).HasColumnName("VALUE_3");
				entity.Property(e => e.value4).HasColumnName("VALUE_4");
				entity.Property(e => e.value5).HasColumnName("VALUE_5");
				entity.Property(e => e.isDeleted).HasColumnName("IS_DELETED").IsRequired();

				entity.HasMany(e => e.ActivityList)
					.WithOne()
					.HasForeignKey(activity => activity.indicatorId);

				entity.HasOne(ind => ind.Objective)
					.WithMany(obj => obj.IndicatorList)
					.HasForeignKey(ind => ind.objectiveId);

				entity.HasOne(ind => ind.Plan)
					.WithMany(plan => plan.IndicarotList)
					.HasForeignKey(ind => ind.sfipId);
			});

			modelBuilder.Entity<SfipIndicatorOffice>(entity =>
			{
				entity.ToTable("SFIP_INDICATOR_OFFICE");
				entity.HasKey(e => e.id);
				entity.Property(e => e.id).HasColumnName("ID").IsRequired();
				entity.Property(e => e.indicatorId).HasColumnName("INDICATOR_ID").IsRequired();
				entity.Property(e => e.officeId).HasColumnName("OFFICE_ID").IsRequired();

				entity.HasOne(indOff => indOff.Indicator)
					.WithMany(ind => ind.IndicatorOfficeList)
					.HasForeignKey(indOff => indOff.indicatorId);

				// entity.HasOne(indOff => indOff.Office)
				// 	.WithMany(off => off.IndicatorOfficeList)
				// 	.HasForeignKey(indOff => indOff.officeId);
			});

			modelBuilder.Entity<SfipActivity>(entity =>
			{
				entity.ToTable("SFIP_ACTIVITIE");
				entity.HasKey(e => e.id);
				entity.Property(e => e.id).HasColumnName("ID").IsRequired();
				entity.Property(e => e.code).HasColumnName("CODE").IsRequired().HasMaxLength(50);
				entity.Property(e => e.descr).HasColumnName("DESCR").IsRequired();
				entity.Property(e => e.indicatorId).HasColumnName("INDICATOR_ID").IsRequired();
				entity.Property(e => e.officeId).HasColumnName("OFFICE_ID").IsRequired();
				entity.Property(e => e.isDeleted).HasColumnName("IS_DELETED").IsRequired();

				entity.HasOne(act => act.Indicator)
					.WithMany(ind => ind.ActivityList)
					.HasForeignKey(act => act.indicatorId);
			});

			modelBuilder.Entity<SfipActivitySchedule>(entity =>
            {
                entity.ToTable("SFIP_ACTIVITY_SCHEDULE");
                entity.Property(e => e.Id).HasColumnName("ID");
                entity.Property(e => e.ActivityId).HasColumnName("ACTIVITY_ID");
                entity.Property(e => e.IsDeleted).HasColumnName("IS_DELETED").HasDefaultValueSql("((0))");
                entity.Property(e => e.ScheduleQuarterLookupId).HasColumnName("SCHEDULE_QUARTER_LOOKUP_ID");
                entity.Property(e => e.ScheduleYear).HasColumnName("SCHEDULE_YEAR");
                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.SfipActivitySchedules)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SFIP_ACTI__ACTIV__3D14D266");
				
				entity.HasOne(aSchedule => aSchedule.ScheduleQuarterLookup)
					.WithMany(l => l.SfipActivitySchedules)
					.HasForeignKey(aSchedule => aSchedule.ScheduleQuarterLookupId);

				// entity.HasOne(d => d.ScheduleQuarterLookup)
                //     .WithMany(p => p.SfipActivitySchedules)
                //     .HasForeignKey(d => d.ScheduleQuarterLookupId)
                //     .OnDelete(DeleteBehavior.ClientSetNull)
                //     .HasConstraintName("FK__SFIP_ACTI__SCHED__63A48FA2");
            });


            modelBuilder.Entity<Gender>(entity =>
            {
                entity.ToTable("Gender", "IMS");
                entity.HasKey(e => e.gender_id);
            });
            
          
            modelBuilder.Entity<MaritalStatus>(entity =>
            {
                entity.ToTable("MaritalStatus", "IMS");
                entity.HasKey(e => e.guid_marital_status_id);
            });

            modelBuilder.Entity<EducationalLevel>(entity =>
            {
                entity.ToTable("EducationalLevel", "IMS");
                entity.HasKey(e => e.guid_educational_level_id);
            });

            modelBuilder.Entity<HouseholdDocumentType>(entity =>
            {
                entity.ToTable("DocumentType", "IMS");
                entity.HasKey(e => e.guid_document_type_id);
            });

            modelBuilder.Entity<AddressPropertyType>(entity => {
                entity.HasKey(e => e.guid_address_property_type_id);
            });

            modelBuilder.Entity<CategoryIntervention>(entity =>
            {
                entity.HasKey(e => e.guid_nav_category_intervention_id);
            });
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.guid_city_id);
            });
            modelBuilder.Entity<ContactType>(entity =>
            {
                entity.HasKey(e => e.guid_contact_type_id);
            });
            modelBuilder.Entity<Continent>(entity =>
            {
                entity.HasKey(e => e.guid_continent_id);
            });
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.guid_country_id);
            });
            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => e.guid_state_id);
            });
            modelBuilder.Entity<DataSection>(entity =>
            {
                entity.HasKey(e => e.guid_data_section_id);
            });

            modelBuilder.Entity<DataSectionDetail>(entity =>
            {
                entity.HasKey(e => e.guid_data_section_detail_id);
            });
            modelBuilder.Entity<Distribution>(entity =>
            {
                entity.HasKey(e => e.guid_distribution_id);
            });
            modelBuilder.Entity<DistributionCommodityType>(entity =>
            {
                entity.HasKey(e => e.distribution_commodity_type_id);
            });
            modelBuilder.Entity<DistributionHousehold>(entity =>
            {
                entity.HasKey(e => e.guid_distribution_household_id);
            });
            modelBuilder.Entity<DistributionIndividual>(entity =>
            {
                entity.HasKey(e => e.guid_distribution_individual_id);
            });
            modelBuilder.Entity<DistributionType>(entity =>
            {
                entity.HasKey(e => e.guid_distribution_type);
            });
            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasKey(e => e.guid_document_type_id);
            });
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.guid_employee_id);
            });

            modelBuilder.Entity<Household>(entity =>
            {
                entity.HasKey(e => e.guid_household_id);
            });
            modelBuilder.Entity<HouseholdAddress>(entity =>
            {
                entity.HasKey(e => e.guid_houshold_address_id);
            });
            modelBuilder.Entity<HouseholdCaseVisit>(entity =>
            {
                entity.HasKey(e => e.guid_case_visit_id);
            });
            modelBuilder.Entity<HouseholdDataSection>(entity =>
            {
                entity.HasKey(e => e.guid_household_datasection_id);
            });
            modelBuilder.Entity<HouseholdMemberMovement>(entity =>
            {
                entity.HasKey(e => e.guid_household_member_movement_id);
            });
            modelBuilder.Entity<HrBiodata>(entity =>
            {
                entity.HasKey(e => e.HrBiodataId);
            });
            modelBuilder.Entity<HrGender>(entity =>
            {
                entity.HasKey(e => e.HrGenderId);
            });
            modelBuilder.Entity<HrOffice>(entity =>
            {
                entity.HasKey(e => e.HrOfficeId);
            });
            modelBuilder.Entity<ImsLabels>(entity =>
            {
                entity.HasKey(e => e.ImsLabelsId);
            });
            modelBuilder.Entity<CaseVisitType>(entity =>
            {
                entity.HasKey(e => e.guid_case_visit_type__id);
            });
            modelBuilder.Entity<HouseholdMember>(entity =>
            {
                entity.HasKey(e => e.guid_household_member_id);
            });
            modelBuilder.Entity<HouseholdStatusType>(entity =>
            {
                entity.HasKey(e => e.guid_household_status_type_id);
            });
            modelBuilder.Entity<HouseholdMemberMovementType>(entity =>
            {
                entity.HasKey(e => e.guid_household_member_movement_type_id);
            });
            modelBuilder.Entity<Individual>(entity =>
            {
                entity.HasKey(e => e.guid_individual_id);
            });
            modelBuilder.Entity<IndividualContact>(entity =>
            {
                entity.HasKey(e => e.guid_individual_contact_id);
            });
            modelBuilder.Entity<IndividualDocumentType>(entity =>
            {
                entity.HasKey(e => e.guid_document_type_id);
            });
            modelBuilder.Entity<IndividualEducationalLevel>(entity =>
            {
                entity.HasKey(e => e.guid_individual_educational_level_id);
            });
            modelBuilder.Entity<IndividualMaritalStatus>(entity =>
            {
                entity.HasKey(e => e.guid_individual_marital_status_id);
            });
            modelBuilder.Entity<IndividualMovementType>(entity =>
            {
                entity.HasKey(e => e.guid_individual_movement_type_id);
            });
            modelBuilder.Entity<IndividualNationality>(entity =>
            {
                entity.HasKey(e => e.guid_individual_nationality_id);
            });
            modelBuilder.Entity<JrsRegion>(entity =>
            {
                entity.HasKey(e => e.guid_jrs_region_id);
            });
            modelBuilder.Entity<JrsRegionCountry>(entity =>
            {
                entity.HasKey(e => e.guid_JRS_region_country_id);
            });
            modelBuilder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<Neighborhood>(entity =>
            {
                entity.HasKey(e => e.guid_neighborhood_id);
            });
            modelBuilder.Entity<NeighborhoodType>(entity =>
            {
                entity.HasKey(e => e.neighborhood_type_id);
            });
            modelBuilder.Entity<Objective>(entity =>
            {
                entity.HasKey(e => e.guid_objective_id);
            });
            modelBuilder.Entity<ObjectiveCategoryIntervention>(entity =>
            {
                entity.HasKey(e => e.guid_objective_category_intervention_id);
            });
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.permission_id);
            });
            modelBuilder.Entity<PmsAnnualPlan>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.project_id);
            });
            modelBuilder.Entity<ProjectDetail>(entity =>
            {
                entity.HasKey(e => e.guid_project_detail_id);
            });
            modelBuilder.Entity<ProjectDirector>(entity =>
            {
                entity.HasKey(e => e.guid_project_director_id);
            });
            modelBuilder.Entity<ProjectLocation>(entity =>
            {
                entity.HasKey(e => e.guid_project_location_id);
            });
            modelBuilder.Entity<RelationshipsType>(entity =>
            {
                entity.HasKey(e => e.relationships_type_id);
            });
            modelBuilder.Entity<Site>(entity =>
            {
                entity.HasKey(e => e.guid_site_id);
            });
            modelBuilder.Entity<SiteType>(entity =>
            {
                entity.HasKey(e => e.guid_site_type_id);
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserUid);
            });
            modelBuilder.Entity<UserLevel>(entity =>
            {
                entity.HasKey(e => e.user_level_id);
            });
            modelBuilder.Entity<UserPermission>(entity =>
            {
                entity.HasKey(e => e.guid_user_permission_id);
            });
            modelBuilder.Entity<StateType>(entity =>
            {
                entity.HasKey(e => e.guid_state_type_id);
            });
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<Test>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
            modelBuilder.Entity<TestDepartment>(entity =>
            {
                entity.HasKey(e => e.TestDepartmentId);
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
