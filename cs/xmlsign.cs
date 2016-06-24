using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace XmlTools
{
    static class Messages
    { 
        public const string EXC_MissingCert = "Xml file {0} not successfully signed. Please check that one of the required certificates is installed in the system.";
        public const string EXC_CertLocalMachineRights = "Xml file {0} not successfully signed. Please check that the certificate '{1}' installed as LocalMachine has correct read rights. Inner exception: {2}";
        public const string EXC_VerifySignature = "Internal error: cannot apply/verify signature to {0}";
        public const string Msg_SignTry = "Trying to use sha1 {0} with {1}, on {2}."; 
        public const string Str_CUSign = "CurrentUser";
        public const string Str_LMSign = "LocalMachine";
        public const string Msg_KeyFound = "Key found.";
    }

    class XmlSignatureHelper
    {
        private string[] _shaStrings;
        private bool _verbose;

        public XmlSignatureHelper(string[] shaStrings, bool verbose) 
        {
            _shaStrings = shaStrings;
            _verbose = verbose;
        }

        private void Verbose(string msg, params string[] args)
        {
            if (_verbose) 
            {
                Console.WriteLine(msg, args);
            }
        }

        private void Warning(string msg, params string[] args)
        {
            Console.WriteLine(msg, args);
        }

        private void Error(string msg, params string[] args)
        {
            Console.Error.WriteLine(msg, args);
        }

        /// <summary>
        /// Sign the passed XML document using envelope method (signature will be inserted inside the XML)
        /// </summary>
        /// <param name="doc">The XML document to sign</param>
        /// <returns>true if the doc has been signed or however it is still valid file (warning reported)</returns>
        public bool Sign(XmlDocument doc, string xmlFileName)
        {
            if (doc == null || doc.DocumentElement == null)
            {
                throw new ArgumentNullException("doc");
            }

            // Get the certificate which will be used for signign
            X509Certificate2 certificate = RetrieveCertificate();
            if (certificate == null)
            {
                Warning(Messages.EXC_MissingCert, xmlFileName);
                // Still valid
                return true;
            }
            AsymmetricAlgorithm key;
            try
            {
                key = certificate.PrivateKey;
            }
            catch (CryptographicException exc)
            {
                Warning(Messages.EXC_CertLocalMachineRights, xmlFileName, certificate.SubjectName.Decode(X500DistinguishedNameFlags.UseSemicolons), exc.Message);
                return true;
            }

            Reference reference = new Reference { Uri = "" };
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            SignedXml signedXml = new SignedXml(doc) { SigningKey = key };
            signedXml.AddReference(reference);
            
            signedXml.ComputeSignature();

            if (!VerifySignature(signedXml, certificate))
            {
                Error(Messages.EXC_VerifySignature, xmlFileName);
                return false;
            }

            XmlElement xmlDigitalSignature = signedXml.GetXml();
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            return true;
        }

        /// <summary>
        /// Verifies the signature contained in the passed signed xml
        /// </summary>
        /// <param name="signedXml">The signed xml to verify</param>
        /// <param name="cert">The certificate to check against</param>
        /// <returns>true if the signature is verified, false otherwise</returns>
        private bool VerifySignature(SignedXml signedXml, X509Certificate2 cert)
        {
            return signedXml.CheckSignature(cert, true);
        }

        private X509Certificate2 RetrieveCertificate()
        {
            if (_shaStrings.Length > 0)
            {
                return TryStore(new X509Store(StoreName.My, StoreLocation.CurrentUser), Messages.Str_CUSign) ?? TryStore(new X509Store(StoreName.My, StoreLocation.LocalMachine), Messages.Str_LMSign);
            }
            return null;
        }

        private X509Certificate2 TryStore(X509Store store, string storeName)
        {
            X509Certificate2 retValue = null;
            store.Open(OpenFlags.ReadOnly);
            foreach (string sha1 in _shaStrings)
            {
                Verbose(Messages.Msg_SignTry, sha1, "X509Store", storeName);

                X509Certificate2Collection certificateCollection = store.Certificates.Find(X509FindType.FindByThumbprint, sha1, true);
                if (certificateCollection.Count >= 1)
                {
                    retValue = certificateCollection[0];
                    Verbose(Messages.Msg_KeyFound);
                    break;
                }
            }
            store.Close();
            return retValue;
        }
    }

    static class Program
    {
        private static void ShowHelp()
        {
            Console.WriteLine("Usage: xmlsign <infile> [/v | /verbose] [/o | /out <outfile>] /sha1 <sha1> [/sha1 <sha1b> ...]");
        }

        /// <summary>
        /// Take the source XML, the target XML (it can be the same), and the set of the sha1 keys to use 
        /// </summary>
        public static int Main(params string[] args)
        {
            string inputFile = null;
            string outputFile = null;
            bool verbose = false;
            List<string> sha1 = new List<string>();
            for (int i = 0; i < args.Length; i++) 
            {
                if (args[i][0] != '/') 
                {
                    if (inputFile != null)
                    {
                        Console.Error.WriteLine("Multiple input files not supported");
                        return 1;
                    }
                    inputFile = args[i];
                    continue;
                }

                switch (args[i]) 
                {
                    case "/v":
                    case "/verbose":
                        verbose = true;
                        break;
                    case "/o":
                    case "/out":
                        if (i < args.Length - 1) 
                        {
                            outputFile = args[++i];
                        }
                        break;
                    case "/sha1":
                        if (i < args.Length - 1) 
                        {
                            sha1.Add(args[++i]);
                        }
                        break;
                }
            }

            if (inputFile == null)
            {
                Console.Error.WriteLine("Missing input file");
                ShowHelp();
                return 1;
            }
            if (outputFile == null)
            {
                outputFile = inputFile;
            }
            if (sha1.Count == 0)
            {
                Console.Error.WriteLine("Missing SHA1 parameter");
                ShowHelp();
                return 1;
            }

            var helper = new XmlSignatureHelper(sha1.ToArray(), verbose); 
            var doc = new XmlDocument();
            doc.Load(inputFile);
            if (helper.Sign(doc, inputFile))
            {
                doc.Save(outputFile);
                return 0;
            }
            else 
            {
                return 1;
            }
        }
    }
}
