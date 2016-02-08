﻿namespace PassportOffice.API.DataAccess
{
    using System.Collections.Generic;

    using PassportOffice.DataSource.Model;
    using PassportOffice.DataSource.UnitOfWork;
    using PassportOffice.DataSource.Searching;

    /// <summary>
    /// Communicates with data source classes to work with passport information.
    /// </summary>
    public class PersonalInfoManager
    {
        /// <summary>
        /// Collection of repositories contains information from database.
        /// </summary>
        private UnitOfWork reposCollection;

        /// <summary>
        /// Create instance of <see cref="PersonalInfoManager"/> class.
        /// </summary>
        public PersonalInfoManager()
        {
            reposCollection = new UnitOfWork();
        }

        /// <summary>
        /// Finds all records of personal information.
        /// </summary>
        /// <returns>All passport data.</returns>
        public IEnumerable<PersonInfo> GetAll()
        {
            return this.reposCollection.PersonalInfoRepository.GetAll();
        }

        /// <summary>
        /// Finds all records of personal data using for selection searching criteria.
        /// </summary>
        /// <param name="searchingOptions">Restriction which used to select data.</param>
        /// <returns>All passport data satisfy to searching criteria.</returns>
        public IEnumerable<PersonInfo> SearchAll(PersonalInfoSearchingOptions searchingOptions)
        {
            return this.reposCollection.PersonalInfoRepository.SearchAll(searchingOptions);
        }
    }
}