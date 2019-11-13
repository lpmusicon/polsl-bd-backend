////////////////////////////////////////////////////////////////////////////////////////////// NIE WIEM CO TO, DO SPRAWDZENIA
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema; 

namespace App
{
  public class DatabaseContext : DbContext{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Receptionist> Receptionists { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<LaboratoryWorker> LaboratoryWorkers { get; set; }
    public DbSet<LaboratoryManager> LaboratoryManagers { get; set; }
    public DbSet<ExaminationDictionary> ExaminationsDictionary { get; set; }
    public DbSet<PhysicalExamination> PhysicalExaminations { get; set; }
    public DbSet<LaboratoryExamination> LaboratoryExaminations { get; set; }
    public DbSet<PatientVisit> PatientVisits { get; set; }
    public DbSet<User> Users { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite("Data Source=database.db");
    }
  }
/////////////////////////////////////////////////////////////////////////////////////////////
  public class Admin{
      public int AdminId { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      public string Lastname { get; set; }
  }

  public class Patient{
      public int PatientId { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      public string Lastname { get; set; }
      [Required]
      public string PESEL { get; set; }

      public ICollection<PatientVisit> PatientVisits { get; set; } // relacja 1 do N
  }

  public class Receptionist{
    [DatabaseGenerated(DatabaseGeneratedOption.None)] 
      public int ReceptionistId { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      public string Lastname { get; set; }

      public ICollection<PatientVisit> RegisteredVisits { get; set; } // relacja 1 do N
  }

  public class Doctor{
    [DatabaseGenerated(DatabaseGeneratedOption.None)] 
      public int DoctorId { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      public string Lastname { get; set; }
      [Required]
      public string PWZNumber { get; set; }

      public ICollection<PatientVisit> PerformedVisits { get; set; } // relacja 1 do N
  }

public class LaboratoryWorker{
  [DatabaseGenerated(DatabaseGeneratedOption.None)] 
      public int LaboratoryWorkerId { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      public string Lastname { get; set; }

      public ICollection<LaboratoryExamination> PerformedExaminations { get; set; } // relacja 1 do N
  }

public class LaboratoryManager{
  [DatabaseGenerated(DatabaseGeneratedOption.None)] 
      public int LaboratoryManagerId { get; set; }
      [Required]
      public string Name { get; set; }
      [Required]
      public string Lastname { get; set; }

      public ICollection<LaboratoryExamination> CheckedExaminations { get; set; } // relacja 1 do N
  }

public class ExaminationDictionary{
      public int ExaminationDictionaryId { get; set; }
      [Required]
      public char Type { get; set; } // Fizykalne/Laboratoryjne
      [Required]
      public string Name { get; set; }

      public ICollection<PhysicalExamination> PhysicalExaminations { get; set; } // relacja 1 do N
      public ICollection<LaboratoryExamination> LaboratoryExaminations { get; set; } // relacja 1 do N
  }

public class PhysicalExamination{
    public int PhysicalExaminationId { get; set; }
    [Required]
    public string Result { get; set; }

    public int PatientVisitId{ get; set; }
    public PatientVisit PatientVisit{ get; set; }
    public int ExaminationDictionaryId{ get; set; }
    public ExaminationDictionary ExaminationDictionary{ get; set; }
}

public class LaboratoryExamination{
    public int LaboratoryExaminationId { get; set; }
    public string Result { get; set; }
    public string DoctorComment { get; set; }
    public DateTime OrderDate { get; set; } // data z godzina
    public DateTime? ExaminationDate { get; set; } // data z godzina
    public string ManagerComment { get; set; }
    public DateTime? ApprovalRejectionDate { get; set; } // data z godzina
    public string Status { get; set; }

    public int PatientVisitId{ get; set; }
    public PatientVisit PatientVisit{ get; set; }
    public int ExaminationDictionaryId{ get; set; }
    public ExaminationDictionary ExaminationDictionary{ get; set; }
    public int? LaboratoryWorkerId{ get; set; } // znak zapytania daje mozliwosc nulla
    public LaboratoryWorker LaboratoryWorker{ get; set; }
    public int? LaboratoryManagerId{ get; set; }
    public LaboratoryManager LaboratoryManager{ get; set; }

}

public class PatientVisit{
  public int PatientVisitId { get; set; }
  public string Description { get; set; }
  public string Diagnosis { get; set; }
  public string Status { get; set; }
  public DateTime RegisterDate { get; set; }
  public DateTime? CloseDate { get; set; } // data zamkniecia wizyty

  public int ReceptionistId{ get; set; }
  public Receptionist Receptionist{ get; set; }
  public int PatientId{ get; set; }
  public Patient Patient { get; set; }
  public int DoctorId{ get; set; }
  public Doctor Doctor{ get; set; }

  public ICollection<LaboratoryExamination> OrderedExaminations { get; set; } // relacja 1 do N
}

public class User{
  public int UserId { get; set; }
  [Required]
  public string Login { get; set; }
  [Required]
  public string Password { get; set; }
  [Required]
  public string Role { get; set; }
  public DateTime DisabledTo { get; set; }
}
}
 