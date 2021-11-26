using System;
using System.IO;
using System.IO.Packaging;
using System.Net.Mime;

namespace FamilyTreeLibrary
{
    public class OPCUtility
    {
        private const string PackageRelationshipType = @"";
        private const string ResourceRelationshipType = @"";

        #region Write Package
        public static void CreatePackage(string PackageFileName, string TargetDirectory)
        {
            using (Package package = Package.Open(PackageFileName, FileMode.Create))
            {
                DirectoryInfo mainDirectory = new DirectoryInfo(TargetDirectory);
                CreatePart(package, mainDirectory, false);
                foreach (DirectoryInfo di in mainDirectory.GetDirectories())
                {
                    CreatePart(package, di, true);
                }
            }
        }

        private static void CreatePart(Package package, DirectoryInfo directoryInfo, bool storeInDirectory)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                switch (file.Extension.ToLower())
                {
                    case ".xml":
                        CreateDocumentPart(package, file, MediaTypeNames.Text.Xml, storeInDirectory);
                        break;
                    case ".jpg":
                        CreateDocumentPart(package, file, MediaTypeNames.Image.Jpeg, storeInDirectory);
                        break;
                    case ".gif":
                        CreateDocumentPart(package, file, MediaTypeNames.Image.Gif, storeInDirectory);
                        break;
                    case ".rtf":
                        CreateDocumentPart(package, file, MediaTypeNames.Text.RichText, storeInDirectory);
                        break;
                    case ".txt":
                        CreateDocumentPart(package, file, MediaTypeNames.Text.Plain, storeInDirectory);
                        break;
                    case ".html":
                        CreateDocumentPart(package, file, MediaTypeNames.Text.Html, storeInDirectory);
                        break;
                }
            }
        }
        private static void CreateDocumentPart(Package package, FileInfo file, string contentType, bool storeInDirectory)
        {
            Uri partUriDocument;
            if (storeInDirectory)
            {
                partUriDocument =
                  PackUriHelper.CreatePartUri(new Uri(Path.Combine(file.Directory.Name, file.Name), UriKind.Relative));
            }
            else
            {
                partUriDocument = PackUriHelper.CreatePartUri(new Uri(file.Name, UriKind.Relative));
            }
            PackagePart packagePartDocument = package.CreatePart(partUriDocument, contentType);
            using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                CopyStream(fileStream, packagePartDocument.GetStream());
            }
            package.CreateRelationship(packagePartDocument.Uri, TargetMode.Internal, PackageRelationshipType);
        }

        #endregion

        #region ReadPackage
        public static void ExtractPackage(string packagePath, string targetDirectory)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(targetDirectory);
                if (directoryInfo.Exists)
                {
                    directoryInfo.Delete(true);
                }

                directoryInfo.Create();
            }
            catch
            {
            }
            using (Package package = Package.Open(packagePath, FileMode.Open, FileAccess.Read))
            {
                PackagePart documentPart = null;
                PackagePart resourcePart = null;
                Uri uriDocumentTarget = null;
                foreach (PackageRelationship relationship in package.GetRelationshipsByType(PackageRelationshipType))
                {
                    uriDocumentTarget = PackUriHelper.ResolvePartUri(new Uri("/", UriKind.Relative), relationship.TargetUri);
                    documentPart = package.GetPart(uriDocumentTarget);
                    ExtractPart(documentPart, targetDirectory);
                }
                Uri uriResourceTarget = null;
                foreach (PackageRelationship relationship in documentPart.GetRelationshipsByType(ResourceRelationshipType))
                {
                    uriResourceTarget = PackUriHelper.ResolvePartUri(documentPart.Uri, relationship.TargetUri);
                    resourcePart = package.GetPart(uriResourceTarget);
                    ExtractPart(resourcePart, targetDirectory);
                }
            }
        }
        private static void ExtractPart(PackagePart packagePart, string targetDirectory)
        {
            string pathToTarget = targetDirectory;
            string stringPart = packagePart.Uri.ToString().TrimStart('/');
            Uri partUri = new Uri(stringPart, UriKind.Relative);
            Uri uriFullPartPath =
             new Uri(new Uri(pathToTarget, UriKind.Absolute), partUri);
            Directory.CreateDirectory(Path.GetDirectoryName(uriFullPartPath.LocalPath));
            using (FileStream fileStream = new FileStream(uriFullPartPath.LocalPath, FileMode.Create))
            {
                CopyStream(packagePart.GetStream(), fileStream);
            }
        }

        #endregion
        private static void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;

            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
            {
                target.Write(buf, 0, bytesRead);
            }
        }
    }
}