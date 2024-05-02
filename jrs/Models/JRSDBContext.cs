using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace jrs.Models;

public partial class JRSDBContext : DbContext
{
   
    public JRSDBContext()
    {
    }

    public JRSDBContext(DbContextOptions<JRSDBContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<AddressPropertyType> AddressPropertyTypes { get; set; }

    public virtual DbSet<AddressStatusType> AddressStatusTypes { get; set; }

    public virtual DbSet<CanceledHousehold> CanceledHouseholds { get; set; }

    public virtual DbSet<CanceledHouseholdType> CanceledHouseholdTypes { get; set; }

    public virtual DbSet<CaseVisitType> CaseVisitTypes { get; set; }

    public virtual DbSet<CategoryIntervention> CategoryInterventions { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<ContactType> ContactTypes { get; set; }

    public virtual DbSet<Continent> Continents { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<DataSection> DataSections { get; set; }

    public virtual DbSet<DataSectionDetail> DataSectionDetails { get; set; }

    public virtual DbSet<Distribution> Distributions { get; set; }

    public virtual DbSet<DistributionCommodityType> DistributionCommodityTypes { get; set; }

    public virtual DbSet<DistributionHousehold> DistributionHouseholds { get; set; }

    public virtual DbSet<DistributionIndividual> DistributionIndividuals { get; set; }

    public virtual DbSet<DistributionType> DistributionTypes { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<EducationalLevel> EducationalLevels { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    
    public virtual DbSet<Gender> Genders { get; set; }

   
    public virtual DbSet<Household> Households { get; set; }

    public virtual DbSet<HouseholdAddress> HouseholdAddresses { get; set; }

    public virtual DbSet<HouseholdCaseVisit> HouseholdCaseVisits { get; set; }

    public virtual DbSet<HouseholdDataSection> HouseholdDataSections { get; set; }

    public virtual DbSet<HouseholdDocumentType> HouseholdDocumentTypes { get; set; }

    public virtual DbSet<HouseholdMember> HouseholdMembers { get; set; }

    public virtual DbSet<HouseholdMemberMovement> HouseholdMemberMovements { get; set; }

    public virtual DbSet<HouseholdMemberMovementType> HouseholdMemberMovementTypes { get; set; }

    public virtual DbSet<HouseholdStatusType> HouseholdStatusTypes { get; set; }

    
    public virtual DbSet<Individual> Individuals { get; set; }

    public virtual DbSet<IndividualContact> IndividualContacts { get; set; }

    public virtual DbSet<IndividualDocumentType> IndividualDocumentTypes { get; set; }

    public virtual DbSet<IndividualEducationalLevel> IndividualEducationalLevels { get; set; }

    public virtual DbSet<IndividualMaritalStatus> IndividualMaritalStatuses { get; set; }

    public virtual DbSet<IndividualMovementType> IndividualMovementTypes { get; set; }

    public virtual DbSet<IndividualNationality> IndividualNationalities { get; set; }

    public virtual DbSet<JrsRegion> JrsRegions { get; set; }

    public virtual DbSet<JrsRegionCountry> JrsRegionCountries { get; set; }

   

    public virtual DbSet<MaritalStatus> MaritalStatuses { get; set; }

   

    public virtual DbSet<Neighborhood> Neighborhoods { get; set; }

    public virtual DbSet<NeighborhoodType> NeighborhoodTypes { get; set; }

    public virtual DbSet<Objective> Objectives { get; set; }

    public virtual DbSet<Objective> Objectives1 { get; set; }

    public virtual DbSet<ObjectiveCategoryIntervention> ObjectiveCategoryInterventions { get; set; }

   

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectDetail> ProjectDetails { get; set; }

    public virtual DbSet<ProjectDirector> ProjectDirectors { get; set; }

    public virtual DbSet<ProjectLocation> ProjectLocations { get; set; }


    public virtual DbSet<RelationshipsType> RelationshipsTypes { get; set; }


   

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<SiteType> SiteTypes { get; set; }

    public virtual DbSet<SpecialNeedType> SpecialNeeds { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<StateType> StateTypes { get; set; }


    //gggggggggggggggggggggggggggggggggggggggggggggggggggg
    //GMT Donors
    public virtual DbSet<Donor> Donor { get; set; }

    public virtual DbSet<DonorCategory> DonorCategory { get; set; }
    public virtual DbSet<DonorContact> DonorContact { get; set; }
    public virtual DbSet<DonorContact> DonorCountry { get; set; }

    public virtual DbSet<GrantStatus> GrantStatus { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=.\\; Database=JRSDB; Integrated Security=true; MultipleActiveResultSets=true; Trusted_Connection=True;Encrypt=false");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AddressPropertyType>(entity =>
        {
            entity.HasKey(e => e.guid_address_property_type_id).HasName("PK_Address_Property_Type");

            entity.ToTable("AddressPropertyType", "PME");

            entity.Property(e => e.guid_address_property_type_id).ValueGeneratedNever();
            entity.Property(e => e.address_property_type).HasMaxLength(50);
            entity.Property(e => e.address_property_type_code).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<AddressStatusType>(entity =>
        {
            entity.HasKey(e => e.guid_address_status_type_id).HasName("PK_Address_Status_Type");

            entity.ToTable("AddressStatusType", "PME");

            entity.Property(e => e.guid_address_status_type_id).ValueGeneratedNever();
            entity.Property(e => e.address_status_type).HasMaxLength(50);
            entity.Property(e => e.address_status_type_code).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<CanceledHousehold>(entity =>
        {
            entity.HasKey(e => e.guid_canceled_household).HasName("PK_Canceled_Household");

            entity.ToTable("CanceledHousehold", "PME");

            entity.Property(e => e.guid_canceled_household).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            entity.HasOne(d => d.guid_canceled_household_type).WithMany(p => p.CanceledHouseholds)
                .HasForeignKey(d => d.guid_canceled_household_type_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Canceled_Household_Canceled_Household_Type");

        });

        modelBuilder.Entity<CanceledHouseholdType>(entity =>
        {
            entity.HasKey(e => e.guid_canceled_household_type_id).HasName("PK_Canceled_Household_Type");

            entity.ToTable("CanceledHouseholdType", "PME");

            entity.Property(e => e.guid_canceled_household_type_id).ValueGeneratedNever();
            entity.Property(e => e.canceled_household_type).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<CaseVisitType>(entity =>
        {
            entity.HasKey(e => e.guid_case_visit_type__id).HasName("PK_Case_Visit_Type");

            entity.ToTable("CaseVisitType", "PME");

            entity.Property(e => e.guid_case_visit_type__id).ValueGeneratedNever();
            entity.Property(e => e.case_visit_code).HasMaxLength(10);
            entity.Property(e => e.case_visit_type).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<CategoryIntervention>(entity =>
        {
            entity.HasKey(e => e.guid_nav_category_intervention_id).HasName("PK_Category_Intervention");

            entity.ToTable("CategoryIntervention", "PME");

            entity.Property(e => e.guid_nav_category_intervention_id).ValueGeneratedNever();
            entity.Property(e => e.category_intervention).HasMaxLength(50);
            entity.Property(e => e.category_intervention_code).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.guid_city_id);

            entity.ToTable("City", "IMS");

            entity.Property(e => e.guid_city_id).ValueGeneratedNever();
            entity.Property(e => e.city_name).HasMaxLength(100);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.latitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.longitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_state).WithMany(p => p.Cities)
            //    .HasForeignKey(d => d.guid_state_id)
            //    .HasConstraintName("FK_City_State");
        });

        modelBuilder.Entity<ContactType>(entity =>
        {
            entity.HasKey(e => e.guid_contact_type_id).HasName("PK_Contact_Type");

            entity.ToTable("ContactType", "IMS");

            entity.Property(e => e.guid_contact_type_id).ValueGeneratedNever();
            entity.Property(e => e.contact_type).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<Continent>(entity =>
        {
            entity.HasKey(e => e.guid_continent_id);

            entity.ToTable("Continent", "IMS");

            entity.Property(e => e.guid_continent_id).ValueGeneratedNever();
            entity.Property(e => e.continent_code).HasMaxLength(3);
            entity.Property(e => e.continent_id).ValueGeneratedOnAdd();
            entity.Property(e => e.continent_name).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.guid_country_id);

            entity.ToTable("Country", "IMS");

            entity.Property(e => e.guid_country_id).ValueGeneratedNever();
            entity.Property(e => e.country_code).HasMaxLength(3);
            entity.Property(e => e.country_formal_name).HasMaxLength(100);
            entity.Property(e => e.country_iso2_code).HasMaxLength(2);
            entity.Property(e => e.country_iso3_code).HasMaxLength(3);
            entity.Property(e => e.country_name).HasMaxLength(100);
            entity.Property(e => e.country_native_language_name).HasMaxLength(100);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.flag).HasMaxLength(100);
            entity.Property(e => e.gmt_offset).HasMaxLength(50);
            entity.Property(e => e.internet_code).HasMaxLength(5);
            entity.Property(e => e.latitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.longitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.nationality).HasMaxLength(100);
            entity.Property(e => e.phone_code).HasMaxLength(50);
            entity.Property(e => e.time_zone).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
            entity.Property(e => e.vehicle_code).HasMaxLength(50);

            //entity.HasOne(d => d.guid_continent).WithMany(p => p.Countries)
            //    .HasForeignKey(d => d.guid_continent_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Country_Continent1");
        });

        modelBuilder.Entity<DataSection>(entity =>
        {
            entity.HasKey(e => e.guid_data_section_id).HasName("PK_data_section");

            entity.ToTable("DataSection", "PME");

            entity.Property(e => e.guid_data_section_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.data_section).HasMaxLength(50);
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<DataSectionDetail>(entity =>
        {
            entity.HasKey(e => e.guid_data_section_detail_id).HasName("PK_Data_Section_Detail");

            entity.ToTable("DataSectionDetail", "PME");

            entity.Property(e => e.guid_data_section_detail_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_data_section).WithMany(p => p.DataSectionDetails)
            //    .HasForeignKey(d => d.guid_data_section_id)
            //    .HasConstraintName("FK_Data_Section_Detail_Data_Section");

            //entity.HasOne(d => d.guid_project_location).WithMany(p => p.DataSectionDetails)
            //    .HasForeignKey(d => d.guid_project_location_id)
            //    .HasConstraintName("FK_Data_Section_Detail_Project_Location");
        });

        modelBuilder.Entity<Distribution>(entity =>
        {
            entity.HasKey(e => e.guid_distribution_id);

            entity.ToTable("Distribution", "PME");

            entity.Property(e => e.guid_distribution_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.distribution_date).HasColumnType("datetime");
            entity.Property(e => e.distribution_title).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.distribution_commodity_type).WithMany(p => p.Distributions)
            //    .HasForeignKey(d => d.distribution_commodity_type_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Distribution_DistributionCommodityType");

            //entity.HasOne(d => d.guid_data_section).WithMany(p => p.Distributions)
            //    .HasForeignKey(d => d.guid_data_section_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Distribution_Data_Section");

            //entity.HasOne(d => d.guid_distribution_typeNavigation).WithMany(p => p.Distributions)
            //    .HasForeignKey(d => d.guid_distribution_type)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Distribution_DistributionType");
        });

        modelBuilder.Entity<DistributionCommodityType>(entity =>
        {
            entity.HasKey(e => e.distribution_commodity_type_id);

            entity.ToTable("DistributionCommodityType", "PME");

            entity.Property(e => e.distribution_commodity_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.distribution_commodity_type).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.parent_distribution_commodity_type).WithMany(p => p.Inverseparent_distribution_commodity_type)
            //    .HasForeignKey(d => d.parent_distribution_commodity_type_id)
            //    .HasConstraintName("FK_DistributionCommodityType_DistributionCommodityType");
        });

        modelBuilder.Entity<DistributionHousehold>(entity =>
        {
            entity.HasKey(e => e.guid_distribution_household_id);

            entity.ToTable("DistributionHousehold", "PME");

            entity.Property(e => e.guid_distribution_household_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_distribution).WithMany(p => p.DistributionHouseholds)
            //    .HasForeignKey(d => d.guid_distribution_id)
            //    .HasConstraintName("FK_DistributionHousehold_Distribution");

        });

        modelBuilder.Entity<DistributionIndividual>(entity =>
        {
            entity.HasKey(e => e.guid_distribution_individual_id);

            entity.ToTable("DistributionIndividual", "PME");

            entity.Property(e => e.guid_distribution_individual_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_distribution).WithMany(p => p.DistributionIndividuals)
            //    .HasForeignKey(d => d.guid_distribution_id)
            //    .HasConstraintName("FK_DistributionIndividual_Distribution");

            //entity.HasOne(d => d.guid_individual).WithMany(p => p.DistributionIndividuals)
            //    .HasForeignKey(d => d.guid_individual_id)
            //    .HasConstraintName("FK_DistributionIndividual_Individual");
        });

        modelBuilder.Entity<DistributionType>(entity =>
        {
            entity.HasKey(e => e.guid_distribution_type);

            entity.ToTable("DistributionType", "PME");

            entity.Property(e => e.guid_distribution_type).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.distribution_type).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.HasKey(e => e.guid_document_type_id).HasName("PK_Document_Type");

            entity.ToTable("DocumentType", "IMS");

            entity.Property(e => e.guid_document_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.document_type).HasMaxLength(50);
            entity.Property(e => e.document_type_id).ValueGeneratedOnAdd();
            entity.Property(e => e.folder_path).HasMaxLength(100);
            entity.Property(e => e.js_mask).HasMaxLength(50);
            entity.Property(e => e.max_length).HasMaxLength(50);
            entity.Property(e => e.regular_expression).HasMaxLength(50);
            entity.Property(e => e.sample).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<EducationalLevel>(entity =>
        {
            entity.HasKey(e => e.guid_educational_level_id).HasName("PK_Educational_Level");

            entity.ToTable("EducationalLevel", "IMS");

            entity.Property(e => e.guid_educational_level_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.educational_level).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.guid_employee_id);

            entity.ToTable("Employee", "HRM");

            entity.Property(e => e.guid_employee_id).ValueGeneratedNever();
            entity.Property(e => e.birth_date).HasColumnType("datetime");
            entity.Property(e => e.birth_place).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.father_name_native).HasMaxLength(50);
            entity.Property(e => e.first_name).HasMaxLength(50);
            entity.Property(e => e.first_name_native).HasMaxLength(50);
            entity.Property(e => e.middle_name).HasMaxLength(50);
            entity.Property(e => e.middle_name_native).HasMaxLength(50);
            entity.Property(e => e.mother_name_native).HasMaxLength(50);
            entity.Property(e => e.surname).HasMaxLength(50);
            entity.Property(e => e.surname_native).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

    








        modelBuilder.Entity<Household>(entity =>
        {
            entity.HasKey(e => e.guid_household_id);

            entity.ToTable("Household", "PME");

            entity.Property(e => e.guid_household_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.displacment_date).HasColumnType("date");
            entity.Property(e => e.file_open_date).HasColumnType("date");
            entity.Property(e => e.household_provider).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<HouseholdAddress>(entity =>
        {
            entity.HasKey(e => e.guid_houshold_address_id).HasName("PK_Houshold_Address");

            entity.ToTable("HouseholdAddress", "PME");

            entity.Property(e => e.guid_houshold_address_id).ValueGeneratedNever();
            entity.Property(e => e.address_notes).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.house_mate).HasMaxLength(50);
            entity.Property(e => e.latitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.longitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.rent_value).HasColumnType("decimal(18, 5)");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

        });

        modelBuilder.Entity<HouseholdCaseVisit>(entity =>
        {
            entity.HasKey(e => e.guid_case_visit_id).HasName("PK_Household_Case_Visit");

            entity.ToTable("HouseholdCaseVisit", "PME");

            entity.Property(e => e.guid_case_visit_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_case_visit_type).WithMany(p => p.HouseholdCaseVisits)
            //    .HasForeignKey(d => d.guid_case_visit_type_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Household_Case_Visit_Case_Visit_Type");


            //entity.HasOne(d => d.guid_household_status_type).WithMany(p => p.HouseholdCaseVisits)
            //    .HasForeignKey(d => d.guid_household_status_type_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Household_Case_Visit_Household_Status_Type");
        });

        modelBuilder.Entity<HouseholdDataSection>(entity =>
        {
            entity.HasKey(e => e.guid_household_datasection_id).HasName("PK_Household_DataSection");

            entity.ToTable("HouseholdDataSection", "PME");

            entity.Property(e => e.guid_household_datasection_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_data_section).WithMany(p => p.HouseholdDataSections)
            //    .HasForeignKey(d => d.guid_data_section_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Household_DataSection_Data_Section");

        });

        modelBuilder.Entity<HouseholdDocumentType>(entity =>
        {
            entity.HasKey(e => e.household_document_type_id).HasName("PK_Household_Document_Type");

            entity.ToTable("HouseholdDocumentType", "PME");

            entity.Property(e => e.household_document_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.document_number).HasMaxLength(50);
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.letter).HasMaxLength(50);
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

        });

        modelBuilder.Entity<HouseholdMember>(entity =>
        {
            entity.HasKey(e => e.guid_household_member_id).HasName("PK_Household_Members");

            entity.ToTable("HouseholdMembers", "PME");

            entity.Property(e => e.guid_household_member_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");


            //entity.HasOne(d => d.guid_individual).WithMany(p => p.HouseholdMembers)
            //    .HasForeignKey(d => d.guid_individual_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Household_Members_Individual");

            //entity.HasOne(d => d.relationships_type).WithMany(p => p.HouseholdMembers)
            //    .HasForeignKey(d => d.relationships_type_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Household_Members_Relationships_Type");
        });

        modelBuilder.Entity<HouseholdMemberMovement>(entity =>
        {
            entity.HasKey(e => e.guid_household_member_movement_id).HasName("PK_Household_Member_Movement");

            entity.ToTable("HouseholdMemberMovement", "PME");

            entity.Property(e => e.guid_household_member_movement_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.description).HasMaxLength(50);
            entity.Property(e => e.movement_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            entity.HasOne(d => d.guid_household_member_movement_type).WithMany(p => p.HouseholdMemberMovements)
                .HasForeignKey(d => d.guid_household_member_movement_type_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Household_Member_Movement_Household_Member_Movement_Type");

            //entity.HasOne(d => d.old_guid_household_member).WithMany(p => p.HouseholdMemberMovements)
            //    .HasForeignKey(d => d.old_guid_household_member_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Household_Member_Movement_Household_Members");
        });

        modelBuilder.Entity<HouseholdMemberMovementType>(entity =>
        {
            entity.HasKey(e => e.guid_household_member_movement_type_id).HasName("PK_Household_Member_Movement_Type");

            entity.ToTable("HouseholdMemberMovementType", "PME");

            entity.Property(e => e.guid_household_member_movement_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.description).HasMaxLength(50);
            entity.Property(e => e.household_member_movement_type).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<HouseholdStatusType>(entity =>
        {
            entity.HasKey(e => e.guid_household_status_type_id).HasName("PK_Household_Status_Type");

            entity.ToTable("HouseholdStatusType", "PME");

            entity.Property(e => e.guid_household_status_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.household_code).HasMaxLength(10);
            entity.Property(e => e.household_status_type).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });




        modelBuilder.Entity<Individual>(entity =>
        {
            entity.HasKey(e => e.guid_individual_id);

            entity.ToTable("Individual", "PME");

            entity.Property(e => e.guid_individual_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.date_of_birth).HasColumnType("datetime");
            entity.Property(e => e.individual_father_name).HasMaxLength(50);
            entity.Property(e => e.individual_father_name_native_lang).HasMaxLength(50);
            entity.Property(e => e.individual_first_name).HasMaxLength(50);
            entity.Property(e => e.JRS_individual_identifier).HasMaxLength(13);

            entity.Property(e => e.individual_first_name_native_lang).HasMaxLength(50);
            entity.Property(e => e.individual_last_name).HasMaxLength(50);
            entity.Property(e => e.individual_last_name_native_lang).HasMaxLength(50);
            entity.Property(e => e.individual_mother_name).HasMaxLength(50);
            entity.Property(e => e.individual_mother_name_native_lang).HasMaxLength(50);
            entity.Property(e => e.photo_path).HasMaxLength(100);
            entity.Property(e => e.place_of_birth).HasMaxLength(50);
            entity.Property(e => e.place_of_birth_native_lang).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<IndividualContact>(entity =>
        {
            entity.HasKey(e => e.guid_individual_contact_id).HasName("PK_Individual_Contact");

            entity.ToTable("IndividualContact", "PME");

            entity.Property(e => e.guid_individual_contact_id).ValueGeneratedNever();
            entity.Property(e => e.contact_value).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_contact_type).WithMany(p => p.IndividualContacts)
            //    .HasForeignKey(d => d.guid_contact_type_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Individual_Contact_Contact_Type");

            //entity.HasOne(d => d.guid_individual).WithMany(p => p.IndividualContacts)
            //    .HasForeignKey(d => d.guid_individual_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Individual_Contact_Individual");
        });

        modelBuilder.Entity<IndividualDocumentType>(entity =>
        {
            entity.HasKey(e => e.guid_document_type_id);

        });

        modelBuilder.Entity<IndividualEducationalLevel>(entity =>
        {
            entity.HasKey(e => e.guid_individual_educational_level_id).HasName("PK_Individual_Educational_Level");

            entity.ToTable("IndividualEducationalLevel", "PME");

            entity.Property(e => e.guid_individual_educational_level_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_educational_level).WithMany(p => p.IndividualEducationalLevels)
            //    .HasForeignKey(d => d.guid_educational_level_id)
            //    .HasConstraintName("FK_Individual_Educational_Level_Educational_Level");

            //entity.HasOne(d => d.guid_individual).WithMany(p => p.IndividualEducationalLevels)
            //    .HasForeignKey(d => d.guid_individual_id)
            //    .HasConstraintName("FK_Individual_Educational_Level_Individual");
        
        });

        modelBuilder.Entity<IndividualMaritalStatus>(entity =>
        {
            entity.HasKey(e => e.guid_individual_marital_status_id).HasName("PK_Individual_Marital_Status");

            entity.ToTable("IndividualMaritalStatus", "PME");

            entity.Property(e => e.guid_individual_marital_status_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_individual).WithMany(p => p.IndividualMaritalStatuses)
            //    .HasForeignKey(d => d.guid_individual_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Individual_Marital_Status_Individual");

        });

        modelBuilder.Entity<IndividualMovementType>(entity =>
        {
            entity.HasKey(e => e.guid_individual_movement_type_id).HasName("PK_Individual_Movement_Type");

            entity.ToTable("IndividualMovementType", "PME");

            entity.Property(e => e.guid_individual_movement_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.individual_movement_type).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<IndividualNationality>(entity =>
        {
            entity.HasKey(e => e.guid_individual_nationality_id).HasName("PK_Individual_Nationality");

            entity.ToTable("IndividualNationality", "PME");

            entity.Property(e => e.guid_individual_nationality_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_country).WithMany(p => p.IndividualNationalities)
            //    .HasForeignKey(d => d.guid_country_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Individual_Nationality_Country");

            //entity.HasOne(d => d.guid_individual).WithMany(p => p.IndividualNationalities)
            //    .HasForeignKey(d => d.guid_individual_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_Individual_Nationality_Individual");
        });

        modelBuilder.Entity<JrsRegion>(entity =>
        {
            entity.HasKey(e => e.guid_jrs_region_id).HasName("PK_JRS_Region");

            entity.ToTable("JrsRegion", "IMS");

            entity.Property(e => e.guid_jrs_region_id).ValueGeneratedNever();
            entity.Property(e => e.JRS_region_code).HasMaxLength(50);
            entity.Property(e => e.JRS_region_id).ValueGeneratedOnAdd();
            entity.Property(e => e.JRS_region_name).HasMaxLength(50);
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<JrsRegionCountry>(entity =>
        {
            entity.HasKey(e => e.guid_JRS_region_country_id).HasName("PK_JRS_Region_Country");

            entity.ToTable("JrsRegionCountry", "IMS");

            entity.Property(e => e.guid_JRS_region_country_id).ValueGeneratedNever();
            entity.Property(e => e.JRS_region_country_id).ValueGeneratedOnAdd();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_jrs_region).WithMany(p => p.JrsRegionCountries)
            //    .HasForeignKey(d => d.guid_jrs_region_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_JrsRegionCountry_JrsRegion");
        });

        modelBuilder.Entity<MaritalStatus>(entity =>
        {
            entity.HasKey(e => e.guid_marital_status_id).HasName("PK_Marital_Status");

            entity.ToTable("MaritalStatus", "IMS");

            entity.Property(e => e.guid_marital_status_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.marital_status).HasMaxLength(50);
            entity.Property(e => e.marital_status_code).HasMaxLength(10);
            entity.Property(e => e.marital_status_id).ValueGeneratedOnAdd();
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });


        modelBuilder.Entity<Neighborhood>(entity =>
        {
            entity.HasKey(e => e.guid_neighborhood_id).HasName("PK_Neighborhood_1");

            entity.ToTable("Neighborhood", "IMS");

            entity.Property(e => e.guid_neighborhood_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.latitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.longitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.neighborhood_id).ValueGeneratedOnAdd();
            entity.Property(e => e.neighborhood_name).HasMaxLength(100);
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_city).WithMany(p => p.Neighborhoods)
            //    .HasForeignKey(d => d.guid_city_id)
            //    .HasConstraintName("FK_Neighborhood_City");

            //entity.HasOne(d => d.neighborhood_type).WithMany(p => p.Neighborhoods)
            //    .HasForeignKey(d => d.neighborhood_type_id)
            //    .HasConstraintName("FK_Neighborhood_Neighborhood_Type");
        });

        modelBuilder.Entity<NeighborhoodType>(entity =>
        {
            entity.HasKey(e => e.neighborhood_type_id).HasName("PK_Neighborhood_Type");

            entity.ToTable("NeighborhoodType", "IMS");

            entity.Property(e => e.neighborhood_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.neighborhood_type).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<Objective>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Objective");

            entity.Property(e => e.objective_code).HasMaxLength(255);
        });

        modelBuilder.Entity<Objective>(entity =>
        {
            entity.HasKey(e => e.guid_objective_id);

            entity.ToTable("Objective", "IMS");

            entity.Property(e => e.guid_objective_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.objective_code).HasMaxLength(50);
            entity.Property(e => e.objective_id).ValueGeneratedOnAdd();
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_project).WithMany(p => p.Objective1s)
            //    .HasForeignKey(d => d.guid_project_id)
            //    .HasConstraintName("FK_Objective_Project");
        });

        modelBuilder.Entity<ObjectiveCategoryIntervention>(entity =>
        {
            entity.HasKey(e => e.guid_objective_category_intervention_id).HasName("PK_Objective_Category_Intervention");

            entity.ToTable("ObjectiveCategoryIntervention", "PME");

            entity.Property(e => e.guid_objective_category_intervention_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            entity.HasOne(d => d.guid_objective).WithMany(p => p.ObjectiveCategoryInterventions)
                .HasForeignKey(d => d.guid_objective_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ObjectiveCategoryIntervention_Objective");
        });



        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.guid_project_id);

            entity.ToTable("Project", "IMS");

            entity.Property(e => e.guid_project_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.project_code).HasMaxLength(50);
            entity.Property(e => e.project_id).ValueGeneratedOnAdd();
            entity.Property(e => e.project_name).HasMaxLength(100);
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_JRS_region_country).WithMany(p => p.Projects)
            //    .HasForeignKey(d => d.guid_JRS_region_country_id)
            //    .HasConstraintName("FK_Project_JrsRegionCountry");

            //entity.HasOne(d => d.guid_parent_project).WithMany(p => p.Inverseguid_parent_project)
            //    .HasForeignKey(d => d.guid_parent_project_id)
            //    .HasConstraintName("FK_Project_Project");
        });

        modelBuilder.Entity<ProjectDetail>(entity =>
        {
            entity.HasKey(e => e.guid_project_detail_id);

            entity.ToTable("ProjectDetail", "PME");

            entity.Property(e => e.guid_project_detail_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_project).WithMany(p => p.ProjectDetails)
            //    .HasForeignKey(d => d.guid_project_id)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_ProjectDetail_Project");
        });

        modelBuilder.Entity<ProjectDirector>(entity =>
        {
            entity.HasKey(e => e.guid_project_director_id).HasName("PK_Project_Director");

            entity.ToTable("ProjectDirector", "HRM");

            entity.Property(e => e.guid_project_director_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.end_date).HasColumnType("datetime");
            entity.Property(e => e.project_director_id).ValueGeneratedOnAdd();
            entity.Property(e => e.start_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_project).WithMany(p => p.ProjectDirectors)
            //    .HasForeignKey(d => d.guid_project_id)
            //    .HasConstraintName("FK_Project_Director_Project");
        });

        modelBuilder.Entity<ProjectLocation>(entity =>
        {
            entity.HasKey(e => e.guid_project_location_id).HasName("PK_Project_Address");

            entity.ToTable("ProjectLocation", "IMS");

            entity.Property(e => e.guid_project_location_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_project).WithMany(p => p.ProjectLocations)
            //    .HasForeignKey(d => d.guid_project_id)
            //    .HasConstraintName("FK_Project_Location_Project");
        });


        modelBuilder.Entity<RelationshipsType>(entity =>
        {
            entity.HasKey(e => e.relationships_type_id).HasName("PK_Household_Relationships_Type");

            entity.ToTable("RelationshipsType", "PME");

            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.relationships_type).HasMaxLength(50);
            entity.Property(e => e.relationships_type_code).HasMaxLength(10);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });



        modelBuilder.Entity<Site>(entity =>
        {
            entity.HasKey(e => e.guid_site_id).HasName("PK_Subsidiary");

            entity.ToTable("Site", "IMS");

            entity.Property(e => e.guid_site_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.site_description).HasMaxLength(100);
            entity.Property(e => e.site_id).ValueGeneratedOnAdd();
            entity.Property(e => e.site_name).HasMaxLength(100);
            entity.Property(e => e.updated_date).HasColumnType("datetime");

            //entity.HasOne(d => d.guid_site_type).WithMany(p => p.Sites)
            //    .HasForeignKey(d => d.guid_site_type_id)
            //    .HasConstraintName("FK_Site_Site_Type");
        });

        modelBuilder.Entity<SiteType>(entity =>
        {
            entity.HasKey(e => e.guid_site_type_id).HasName("PK_Station_Type");

            entity.ToTable("SiteType", "IMS");

            entity.Property(e => e.guid_site_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.site_type_id).ValueGeneratedOnAdd();
            entity.Property(e => e.site_type_name).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<SpecialNeedType>(entity =>
        {
            entity.HasKey(e => e.guid_special_need_type_id);

            entity.ToTable("SpecialNeed", "IMS");

            entity.Property(e => e.guid_special_need_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.special_need_type).HasMaxLength(50);
            entity.Property(e => e.special_need_type_code).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.guid_state_id);

            entity.ToTable("State", "IMS");

            entity.Property(e => e.guid_state_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.iso_code).HasMaxLength(260);
            entity.Property(e => e.native_language).HasMaxLength(260);
            entity.Property(e => e.phone_code).HasMaxLength(260);
            entity.Property(e => e.postal_code).HasMaxLength(260);
            entity.Property(e => e.state_name).HasMaxLength(260);
            entity.Property(e => e.updated_date).HasColumnType("datetime");


        });

        modelBuilder.Entity<StateType>(entity =>
        {
            entity.HasKey(e => e.guid_state_type_id).HasName("PK_State_Type");

            entity.ToTable("StateType", "IMS");

            entity.Property(e => e.guid_state_type_id).ValueGeneratedNever();
            entity.Property(e => e.created_date).HasColumnType("datetime");
            entity.Property(e => e.state_type_id).ValueGeneratedOnAdd();
            entity.Property(e => e.state_type_name).HasMaxLength(50);
            entity.Property(e => e.updated_date).HasColumnType("datetime");
        });

        //Grant


    }




    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
