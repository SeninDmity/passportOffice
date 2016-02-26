﻿namespace PassportOffice.DataSource.DataAccess.Repositories.PersonalInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using PassportOffice.DataSource.Context;
    using PassportOffice.DataSource.Model;
    using Searching;

    /// <summary>
    /// Repository with personal data based on database context.
    /// </summary>
    public class PersonalInfoRepository : IPersonalInfoRepositiry
    {
        /// <summary>
        /// Name of table in database which is used to store personal data.
        /// </summary>
        private static readonly string TargetTableName = "PersonInfo";

        /// <summary>
        /// Current context of database.
        /// </summary>
        private PassportOfficeContext passportOfficeContext;

        /// <summary>
        /// Flag which identifies that object was already disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Create an instance of <see cref="PersonalInfoRepository"/> class.
        /// </summary>
        /// <param name="passportOfficeContext">Context of current database.</param>
        internal PersonalInfoRepository(PassportOfficeContext passportOfficeContext)
        {
            this.passportOfficeContext = passportOfficeContext;
        }

        /// <summary>
        /// Finds all data at database.
        /// </summary>
        /// <param name="fullSort">Flag which identifies that records should be ordered by all discussed fields or only by ID.</param>
        /// <returns>All records from database.</returns>
        public IEnumerable<PersonInfo> GetAll(bool fullSort = false)
        {
            List<PersonInfo> personalData = this.SortRecords(this.GetAllFromDB(), fullSort).ToList();
            return personalData;
        }

        /// <summary>
        /// Searches all data considering criteria.
        /// </summary>
        /// <param name="searchOptions">Searching criteria.</param>
        /// <param name="fullSort">Flag which identifies that records should be ordered by all discussed fields or only by ID.</param>
        /// <returns>All records from database which satisfy restrictions.</returns>
        public IEnumerable<PersonInfo> SearchAll(PersonalInfoSearchingOptions searchOptions, bool fullSort = false)
        {
            IQueryable<PersonInfo> dbPersonalInfo = this.GetAllFromDB();
            dbPersonalInfo = this.SortRecords(this.SearchRecords(dbPersonalInfo, searchOptions), fullSort);
            return dbPersonalInfo.ToList();
        }

        /// <summary>
        /// Finds personal data which satisfy criteria and located on page with passed number.
        /// </summary>
        /// <param name="pageSize">Size of page of information.</param>
        /// <param name="pageNumber">Number of page.</param>
        /// <param name="searchOptions">Criteria for selecting data.</param>
        /// <param name="fullSort">Flag which identifies that records should be ordered by all discussed fields or only by ID.</param>
        /// <returns>All records located on requested page.</returns>
        public IEnumerable<PersonInfo> GetPage(int pageSize, int pageNumber, PersonalInfoSearchingOptions searchOptions, bool fullSort = false)
        {
            IQueryable<PersonInfo> dbPersonalInfo = this.GetAllFromDB();
            dbPersonalInfo = this.SortRecords(this.SearchRecords(dbPersonalInfo, searchOptions), fullSort);
            dbPersonalInfo = this.GetPagedRecords(dbPersonalInfo, pageSize, pageNumber);
            return dbPersonalInfo.ToList();
        }

        /// <summary>
        /// Finds record with concete unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of record at database.</param>
        /// <returns>Record with Id equals to passed parameter.</returns>
        public PersonInfo GetById(int id)
        {
            return this.passportOfficeContext.Persons.FirstOrDefault(p => p.ID == id);
        }

        /// <summary>
        /// Clear all personal information records from database.
        /// </summary>
        public void RemoveAll()
        {
            this.passportOfficeContext.Database.ExecuteSqlCommand("delete from " + TargetTableName);
        }

        /// <summary>
        /// Save changes of repository.
        /// </summary>
        public void Save()
        {
            this.passportOfficeContext.SaveChanges();
        }

        /// <summary>
        /// Dispose current instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose current instance.
        /// </summary>
        /// <param name="disposing">Flag which identifies that disposing was invoked by user.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.passportOfficeContext.Dispose();
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Load data from database.
        /// </summary>
        /// <returns>All personal information from database.</returns>
        private IQueryable<PersonInfo> GetAllFromDB()
        {
            return this.passportOfficeContext.Persons.AsQueryable<PersonInfo>();
        }

        /// <summary>
        /// Sort personal information.
        /// It sorts data firstly by last name, then by first name,
        /// middle name, date of birth, series of passport, number of passport.
        /// </summary>
        /// <param name="personalInfo">Data that should be sorted.</param>
        /// <param name="fullSort">Flag which identifies that records should be ordered by all discussed fields or only by ID.</param>
        /// <returns>Sorted collection of personal data.</returns>
        private IQueryable<PersonInfo> SortRecords(IQueryable<PersonInfo> personalInfo, bool fullSort = false)
        {
            if (fullSort)
            {
                return personalInfo.OrderBy(p => p.LastName)
                                    .ThenBy(p => p.FirstName)
                                    .ThenBy(p => p.MiddleName)
                                    .ThenBy(p => p.BirthdayDate)
                                    .ThenBy(p => p.PassportSeries)
                                    .ThenBy(p => p.PassportNumber);
            }
            else
            {
                return personalInfo.OrderBy(p => p.ID);
            }
        }

        /// <summary>
        /// Searches personal data using criteria.
        /// </summary>
        /// <param name="personalInfo">Collection of personal data.</param>
        /// <param name="searchingOptions">Searching criteria.</param>
        /// <returns>Selected records of personal data.</returns>
        private IQueryable<PersonInfo> SearchRecords(IQueryable<PersonInfo> personalInfo, PersonalInfoSearchingOptions searchingOptions)
        {
            if (PersonalInfoSearchingOptions.CheckSearchingOptions(searchingOptions))
            {
                // Add searching by first name if need
                if (searchingOptions.UseFirstName())
                {
                    personalInfo = personalInfo.Where(p => p.FirstName.StartsWith(searchingOptions.FirstName));
                }

                // Add searching by last name if need
                if (searchingOptions.UseLastName())
                {
                    personalInfo = personalInfo.Where(p => p.LastName.StartsWith(searchingOptions.LastName));
                }

                // Add searching by middle name if need
                if (searchingOptions.UseMiddleName())
                {
                    personalInfo = personalInfo.Where(p => p.MiddleName.StartsWith(searchingOptions.MiddleName));
                }

                // Add searching by series of passport if need
                if (searchingOptions.UsePassportSeries())
                {
                    personalInfo = personalInfo.Where(p => p.PassportSeries.StartsWith(searchingOptions.PassportSeries));
                }

                // Add searching by number of passport if need
                if (searchingOptions.UsePassportNumber())
                {
                    personalInfo = personalInfo.Where(p => p.PassportNumber.StartsWith(searchingOptions.PassportNumber));
                }

                // Add searching by date of birthday if need
                if (searchingOptions.UseBirthdayDate())
                {
                    personalInfo = personalInfo.Where(p => p.BirthdayDate.Year == searchingOptions.BirthdayDate.Value.Year
                                                            && p.BirthdayDate.Month == searchingOptions.BirthdayDate.Value.Month
                                                            && p.BirthdayDate.Day == searchingOptions.BirthdayDate.Value.Day);
                }
            }
            return personalInfo;
        }

        /// <summary>
        /// Applies restriction to get data from requested page.
        /// </summary>
        /// <param name="personalInfo">Collection of personal data.</param>
        /// <param name="pageSize">Size of portion of data.</param>
        /// <param name="pageNumber">Number of requested page.</param>
        /// <returns>Collection with applied selecting options.</returns>
        private IQueryable<PersonInfo> GetPagedRecords(IQueryable<PersonInfo> personalInfo, int pageSize, int pageNumber)
        {
            if ((pageSize > 0) && (pageNumber > 0))
            {
                return personalInfo.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            else
            {
                return personalInfo;
            }
        }
    }
}
