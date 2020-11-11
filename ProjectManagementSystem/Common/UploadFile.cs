using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.AccessControl;
using System.Configuration;

namespace ProjectManagementSystem.Common
{
    public class UploadFile
    {
        /// <summary>
        /// Move File
        /// </summary>
        /// <param name="source_file">source_file</param>
        /// <param name="target_file">target_file</param>
        public static void MoveFile(string source_file, string target_file)
        {
            if (File.Exists(source_file))
            {
                if (File.Exists(target_file))
                {
                    File.Delete(target_file);
                }

                File.Move(source_file, target_file);
            }
        }

        /// <summary>
        /// Delete File
        /// </summary>
        /// <param name="filePath">Path of delete file</param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file">file</param>
        /// <returns>tempPath</returns>
        public static string UploadFiles(string saveBaseFilePath, HttpPostedFileBase file, string tempPath)
        {
            DirectoryInfo di = new DirectoryInfo(saveBaseFilePath);
            FileSystemAccessRule fsar = new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow);
            DirectorySecurity ds = null;
            if (!Directory.Exists(saveBaseFilePath))
            {
                Directory.CreateDirectory(saveBaseFilePath);
            }
            ds = di.GetAccessControl();
            ds.AddAccessRule(fsar);

            bool isExists = Directory.Exists(saveBaseFilePath + tempPath);
            if (!isExists)
                Directory.CreateDirectory(saveBaseFilePath + tempPath);

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(saveBaseFilePath + tempPath, fileName);
                file.SaveAs(path);
            }
            return tempPath + "/" + file.FileName;
        }

        public static string SaveAttachFile(HttpPostedFileBase file, string companyCode, string projectId)
        {
            try
            {
                string savePath = Path.Combine(ConfigurationManager.AppSettings[ConfigurationKeys.SAVE_ATTACH_FILE_PATH],
                    ConfigurationManager.AppSettings[ConfigurationKeys.ATTACH_FILE_FOLDER],
                    companyCode,
                    projectId);
                CreateFolder(savePath);

                DirectoryInfo directInfo = new DirectoryInfo(savePath);
                FileSystemAccessRule fullControlRule = new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity directSec = directInfo.GetAccessControl();
                directSec.AddAccessRule(fullControlRule);

                string extension = Path.GetExtension(file.FileName);
                var fileName = Path.GetFileName(file.FileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                var filePath = Path.Combine(savePath, fileName);
                string tempfileName = fileName;

                if (System.IO.File.Exists(filePath))
                {
                    int counter = 2;
                    while (System.IO.File.Exists(filePath))
                    {
                        tempfileName = fileNameWithoutExtension + "(" + counter.ToString() + ")" + extension;
                        filePath = Path.Combine(savePath, tempfileName);
                        counter++;
                    }
                }

                file.SaveAs(filePath);

                return tempfileName;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            DirectoryInfo directInfo = new DirectoryInfo(folderPath);
            FileSystemAccessRule fullControlRule = new FileSystemAccessRule("everyone", FileSystemRights.FullControl, AccessControlType.Allow);
            DirectorySecurity directSec = directInfo.GetAccessControl();
            directSec.AddAccessRule(fullControlRule);
        }
    }
}