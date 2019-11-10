////////////////////////////////////////////////////////////////////////////////////////////// NIE WIEM CO TO, DO SPRAWDZENIA
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace App
{
  public class DatabaseContext : DbContext{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Receptionist> Receptionists { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<LaboratoryWorker> LaboratoryWorkers { get; set; }
    public DbSet<LaboratoryManager> LaboratoryManagers { get; set; }
    public DbSet<ExaminationDictionary> ExaminationsDictionary { get; set; }
    public DbSet<PhysicalExamination> PhysicalExaminations { get; set; }
    public DbSet<LaboratoryExamination> LaboratoryExaminations { get; set; }
    public DbSet<PatientVisit> PatientVisits { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite("Data Source=database.db");
    }
  }
/////////////////////////////////////////////////////////////////////////////////////////////

  public class Patient{
      public int PatientId { get; set; }
      public string Name { get; set; }
      public string Lastname { get; set; }
      public string PESEL { get; set; }

      public ICollection<PatientVisit> PatientVisits { get; set; } // relacja 1 do N
  }

  public class Receptionist{
      public int ReceptionistId { get; set; }
      public string Name { get; set; }
      public string Lastname { get; set; }

      public ICollection<PatientVisit> RegisteredVisits { get; set; } // relacja 1 do N
  }

  public class Doctor{
      public int DoctorId { get; set; }
      public string Name { get; set; }
      public string Lastname { get; set; }
      public string PWZNumber { get; set; }

      public ICollection<PatientVisit> PerformedVisits { get; set; } // relacja 1 do N
  }

public class LaboratoryWorker{
      public int LaboratoryWorkerId { get; set; }
      public string Name { get; set; }
      public string Lastname { get; set; }

      public ICollection<LaboratoryExamination> PerformedExaminations { get; set; } // relacja 1 do N
  }

public class LaboratoryManager{
      public int LaboratoryManagerId { get; set; }
      public string Name { get; set; }
      public string Lastname { get; set; }

      public ICollection<LaboratoryExamination> CheckedExaminations { get; set; } // relacja 1 do N
  }

public class ExaminationDictionary{
      public int ExaminationDictionaryId { get; set; }
      public char Type { get; set; } // Fizykalne/Laboratoryjne
      public string Name { get; set; }

      public ICollection<PhysicalExamination> PhysicalExaminations { get; set; } // relacja 1 do N
      public ICollection<LaboratoryExamination> LaboratoryExaminations { get; set; } // relacja 1 do N
  }

public class PhysicalExamination{
    public int PhysicalExaminationId { get; set; }
    public string Result { get; set; }
}

public class LaboratoryExamination{
    public int LaboratoryExaminationId { get; set; }
    public string Result { get; set; }
    public string DoctorComment { get; set; }
    public DateTime OrderDate { get; set; } // data z godzina
    public DateTime ExaminationDate { get; set; } // data z godzina
    public string MenagerComment { get; set; }
    public DateTime ApprovalRejectionDate { get; set; } // data z godzina
    public string Status { get; set; }
}

public class PatientVisit{
  public int PatientVisitId { get; set; }
  public string Description { get; set; }
  public string Diagnosis { get; set; }
  public string Status { get; set; }
  public DateTime RegisterDate { get; set; }
  public DateTime CloseDate { get; set; } // data zamkniecia wizyty

  public ICollection<LaboratoryExamination> OrderedExaminations { get; set; } // relacja 1 do N
}
}
 