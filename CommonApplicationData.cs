﻿//Author: DaveyM69
//Date: 22 May 2010
//Link: http://www.codeproject.com/Tips/61987/Allow-write-modify-access-to-CommonApplicationData

using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FileHasher
{
    internal class CommonApplicationData
    {
        private static readonly string directory =
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        private readonly string applicationFolder;
        private readonly string companyFolder;

        /// <summary>
        ///     Creates a new instance of this class creating the specified company and application folders
        ///     if they don't already exist and optionally allows write/modify to all users.
        /// </summary>
        /// <param name="companyFolder">The name of the company's folder (normally the company name).</param>
        /// <param name="applicationFolder">The name of the application's folder (normally the application name).</param>
        /// <remarks>If the application folder already exists then permissions if requested are NOT altered.</remarks>
        public CommonApplicationData(string companyFolder, string applicationFolder)
            : this(companyFolder, applicationFolder, false)
        {
        }

        /// <summary>
        ///     Creates a new instance of this class creating the specified company and application folders
        ///     if they don't already exist and optionally allows write/modify to all users.
        /// </summary>
        /// <param name="companyFolder">The name of the company's folder (normally the company name).</param>
        /// <param name="applicationFolder">The name of the application's folder (normally the application name).</param>
        /// <param name="allUsers">true to allow write/modify to all users; otherwise, false.</param>
        /// <remarks>If the application folder already exists then permissions if requested are NOT altered.</remarks>
        public CommonApplicationData(string companyFolder, string applicationFolder, bool allUsers)
        {
            this.applicationFolder = applicationFolder;
            this.companyFolder = companyFolder;
            CreateFolders(allUsers);
        }

        /// <summary>
        ///     Gets the path of the application's data folder.
        /// </summary>
        public string ApplicationFolderPath => Path.Combine(CompanyFolderPath, applicationFolder);

        /// <summary>
        ///     Gets the path of the company's data folder.
        /// </summary>
        public string CompanyFolderPath => Path.Combine(directory, companyFolder);

        private void CreateFolders(bool allUsers)
        {
            DirectoryInfo directoryInfo;
            DirectorySecurity directorySecurity;
            AccessRule rule;
            var securityIdentifier = new SecurityIdentifier
                (WellKnownSidType.BuiltinUsersSid, null);
            if (!Directory.Exists(CompanyFolderPath))
            {
                directoryInfo = Directory.CreateDirectory(CompanyFolderPath);
                bool modified;
                directorySecurity = directoryInfo.GetAccessControl();
                rule = new FileSystemAccessRule(
                    securityIdentifier,
                    FileSystemRights.Write |
                    FileSystemRights.ReadAndExecute |
                    FileSystemRights.Modify,
                    AccessControlType.Allow);
                directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out modified);
                directoryInfo.SetAccessControl(directorySecurity);
            }
            if (!Directory.Exists(ApplicationFolderPath))
            {
                directoryInfo = Directory.CreateDirectory(ApplicationFolderPath);
                if (allUsers)
                {
                    bool modified;
                    directorySecurity = directoryInfo.GetAccessControl();
                    rule = new FileSystemAccessRule(
                        securityIdentifier,
                        FileSystemRights.Write |
                        FileSystemRights.ReadAndExecute |
                        FileSystemRights.Modify,
                        InheritanceFlags.ContainerInherit |
                        InheritanceFlags.ObjectInherit,
                        PropagationFlags.InheritOnly,
                        AccessControlType.Allow);
                    directorySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out modified);
                    directoryInfo.SetAccessControl(directorySecurity);
                }
            }
        }

        /// <summary>
        ///     Returns the path of the application's data folder.
        /// </summary>
        /// <returns>The path of the application's data folder.</returns>
        public override string ToString()
        {
            return ApplicationFolderPath;
        }
    }
}